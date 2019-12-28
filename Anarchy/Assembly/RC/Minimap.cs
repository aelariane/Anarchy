using System.Collections;
using Anarchy;
using Anarchy.Configuration;
using Anarchy.InputPos;
using Optimization.Caching;
using RC;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Minimap : MonoBehaviour
{
    private bool assetsInitialized = false;
    private static UnityEngine.Sprite borderSprite;
    private RectTransform borderT;
    private Canvas canvas;
    private Vector2 cornerPosition;
    private float cornerSizeRatio;
    private Minimap.Preset initialPreset;
    public static Minimap instance;
    private bool isEnabled;
    private bool isEnabledTemp;
    private Vector3 lastMinimapCenter;
    private float lastMinimapOrthoSize;
    private Camera lastUsedCamera;
    private bool maximized = false;
    private RectTransform minimap;
    private float MINIMAP_CORNER_SIZE;
    private Vector2 MINIMAP_ICON_SIZE;
    private float MINIMAP_POINTER_DIST;
    private float MINIMAP_POINTER_SIZE;
    private int MINIMAP_SIZE;
    private Vector2 MINIMAP_SUPPLY_SIZE;
    private Minimap.MinimapIcon[] minimapIcons;
    private bool minimapIsCreated = false;
    private RectTransform minimapMaskT;
    private Bounds minimapOrthographicBounds;
    public RenderTexture minimapRT;
    public Camera myCam;
    private static UnityEngine.Sprite pointerSprite;
    private CanvasScaler scaler;
    private static UnityEngine.Sprite supplySprite;
    private static UnityEngine.Sprite whiteIconSprite;

    private void AddBorderToTexture(ref Texture2D texture, Color borderColor, int borderPixelSize)
    {
        int num = texture.width * borderPixelSize;
        Color[] array = new Color[num];
        for (int i = 0; i < num; i++)
        {
            array[i] = borderColor;
        }
        texture.SetPixels(0, texture.height - borderPixelSize, texture.width - 1, borderPixelSize, array);
        texture.SetPixels(0, 0, texture.width, borderPixelSize, array);
        texture.SetPixels(0, 0, borderPixelSize, texture.height, array);
        texture.SetPixels(texture.width - borderPixelSize, 0, borderPixelSize, texture.height, array);
        texture.Apply();
    }

    private void AutomaticSetCameraProperties(Camera cam)
    {
        Renderer[] array = FindObjectsOfType<Renderer>();
        bool flag = array.Length != 0;
        if (flag)
        {
            this.minimapOrthographicBounds = new Bounds(array[0].transform.position, Vectors.zero);
            for (int i = 0; i < array.Length; i++)
            {
                bool flag2 = array[i].gameObject.layer == 9;
                if (flag2)
                {
                    this.minimapOrthographicBounds.Encapsulate(array[i].bounds);
                }
            }
        }
        Vector3 size = this.minimapOrthographicBounds.size;
        float num = (size.x > size.z) ? size.x : size.z;
        size.z = (size.x = num);
        this.minimapOrthographicBounds.size = size;
        cam.orthographic = true;
        cam.orthographicSize = num * 0.5f;
        Vector3 center = this.minimapOrthographicBounds.center;
        center.y = cam.farClipPlane * 0.5f;
        Transform transform = cam.transform;
        transform.position = center;
        transform.eulerAngles = new Vector3(90f, 0f, 0f);
        cam.aspect = 1f;
        this.lastMinimapCenter = center;
        this.lastMinimapOrthoSize = cam.orthographicSize;
    }

    private void AutomaticSetOrthoBounds()
    {
        Renderer[] array = UnityEngine.Object.FindObjectsOfType<Renderer>();
        bool flag = array.Length != 0;
        if (flag)
        {
            this.minimapOrthographicBounds = new Bounds(array[0].transform.position, Vectors.zero);
            for (int i = 0; i < array.Length; i++)
            {
                this.minimapOrthographicBounds.Encapsulate(array[i].bounds);
            }
        }
        Vector3 size = this.minimapOrthographicBounds.size;
        float num = (size.x > size.z) ? size.x : size.z;
        size.z = (size.x = num);
        this.minimapOrthographicBounds.size = size;
        this.lastMinimapCenter = this.minimapOrthographicBounds.center;
        this.lastMinimapOrthoSize = num * 0.5f;
    }

    private void Awake()
    {
        Minimap.instance = this;
    }

    private Texture2D CaptureMinimap(Camera cam)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        Texture2D texture2D = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGB24, false)
        {
            filterMode = FilterMode.Bilinear
        };
        texture2D.ReadPixels(new Rect(0f, 0f, (float)cam.targetTexture.width, (float)cam.targetTexture.height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = active;
        return texture2D;
    }

    private void CaptureMinimapRT(Camera cam)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = this.minimapRT;
        cam.targetTexture = this.minimapRT;
        cam.Render();
        RenderTexture.active = active;
    }

    private void CheckUserInput()
    {
        if (Settings.Minimap.Value && !GameModes.MinimapDisable.Enabled)
        {
            if (this.minimapIsCreated)
            {
                if (InputManager.IsInputRebind((int)InputRebinds.MinimapToggle))
                {
                    this.SetEnabled(!this.isEnabled);
                    return;
                }
                if (isEnabled)
                {
                    if (InputManager.IsInputRebindHolding((int)InputRebinds.MinimapMax))
                    {
                        if (!this.maximized)
                        {
                            this.Maximize();
                        }
                    }
                    else
                    {
                        if (maximized)
                        {
                            this.Minimize();
                        }
                    }
                    if (this.maximized)
                    {
                        bool needRecapture = false;
                        if (InputManager.IsInputRebindHolding((int)InputRebinds.MinimapReset))
                        {
                            if (this.initialPreset != null)
                            {
                                this.ManualSetCameraProperties(this.lastUsedCamera, this.initialPreset.center, this.initialPreset.orthographicSize);
                            }
                            else
                            {
                                this.AutomaticSetCameraProperties(this.lastUsedCamera);
                            }
                            needRecapture = true;
                        }
                        else
                        {
                            float num = Input.GetAxis("Mouse ScrollWheel");
                            bool flag11 = num != 0f;
                            if (flag11)
                            {
                                if (Input.GetKey(KeyCode.LeftShift))
                                {
                                    num *= 3f;
                                }
                                this.lastMinimapOrthoSize = Mathf.Max(this.lastMinimapOrthoSize + num, 1f);
                                needRecapture = true;
                            }
                            if (Input.GetKey(KeyCode.UpArrow))
                            {
                                float num2 = Time.deltaTime * ((Input.GetKey(KeyCode.LeftShift) ? 2f : 0.75f) * this.lastMinimapOrthoSize);
                                this.lastMinimapCenter.z = this.lastMinimapCenter.z + num2;
                                needRecapture = true;
                            }
                            else
                            {
                                if (Input.GetKey(KeyCode.DownArrow))
                                {
                                    float num2 = Time.deltaTime * ((Input.GetKey(KeyCode.LeftShift) ? 2f : 0.75f) * this.lastMinimapOrthoSize);
                                    this.lastMinimapCenter.z = this.lastMinimapCenter.z - num2;
                                    needRecapture = true;
                                }
                            }
                            if (Input.GetKey(KeyCode.RightArrow))
                            {
                                float num2 = Time.deltaTime * ((Input.GetKey(KeyCode.LeftShift) ? 2f : 0.75f) * this.lastMinimapOrthoSize);
                                this.lastMinimapCenter.x = this.lastMinimapCenter.x + num2;
                                needRecapture = true;
                            }
                            else
                            {
                                if (Input.GetKey(KeyCode.LeftArrow))
                                {
                                    float num2 = Time.deltaTime * ((Input.GetKey(KeyCode.LeftShift) ? 2f : 0.75f) * this.lastMinimapOrthoSize);
                                    this.lastMinimapCenter.x = this.lastMinimapCenter.x - num2;
                                    needRecapture = true;
                                }
                            }
                        }
                        if (needRecapture)
                        {
                            this.RecaptureMinimap(this.lastUsedCamera, this.lastMinimapCenter, this.lastMinimapOrthoSize);
                        }
                    }
                }
            }
        }
        else
        {
            if (this.isEnabled)
            {
                this.SetEnabled(false);
            }
        }
    }

    public void CreateMinimap(Camera cam, int minimapResolution = 512, float cornerSize = 0.3f, Minimap.Preset mapPreset = null)
    {
        this.isEnabled = true;
        this.lastUsedCamera = cam;
        if (!this.assetsInitialized)
        {
            this.Initialize();
        }
        GameObject lightGO = GameObject.Find("mainLight");
        Light light = null;
        Quaternion rotation = Quaternion.identity;
        LightShadows shadows = LightShadows.None;
        Color color = Colors.clear;
        float intensity = 0f;
        float nearClipPlane = cam.nearClipPlane;
        float farClipPlane = cam.farClipPlane;
        int cullingMask = cam.cullingMask;
        if (lightGO != null)
        {
            light = lightGO.GetComponent<Light>();
            rotation = light.transform.rotation;
            shadows = light.shadows;
            intensity = light.intensity;
            color = light.color;
            light.shadows = LightShadows.None;
            light.color = Colors.white;
            light.intensity = 0.5f;
            light.transform.eulerAngles = new Vector3(90f, 0f, 0f);
        }
        cam.nearClipPlane = 0.3f;
        cam.farClipPlane = 1000f;
        cam.cullingMask = 512;
        cam.clearFlags = CameraClearFlags.Color;
        this.MINIMAP_SIZE = minimapResolution;
        this.MINIMAP_CORNER_SIZE = (float)this.MINIMAP_SIZE * cornerSize;
        this.cornerSizeRatio = cornerSize;
        this.CreateMinimapRT(cam, minimapResolution);
        if (mapPreset != null)
        {
            this.initialPreset = mapPreset;
            this.ManualSetCameraProperties(cam, mapPreset.center, mapPreset.orthographicSize);
        }
        else
        {
            this.AutomaticSetCameraProperties(cam);
        }
        this.CaptureMinimapRT(cam);
        if (lightGO != null)
        {
            light.shadows = shadows;
            light.transform.rotation = rotation;
            light.color = color;
            light.intensity = intensity;
        }
        cam.nearClipPlane = nearClipPlane;
        cam.farClipPlane = farClipPlane;
        cam.cullingMask = cullingMask;
        cam.orthographic = false;
        cam.clearFlags = CameraClearFlags.Skybox;
        this.CreateUnityUIRT(minimapResolution);
        this.minimapIsCreated = true;
        base.StartCoroutine(this.HackRoutine());
    }

    private void CreateMinimapRT(Camera cam, int pixelSize)
    {
        if (minimapRT == null)
        {
            bool flag2 = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB565);
            int num = flag2 ? 4 : 7;
            this.minimapRT = new RenderTexture(pixelSize, pixelSize, 16, RenderTextureFormat.RGB565);
            if (!flag2)
            {
                Debug.LogWarning(SystemInfo.graphicsDeviceName + " (" + SystemInfo.graphicsDeviceVendor + ") does not support RGB565 format, the minimap will have transparency issues on certain maps");
            }
        }
        cam.targetTexture = this.minimapRT;
    }

    private void CreateUnityUI(Texture2D map, int minimapResolution)
    {
        GameObject gameObject = new GameObject("Canvas");
        gameObject.AddComponent<RectTransform>();
        this.canvas = gameObject.AddComponent<Canvas>();
        this.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        this.scaler = gameObject.AddComponent<CanvasScaler>();
        this.scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        this.scaler.referenceResolution = new Vector2(900f, 600f);
        this.scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        GameObject gameObject2 = new GameObject("CircleMask");
        gameObject2.transform.SetParent(gameObject.transform, false);
        this.minimapMaskT = gameObject2.AddComponent<RectTransform>();
        gameObject2.AddComponent<CanvasRenderer>();
        RectTransform rectTransform = this.minimapMaskT;
        Vector2 vector = this.minimapMaskT.anchorMax = Vector2.one;
        rectTransform.anchorMin = vector;
        float num = this.MINIMAP_CORNER_SIZE * 0.5f;
        this.cornerPosition = new Vector2(-(num + 5f), -(num + 70f));
        this.minimapMaskT.anchoredPosition = this.cornerPosition;
        this.minimapMaskT.sizeDelta = new Vector2(this.MINIMAP_CORNER_SIZE, this.MINIMAP_CORNER_SIZE);
        GameObject gameObject3 = new GameObject("Minimap");
        gameObject3.transform.SetParent(this.minimapMaskT, false);
        this.minimap = gameObject3.AddComponent<RectTransform>();
        gameObject3.AddComponent<CanvasRenderer>();
        RectTransform rectTransform2 = this.minimap;
        RectTransform rectTransform3 = this.minimap;
        vector = new Vector2(0.5f, 0.5f);
        rectTransform3.anchorMax = vector;
        rectTransform2.anchorMin = vector;
        this.minimap.anchoredPosition = Vector2.zero;
        this.minimap.sizeDelta = this.minimapMaskT.sizeDelta;
        Image image = gameObject3.AddComponent<Image>();
        Rect rect = new Rect(0f, 0f, (float)map.width, (float)map.height);
        image.sprite = UnityEngine.Sprite.Create(map, rect, new Vector3(0.5f, 0.5f));
        image.type = Image.Type.Simple;
    }

    private void CreateUnityUIRT(int minimapResolution)
    {
        GameObject gameObject = new GameObject("Canvas");
        gameObject.AddComponent<RectTransform>();
        this.canvas = gameObject.AddComponent<Canvas>();
        this.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        this.scaler = gameObject.AddComponent<CanvasScaler>();
        this.scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        this.scaler.referenceResolution = new Vector2(800f, 600f);
        this.scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        this.scaler.matchWidthOrHeight = 1f;
        GameObject gameObject2 = new GameObject("Mask");
        gameObject2.transform.SetParent(gameObject.transform, false);
        this.minimapMaskT = gameObject2.AddComponent<RectTransform>();
        gameObject2.AddComponent<CanvasRenderer>();
        RectTransform rectTransform = this.minimapMaskT;
        Vector2 vector = this.minimapMaskT.anchorMax = Vector2.one;
        rectTransform.anchorMin = vector;
        float num = this.MINIMAP_CORNER_SIZE * 0.5f;
        this.cornerPosition = new Vector2(-(num + 5f), -(num + 70f));
        this.minimapMaskT.anchoredPosition = this.cornerPosition;
        this.minimapMaskT.sizeDelta = new Vector2(this.MINIMAP_CORNER_SIZE, this.MINIMAP_CORNER_SIZE);
        GameObject gameObject3 = new GameObject("MapBorder");
        gameObject3.transform.SetParent(this.minimapMaskT, false);
        this.borderT = gameObject3.AddComponent<RectTransform>();
        RectTransform rectTransform2 = this.borderT;
        RectTransform rectTransform3 = this.borderT;
        vector = new Vector2(0.5f, 0.5f);
        rectTransform3.anchorMax = vector;
        rectTransform2.anchorMin = vector;
        this.borderT.sizeDelta = this.minimapMaskT.sizeDelta;
        gameObject3.AddComponent<CanvasRenderer>();
        Image image = gameObject3.AddComponent<Image>();
        image.sprite = Minimap.borderSprite;
        image.type = Image.Type.Sliced;
        GameObject gameObject4 = new GameObject("Minimap");
        gameObject4.transform.SetParent(this.minimapMaskT, false);
        this.minimap = gameObject4.AddComponent<RectTransform>();
        this.minimap.SetAsFirstSibling();
        gameObject4.AddComponent<CanvasRenderer>();
        RectTransform rectTransform4 = this.minimap;
        RectTransform rectTransform5 = this.minimap;
        vector = new Vector2(0.5f, 0.5f);
        rectTransform5.anchorMax = vector;
        rectTransform4.anchorMin = vector;
        this.minimap.anchoredPosition = Vector2.zero;
        this.minimap.sizeDelta = this.minimapMaskT.sizeDelta;
        RawImage rawImage = gameObject4.AddComponent<RawImage>();
        rawImage.texture = this.minimapRT;
        rawImage.maskable = true;
        gameObject4.AddComponent<Mask>().showMaskGraphic = true;
    }

    private Vector2 GetSizeForStyle(IconStyle style)
    {
        if (style == IconStyle.Circle)
        {
            return MINIMAP_ICON_SIZE;
        }
        else if (style == IconStyle.Supply)
        {
            return MINIMAP_SUPPLY_SIZE;
        }
        return Vectors.v2zero;
    }

    private static UnityEngine.Sprite GetSpriteForStyle(IconStyle style)
    {
        if (style == IconStyle.Circle)
        {
            return whiteIconSprite;
        }
        else if (style == IconStyle.Supply)
        {
            return supplySprite;
        }
        return null;
    }

    private IEnumerator HackRoutine()
    {
        yield return new WaitForEndOfFrame();
        this.RecaptureMinimap(this.lastUsedCamera, this.lastMinimapCenter, this.lastMinimapOrthoSize);
        yield break;
    }

    private void Initialize()
    {
        Vector3 v = new Vector3(0.5f, 0.5f);
        Texture2D texture2D = (Texture2D)RCManager.Load("icon");
        Rect rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
        Minimap.whiteIconSprite = UnityEngine.Sprite.Create(texture2D, rect, v);
        texture2D = (Texture2D)RCManager.Load("iconpointer");
        rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
        Minimap.pointerSprite = UnityEngine.Sprite.Create(texture2D, rect, v);
        texture2D = (Texture2D)RCManager.Load("supplyicon");
        rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
        Minimap.supplySprite = UnityEngine.Sprite.Create(texture2D, rect, v);
        texture2D = (Texture2D)RCManager.Load("mapborder");
        rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
        Vector4 border = new Vector4(5f, 5f, 5f, 5f);
        Minimap.borderSprite = UnityEngine.Sprite.Create(texture2D, rect, v, 100f, 1u, SpriteMeshType.FullRect, border);
        this.MINIMAP_ICON_SIZE = new Vector2((float)Minimap.whiteIconSprite.texture.width, (float)Minimap.whiteIconSprite.texture.height);
        this.MINIMAP_POINTER_SIZE = (float)(Minimap.pointerSprite.texture.width + Minimap.pointerSprite.texture.height) / 2f;
        this.MINIMAP_POINTER_DIST = (this.MINIMAP_ICON_SIZE.x + this.MINIMAP_ICON_SIZE.y) * 0.25f;
        this.MINIMAP_SUPPLY_SIZE = new Vector2((float)Minimap.supplySprite.texture.width, (float)Minimap.supplySprite.texture.height);
        this.assetsInitialized = true;
    }

    private void ManualSetCameraProperties(Camera cam, Vector3 centerPoint, float orthoSize)
    {
        Transform transform = cam.transform;
        centerPoint.y = cam.farClipPlane * 0.5f;
        transform.position = centerPoint;
        transform.eulerAngles = new Vector3(90f, 0f, 0f);
        cam.orthographic = true;
        cam.orthographicSize = orthoSize;
        float num = orthoSize * 2f;
        this.minimapOrthographicBounds = new Bounds(centerPoint, new Vector3(num, 0f, num));
        this.lastMinimapCenter = centerPoint;
        this.lastMinimapOrthoSize = orthoSize;
    }

    private void ManualSetOrthoBounds(Vector3 centerPoint, float orthoSize)
    {
        float num = orthoSize * 2f;
        this.minimapOrthographicBounds = new Bounds(centerPoint, new Vector3(num, 0f, num));
        this.lastMinimapCenter = centerPoint;
        this.lastMinimapOrthoSize = orthoSize;
    }

    public void Maximize()
    {
        this.isEnabledTemp = true;
        if (!this.isEnabled)
        {
            this.SetEnabledTemp(true);
        }
        RectTransform rectTransform = this.minimapMaskT;
        RectTransform rectTransform2 = this.minimapMaskT;
        Vector2 vector = new Vector2(0.5f, 0.5f);
        rectTransform2.anchorMax = vector;
        rectTransform.anchorMin = vector;
        this.minimapMaskT.anchoredPosition = Vector2.zero;
        this.minimapMaskT.sizeDelta = new Vector2((float)this.MINIMAP_SIZE, (float)this.MINIMAP_SIZE);
        this.minimap.sizeDelta = this.minimapMaskT.sizeDelta;
        this.borderT.sizeDelta = this.minimapMaskT.sizeDelta;
        if (this.minimapIcons != null)
        {
            for (int i = 0; i < this.minimapIcons.Length; i++)
            {
                MinimapIcon minimapIcon = this.minimapIcons[i];
                if (minimapIcon != null)
                {
                    minimapIcon.SetSize(this.GetSizeForStyle(minimapIcon.Style));
                    if (minimapIcon.Rotation)
                    {
                        minimapIcon.SetPointerSize(this.MINIMAP_POINTER_SIZE, this.MINIMAP_POINTER_DIST);
                    }
                }
            }
        }
        this.maximized = true;
    }

    public void Minimize()
    {
        this.isEnabledTemp = false;
        if (!this.isEnabled)
        {
            this.SetEnabledTemp(false);
        }
        this.minimapMaskT.anchorMin = (this.minimapMaskT.anchorMax = Vector2.one);
        this.minimapMaskT.anchoredPosition = this.cornerPosition;
        this.minimapMaskT.sizeDelta = new Vector2(this.MINIMAP_CORNER_SIZE, this.MINIMAP_CORNER_SIZE);
        this.minimap.sizeDelta = this.minimapMaskT.sizeDelta;
        this.borderT.sizeDelta = this.minimapMaskT.sizeDelta;
        if (this.minimapIcons != null)
        {
            float num = 1f - ((float)this.MINIMAP_SIZE - this.MINIMAP_CORNER_SIZE) / (float)this.MINIMAP_SIZE;
            float num2 = this.MINIMAP_POINTER_SIZE * num;
            num2 = Mathf.Max(num2, this.MINIMAP_POINTER_SIZE * 0.5f);
            float num3 = (this.MINIMAP_POINTER_SIZE - num2) / this.MINIMAP_POINTER_SIZE;
            num3 = this.MINIMAP_POINTER_DIST * num3;
            for (int i = 0; i < this.minimapIcons.Length; i++)
            {
                MinimapIcon minimapIcon = this.minimapIcons[i];
                if (minimapIcon != null)
                {
                    Vector2 sizeForStyle = this.GetSizeForStyle(minimapIcon.Style);
                    sizeForStyle.x = Mathf.Max(sizeForStyle.x * num, sizeForStyle.x * 0.5f);
                    sizeForStyle.y = Mathf.Max(sizeForStyle.y * num, sizeForStyle.y * 0.5f);
                    minimapIcon.SetSize(sizeForStyle);
                    if (minimapIcon.Rotation)
                    {
                        minimapIcon.SetPointerSize(num2, num3);
                    }
                }
            }
        }
        this.maximized = false;
    }

    public static void OnScreenResolutionChanged()
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.ScreenResolutionChangedRoutine());
        }
    }

    private void RecaptureMinimap()
    {
        if (lastUsedCamera != null)
        {
            this.RecaptureMinimap(this.lastUsedCamera, this.lastMinimapCenter, this.lastMinimapOrthoSize);
        }
    }

    private void RecaptureMinimap(Camera cam, Vector3 centerPosition, float orthoSize)
    {
        if (minimap != null)
        {
            GameObject gameObject = GameObject.Find("mainLight");
            Light light = null;
            Quaternion rotation = Quaternion.identity;
            LightShadows shadows = LightShadows.None;
            Color color = Color.clear;
            float intensity = 0f;
            float nearClipPlane = cam.nearClipPlane;
            float farClipPlane = cam.farClipPlane;
            int cullingMask = cam.cullingMask;
            bool flag2 = gameObject != null;
            if (flag2)
            {
                light = gameObject.GetComponent<Light>();
                rotation = light.transform.rotation;
                shadows = light.shadows;
                color = light.color;
                intensity = light.intensity;
                light.shadows = LightShadows.None;
                light.color = Color.white;
                light.intensity = 0.5f;
                light.transform.eulerAngles = new Vector3(90f, 0f, 0f);
            }
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 1000f;
            cam.clearFlags = CameraClearFlags.Color;
            cam.cullingMask = 512;
            this.CreateMinimapRT(cam, this.MINIMAP_SIZE);
            this.ManualSetCameraProperties(cam, centerPosition, orthoSize);
            this.CaptureMinimapRT(cam);
            if (gameObject != null)
            {
                light.shadows = shadows;
                light.transform.rotation = rotation;
                light.color = color;
                light.intensity = intensity;
            }
            cam.nearClipPlane = nearClipPlane;
            cam.farClipPlane = farClipPlane;
            cam.cullingMask = cullingMask;
            cam.orthographic = false;
            cam.clearFlags = CameraClearFlags.Skybox;
        }
    }

    private IEnumerator ScreenResolutionChangedRoutine()
    {
        yield return 0;
        this.RecaptureMinimap();
        yield break;
    }

    public void SetEnabled(bool enabled)
    {
        this.isEnabled = enabled;
        if (this.canvas != null)
        {
            this.canvas.gameObject.SetActive(enabled);
        }
    }

    public void SetEnabledTemp(bool enabled)
    {
        isEnabledTemp = enabled;
        if (this.canvas != null)
        {
            this.canvas.gameObject.SetActive(enabled);
        }
    }

    private void TrackGameObject(GameObject objToTrack, Color iconColor, bool trackOrientation, bool depthAboveAll = false, Minimap.IconStyle iconStyle = Minimap.IconStyle.Circle)
    {
        if (minimap != null)
        {
            Minimap.MinimapIcon minimapIcon;
            if (trackOrientation)
            {
                minimapIcon = Minimap.MinimapIcon.CreateWithRotation(this.minimap, objToTrack, iconStyle, this.MINIMAP_POINTER_DIST);
            }
            else
            {
                minimapIcon = Minimap.MinimapIcon.Create(this.minimap, objToTrack, iconStyle);
            }
            minimapIcon.SetColor(iconColor);
            minimapIcon.SetDepth(depthAboveAll);
            Vector2 sizeForStyle = this.GetSizeForStyle(iconStyle);
            if (this.maximized)
            {
                minimapIcon.SetSize(sizeForStyle);
                if (minimapIcon.Rotation)
                {
                    minimapIcon.SetPointerSize(this.MINIMAP_POINTER_SIZE, this.MINIMAP_POINTER_DIST);
                }
            }
            else
            {
                float num = 1f - ((float)this.MINIMAP_SIZE - this.MINIMAP_CORNER_SIZE) / (float)this.MINIMAP_SIZE;
                sizeForStyle.x = Mathf.Max(sizeForStyle.x * num, sizeForStyle.x * 0.5f);
                sizeForStyle.y = Mathf.Max(sizeForStyle.y * num, sizeForStyle.y * 0.5f);
                minimapIcon.SetSize(sizeForStyle);
                if (minimapIcon.Rotation)
                {
                    float num2 = this.MINIMAP_POINTER_SIZE * num;
                    num2 = Mathf.Max(num2, this.MINIMAP_POINTER_SIZE * 0.5f);
                    float num3 = (this.MINIMAP_POINTER_SIZE - num2) / this.MINIMAP_POINTER_SIZE;
                    num3 = this.MINIMAP_POINTER_DIST * num3;
                    minimapIcon.SetPointerSize(num2, num3);
                }
            }
            if (this.minimapIcons == null)
            {
                this.minimapIcons = new MinimapIcon[] { minimapIcon };
            }
            else
            {
                Minimap.MinimapIcon[] array = new Minimap.MinimapIcon[this.minimapIcons.Length + 1];
                for (int i = 0; i < this.minimapIcons.Length; i++)
                {
                    array[i] = this.minimapIcons[i];
                }
                array[array.Length - 1] = minimapIcon;
                this.minimapIcons = array;
            }
        }
    }

    public static void TrackGameObjectOnMinimap(GameObject objToTrack, Color iconColor, bool trackOrientation, bool depthAboveAll = false, IconStyle iconStyle = IconStyle.Circle)
    {
        if(instance != null)
        {
            instance.TrackGameObject(objToTrack, iconColor, trackOrientation, depthAboveAll, iconStyle);
        }
    }

    public static void TryRecaptureInstance()
    {
        if (instance != null)
        {
            instance.RecaptureMinimap();
        }
    }

    public IEnumerator TryRecaptureInstanceE(float time)
    {
        yield return new WaitForSeconds(time);
        TryRecaptureInstance();
        yield break;
    }

    private void Update()
    {
        this.CheckUserInput();
        if ((this.isEnabled || this.isEnabledTemp) && this.minimapIsCreated && this.minimapIcons != null)
        {
            for (int i = 0; i < this.minimapIcons.Length; i++)
            {
                Minimap.MinimapIcon minimapIcon = this.minimapIcons[i];
                bool flag2 = minimapIcon == null;
                if (flag2)
                {
                    Anarchy.AnarchyExtensions.RemoveAt<Minimap.MinimapIcon>(ref this.minimapIcons, i);
                }
                else
                {
                    bool flag3 = !minimapIcon.UpdateUI(this.minimapOrthographicBounds, this.maximized ? ((float)this.MINIMAP_SIZE) : this.MINIMAP_CORNER_SIZE);
                    if (flag3)
                    {
                        minimapIcon.Destroy();
                        Anarchy.AnarchyExtensions.RemoveAt<Minimap.MinimapIcon>(ref this.minimapIcons, i);
                    }
                }
            }
        }
    }

    public static void WaitAndTryRecaptureInstance(float time)
    {
        Minimap.instance.StartCoroutine(Minimap.instance.TryRecaptureInstanceE(time));
    }


    public enum IconStyle
    {
        Circle,
        Supply
    }

    public class MinimapIcon
    {
        private Transform obj;
        private RectTransform pointerRect;
        public readonly bool Rotation;
        public readonly Minimap.IconStyle Style;
        private RectTransform uiRect;

        public MinimapIcon(GameObject trackedObject, GameObject uiElement, Minimap.IconStyle style)
        {
            this.Rotation = false;
            this.Style = style;
            this.obj = trackedObject.transform;
            this.uiRect = uiElement.GetComponent<RectTransform>();
            CatchDestroy component = this.obj.GetComponent<CatchDestroy>();
            if (component == null)
            {
                this.obj.gameObject.AddComponent<CatchDestroy>().target = uiElement;
            }
            else
            {
                if (component.target != null && component.target != uiElement)
                {
                    UnityEngine.Object.Destroy(component.target);
                }
                else
                {
                    component.target = uiElement;
                }
            }
        }

        public MinimapIcon(GameObject trackedObject, GameObject uiElement, GameObject uiPointer, Minimap.IconStyle style)
        {
            this.Rotation = true;
            this.Style = style;
            this.obj = trackedObject.transform;
            this.uiRect = uiElement.GetComponent<RectTransform>();
            this.pointerRect = uiPointer.GetComponent<RectTransform>();
            CatchDestroy component = this.obj.GetComponent<CatchDestroy>();
            if (component == null)
            {
                this.obj.gameObject.AddComponent<CatchDestroy>().target = uiElement;
            }
            else
            {
                if (component.target != null && component.target != uiElement)
                {
                    UnityEngine.Object.Destroy(component.target);
                }
                else
                {
                    component.target = uiElement;
                }
            }
        }

        public static Minimap.MinimapIcon Create(RectTransform parent, GameObject trackedObject, Minimap.IconStyle style)
        {
            UnityEngine.Sprite spriteForStyle = Minimap.GetSpriteForStyle(style);
            GameObject gameObject = new GameObject("MinimapIcon");
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = (rectTransform.anchorMax = new Vector3(0.5f, 0.5f));
            rectTransform.sizeDelta = new Vector2((float)spriteForStyle.texture.width, (float)spriteForStyle.texture.height);
            Image image = gameObject.AddComponent<Image>();
            image.sprite = spriteForStyle;
            image.type = Image.Type.Simple;
            gameObject.transform.SetParent(parent, false);
            return new Minimap.MinimapIcon(trackedObject, gameObject, style);
        }

        public static Minimap.MinimapIcon CreateWithRotation(RectTransform parent, GameObject trackedObject, Minimap.IconStyle style, float pointerDist)
        {
            UnityEngine.Sprite spriteForStyle = Minimap.GetSpriteForStyle(style);
            GameObject gameObject = new GameObject("MinimapIcon");
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = (rectTransform.anchorMax = new Vector3(0.5f, 0.5f));
            rectTransform.sizeDelta = new Vector2((float)spriteForStyle.texture.width, (float)spriteForStyle.texture.height);
            Image image = gameObject.AddComponent<Image>();
            image.sprite = spriteForStyle;
            image.type = Image.Type.Simple;
            gameObject.transform.SetParent(parent, false);
            GameObject gameObject2 = new GameObject("IconPointer");
            RectTransform rectTransform2 = gameObject2.AddComponent<RectTransform>();
            rectTransform2.anchorMin = (rectTransform2.anchorMax = rectTransform.anchorMin);
            rectTransform2.sizeDelta = new Vector2((float)Minimap.pointerSprite.texture.width, (float)Minimap.pointerSprite.texture.height);
            Image image2 = gameObject2.AddComponent<Image>();
            image2.sprite = Minimap.pointerSprite;
            image2.type = Image.Type.Simple;
            gameObject2.transform.SetParent(rectTransform, false);
            rectTransform2.anchoredPosition = new Vector2(0f, pointerDist);
            return new Minimap.MinimapIcon(trackedObject, gameObject, gameObject2, style);
        }

        public void Destroy()
        {
            if (this.uiRect != null)
            {
                Object.Destroy(this.uiRect.gameObject);
            }
        }

        public void SetColor(Color color)
        {
            if (this.uiRect != null)
            {
                this.uiRect.GetComponent<Image>().color = color;
            }
        }

        public void SetDepth(bool aboveAll)
        {
            if (this.uiRect != null)
            {
                if (aboveAll)
                {
                    this.uiRect.SetAsLastSibling();
                }
                else
                {
                    this.uiRect.SetAsFirstSibling();
                }
            }
        }

        public void SetPointerSize(float size, float originDistance)
        {
            if (this.pointerRect != null)
            {
                this.pointerRect.sizeDelta = new Vector2(size, size);
                this.pointerRect.anchoredPosition = new Vector2(0f, originDistance);
            }
        }

        public void SetSize(Vector2 size)
        {
            if (this.uiRect != null)
            {
                this.uiRect.sizeDelta = size;
            }
        }

        public bool UpdateUI(Bounds worldBounds, float minimapSize)
        {
            if (this.obj == null || uiRect == null)
            {
                return false;
            }
            float x = worldBounds.size.x;
            Vector3 vector = this.obj.position - worldBounds.center;
            vector.y = vector.z;
            vector.z = 0f;
            float num = Mathf.Abs(vector.x) / x;
            vector.x = ((vector.x < 0f) ? (-num) : num);
            float num2 = Mathf.Abs(vector.y) / x;
            vector.y = ((vector.y < 0f) ? (-num2) : num2);
            Vector2 anchoredPosition = vector * minimapSize;
            this.uiRect.anchoredPosition = anchoredPosition;
            if (this.Rotation)
            {
                float z = Mathf.Atan2(this.obj.forward.z, this.obj.forward.x) * 57.29578f - 90f;
                this.uiRect.eulerAngles = new Vector3(0f, 0f, z);
            }
            return true;
        }
    }

    public class Preset
    {
        public Preset(Vector3 center, float orthographicSize)
        {
            this.center = center;
            this.orthographicSize = orthographicSize;
        }

        public readonly Vector3 center;

        public readonly float orthographicSize;
    }
}
