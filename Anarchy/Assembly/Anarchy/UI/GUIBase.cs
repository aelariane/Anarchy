using Anarchy.UI.Animation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Anarchy.UI
{
    /// <summary>
    /// Base class for GUIs in Anarchy
    /// </summary>
    public abstract class GUIBase
    {
        private static readonly List<int> usedLayers = new List<int>();
        private static Texture2D cachedEmptyTexture;
        private static int maxLayer = 0;
        private readonly Dictionary<string, Texture2D> textureCache;
        protected GUIAnimation animator;
        public readonly string Name;

        public Action OnGUI = () => { };

        /// <summary>
        /// Localization file
        /// </summary>
        /// <remarks>Automatically loads when GUI is enabled and unloads when GUI is disabled, due to lower consumable RAM usage</remarks>
        protected Localization.Locale locale { get; }

        /// <summary>
        /// List of all instantiated <seealso cref="GUIBase"/>
        /// </summary>
        public static List<GUIBase> AllBases { get; private set; }

        /// <summary>
        /// Completely transparent texture
        /// </summary>
        public static Texture2D EmptyTexture
        {
            get
            {
                if (cachedEmptyTexture == null)
                {
                    cachedEmptyTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    cachedEmptyTexture.SetPixel(0, 0, Optimization.Caching.Colors.empty);
                    cachedEmptyTexture.Apply();
                }
                return cachedEmptyTexture;
            }
        }

        /// <summary>
        /// Directory of GUI
        /// </summary>
        /// <remarks>Ends with '/' symbol (Directory separator). Location:  "AoTTG_Data/Resources/DirectoryName"</remarks>
        public string Directory { get; }

        /// <summary>
        /// Drawer object
        /// </summary>
        public GUIDrawer Drawer { get; }

        /// <summary>
        /// If this GUI is on the screen
        /// </summary>
        public bool IsActive { get; private set; } = false;

        /// <summary>
        /// GUI Layer, indicates order of drawing
        /// </summary>
        public int Layer { get; }

        ~GUIBase()
        {
            lock (usedLayers)
            {
                int indexToRemove = -1;
                for (int i = 0; i < usedLayers.Count; i++)
                {
                    if (usedLayers[i] == Layer)
                    {
                        indexToRemove = i;
                        break;
                    }
                }
                if (indexToRemove >= 0)
                {
                    usedLayers.RemoveAt(indexToRemove);
                }

                UpdateMaxLayer();
            }
            if (AllBases.Contains(this))
            {
                AllBases.Remove(this);
            }
        }

        public GUIBase(string name) : this(name, -1)
        {
        }

        public GUIBase(string name, int layer)
        {
            Name = name;
            Layer = GetLayer(layer, name);
            Directory = Application.dataPath + "/Resources/" + Name + "/";
            if (!System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.CreateDirectory(Directory);
            }
            textureCache = new Dictionary<string, Texture2D>();
            locale = new Localization.Locale(name);
            animator = new NoneAnimation(this);
            if (AllBases == null)
            {
                AllBases = new List<GUIBase>();
            }
            AllBases.Add(this);
            Drawer = new GUIDrawer(this);
        }

        private static int GetLayer(int layerToSet, string name)
        {
            lock (usedLayers)
            {
                if (layerToSet >= 0)
                {
                    if (usedLayers.Contains(layerToSet))
                    {
                        Debug.LogError($"Attemption to create GUIBase with already existing layer. Please make sure you wanted to use this one. Layer: {layerToSet}, GUIBase name: {name}");
                        return GetLayer(++layerToSet, name);
                    }
                    usedLayers.Add(layerToSet);
                    UpdateMaxLayer();
                    return layerToSet;
                }
                maxLayer = usedLayers.Max();
                usedLayers.Add(++maxLayer);
                return maxLayer;
            }
        }

        private static void UpdateMaxLayer()
        {
            int max = -1;
            for (int i = 0; i < usedLayers.Count; i++)
            {
                if (usedLayers[i] > max)
                {
                    max = usedLayers[i];
                }
            }
            maxLayer = max;
        }

        /// <summary>
        /// Calls at the moment when GUI was disabled
        /// </summary>
        /// <remarks>If GUI is using <seealso cref="GUIAnimation"/>, then will be called after animation was fully executed</remarks>
        protected virtual void OnDisable()
        {
        }

        /// <summary>
        /// Calls at the momentrwhen GUI was enabled
        /// </summary>
        /// <remarks>If GUI is using <seealso cref="GUIAnimation"/>, then will be called after animation was fully executed</remarks>
        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// Draws the GUI
        /// </summary>
        /// <remarks>Basically, you should use it instead of OnGUI method of Unity. Put all the GUI calls inside it, and done.</remarks>
        protected internal abstract void Draw();

        /// <summary>
        /// Loads picture as <seealso cref="Texture2D"/> from given  GUI
        /// </summary>
        /// <param name="base">Base to load picture from</param>
        /// <param name="name">Picture name without full path</param>
        /// <param name="ext">Extension of picture. png by default</param>
        /// <returns></returns>
        /// <remarks>Acceptable extensions: jpeg, png, jpg. If <paramref name="ext"/> is <seealso cref="string.Empty"/>, then it is possible to specify extension in <paramref name="name"/>. Example: name.png</remarks>
        public static Texture2D LoadTexture(GUIBase @base, string name, string ext = "png")
        {
            return @base.LoadTexture(name, ext);
        }

        /// <summary>
        /// Clears texture cache
        /// </summary>
        public void ClearCache()
        {
            textureCache.Clear();
        }

        /// <summary>
        /// Disables GUI with closing animation
        /// </summary>
        public void Disable()
        {
            if (!IsActive)
            {
                return;
            }

            OnGUI = animator.StartClose(() =>
            {
                DisableImmediate();
            });
        }

        /// <summary>
        /// Disables GUI instantly, without animations
        /// </summary>
        public void DisableImmediate()
        {
            if (UIManager.Disable(this))
            {
                Drawer.Disable();
                OnDisable();
                locale.Unload();
                IsActive = false;
                OnGUI = null;
            }
        }

        /// <summary>
        /// Enables GUI with open animation
        /// </summary>
        public void Enable()
        {
            if (IsActive)
            {
                return;
            }

            if (UIManager.Enable(this))
            {
                locale.Load();
                OnEnable();
                OnGUI = animator.StartOpen(() => { });
                IsActive = true;
            }
        }

        /// <summary>
        /// Enables GUI instantly, without animations
        /// </summary>
        public void EnableImmediate()
        {
            if (UIManager.Enable(this))
            {
                locale.Load();
                OnEnable();
                IsActive = true;
                OnGUI = Draw;
            }
        }

        /// <summary>
        /// Enables <paramref name="next"/> GUI once calling object will be disabled. Uses animations
        /// </summary>
        /// <param name="next">Next GUI to be activated</param>
        public void EnableNext(GUIBase next)
        {
            if (!IsActive)
            {
                return;
            }

            OnGUI = animator.StartClose(() =>
            {
                if (UIManager.Disable(this))
                {
                    OnDisable();
                    locale.Unload();
                    IsActive = false;
                    next.Enable();
                }
            });
        }

        /// <summary>
        /// Loads picture as <seealso cref="Texture2D"/> from <see cref="Directory"/> of GUI
        /// </summary>
        /// <param name="namebase">Picture name, without full path</param>
        /// <param name="ext">Extension of picture. png by default</param>
        /// <returns>Picture as <seealso cref="Texture2D"/></returns>
        /// <remarks>Acceptable extensions: jpeg, png, jpg. If <paramref name="ext"/> is <seealso cref="string.Empty"/>, then it is possible to specify extension in <paramref name="name"/>. Example: name.png</remarks>
        public Texture2D LoadTexture(string namebase, string ext)
        {
            if (textureCache.TryGetValue(namebase, out Texture2D res) && res != null)
            {
                return res;
            }
            string name = namebase;
            bool error = false;
            if (ext == string.Empty)
            {
                if (!name.EndsWith(".png") && !name.EndsWith(".jpg") && !name.EndsWith(".jpeg"))
                {
                    error = true;
                }
            }
            else
            {
                if (!ext.Equals("png") && !ext.Equals("jpg") && !ext.Equals("jpeg"))
                {
                    error = true;
                }

                name += "." + ext;
            }
            if (error)
            {
                Debug.LogError($"You should use png, jpg or jpeg extensions for loading Texture2D");
                return Texture2D.blackTexture;
            }
            string path = Directory + name;
            if (!File.Exists(path))
            {
                Debug.LogError($"File what you are trying to load doesnt't exist: \"{path}\"");
                return Texture2D.blackTexture;
            }
            res = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            res.LoadImage(File.ReadAllBytes(path));
            res.Apply();
            textureCache.Add(namebase, res);
            return res;
        }

        /// <summary>
        /// Loads .ogv file as <seealso cref="MovieTexture"/> from <see cref="Directory"/> of GUI
        /// </summary>
        /// <param name="namebase">Name of file without full path and extension</param>
        /// <param name="ext">Extension of file</param>
        /// <returns></returns>
        public MovieTexture LoadVideo(string namebase)
        {
            MovieTexture tex;
            string name = namebase;
            string path = Directory + name + ".ogv";
            if (!File.Exists(path))
            {
                Debug.LogError($"File what you are trying to load doesnt't exist: \"{path}\"");
                return null;
            }
            WWW www = new WWW("file://" + path);
            if (www.texture == null)
            {
                Debug.LogError($"Null texture");
                www.Dispose();
                GC.SuppressFinalize(www);
                return null;
            }
            tex = www.movie;
            www.Dispose();
            GC.SuppressFinalize(www);
            return tex;
        }

        /// <summary>
        /// Calls when scaling was updated in runtime
        /// </summary>
        public virtual void OnUpdateScaling()
        {
        }

        /// <summary>
        /// Calls every frame
        /// </summary>
        public virtual void Update()
        {
        }
    }
}