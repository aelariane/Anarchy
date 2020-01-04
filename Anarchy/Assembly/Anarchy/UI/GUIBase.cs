using Anarchy.UI.Animation;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Anarchy.UI
{
    public abstract class GUIBase
    {
        private static int maxLayer = 0;
        private static readonly List<int> usedLayers = new List<int>();

        protected Animation.Animation animator;
        public readonly string Directory;
        public readonly int Layer;
        protected readonly Localization.Locale locale;
        public readonly string Name;
        public Action OnGUI = () => { };
        private readonly Dictionary<string, Texture2D> textureCache;

        private static Texture2D cachedEmptyTexture;

        internal static List<GUIBase> AllBases { get; private set; }
        internal static Texture2D EmptyTexture
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

        public bool Active { get; private set; } = false;

        public GUIBase(string name) : this(name, -1) { }

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
            if(AllBases == null)
            {
                AllBases = new List<GUIBase>();
            }
            AllBases.Add(this);
        }

        ~GUIBase()
        {
            lock (usedLayers)
            {
                int indexToRemove = -1;
                for(int i = 0; i < usedLayers.Count; i++)
                {
                    if(usedLayers[i] == Layer)
                    {
                        indexToRemove = i;
                        break;
                    }
                }
                if(indexToRemove >= 0)
                    usedLayers.RemoveAt(indexToRemove);
                UpdateMaxLayer();
            }
            if (AllBases.Contains(this))
            {
                AllBases.Remove(this);
            }
        }

        public void ClearCache()
        {
            textureCache.Clear();
        }

        public void Disable()
        {
            if (!Active)
                return;
            OnGUI = animator.StartClose(() =>
            {
                DisableImmediate();
            });
        }

        public void DisableImmediate()
        {
            if (UIManager.Disable(this))
            {
                OnDisable();
                locale.Unload();
                Active = false;
                OnGUI = null;
            }
        }

        protected internal abstract void Draw();

        public void Enable()
        {
            if (Active)
                return;
            if (UIManager.Enable(this))
            {
                locale.Load();
                OnEnable();
                OnGUI = animator.StartOpen(() => { });
                Active = true;
            }
        }

        public void EnableImmediate()
        {
            if (UIManager.Enable(this))
            {
                locale.Load();
                OnEnable();
                Active = true;
                OnGUI = Draw;
            }
        }

        public void EnableNext(GUIBase next)
        {
            if (!Active)
                return;
            OnGUI = animator.StartClose(() =>
            {
                if (UIManager.Disable(this))
                {
                    OnDisable();
                    locale.Unload();
                    Active = false;
                    next.Enable();
                }
            });
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
                usedLayers.Add(++maxLayer);
                return maxLayer;
            }
        }

        public static Texture2D LoadTexture(GUIBase @base, string name, string ext = "png")
        {
            return @base.LoadTexture(name, ext);
        }

        internal MovieTexture LoadVideo(string namebase, string ext)
        {
            MovieTexture tex;
            string name = namebase;
            bool error = false;
            if (ext == string.Empty)
            {
                if (!name.EndsWith(".ogv"))
                    error = true;
            }
            else
            {
                if (!ext.Equals("ogv"))
                    error = true;
                name += "." + ext;
            }
            if (error)
            {
                Debug.LogError($"You should use ogv extensions for loading MovieTexture");
                return null;
            }
            string path = Directory + name;
            Debug.Log(path);
            WWW www = new WWW("file://" + path);
            if (!File.Exists(path))
            {
                Debug.LogError($"File what you are trying to load doesnt't exist: \"{path}\"");
                return null;
            }
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
                if(!name.EndsWith(".png") && !name.EndsWith(".jpg") && !name.EndsWith(".jpeg"))
                    error = true;
            }
            else
            {
                if (!ext.Equals("png") && !ext.Equals("jpg") && !ext.Equals("jpeg"))
                    error = true;
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

        protected virtual void OnDisable() { }

        protected virtual void OnEnable() { }

        public virtual void OnUpdateScaling() { }

        public virtual void Update() { }

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
    }
}
