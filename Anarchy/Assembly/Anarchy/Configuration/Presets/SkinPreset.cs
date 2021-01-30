using System.IO;

namespace Anarchy.Configuration.Presets
{
    public abstract class SkinPreset : ISetting
    {
        protected const string Extension = ".skinpreset";
        public readonly string FullPath;
        public string Name;

        public SkinPreset(string name, string path)
        {
            Name = name;
            FullPath = path + name + Extension;
        }

        public void Delete()
        {
            if (File.Exists(FullPath))
            {
                File.Delete(FullPath);
            }
        }

        public bool Exists()
        {
            return File.Exists(FullPath);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(GetType()))
            {
                return (obj as SkinPreset).Name.Equals(Name);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public abstract void Draw(UI.SmartRect rect, Localization.Locale locale);

        public abstract void Load();

        public abstract void Save();

        public abstract string[] ToSkinData();
    }
}