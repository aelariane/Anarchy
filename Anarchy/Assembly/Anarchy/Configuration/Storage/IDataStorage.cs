using System;

namespace Anarchy.Configuration.Storage
{
    public interface IDataStorage : IDisposable
    {
        void Clear();

        bool GetBool(string key, bool def);

        float GetFloat(string key, float def);

        int GetInt(string key, int def);

        string GetString(string key, string def);

        void Save();

        void SetBool(string key, bool value);

        void SetFloat(string key, float value);

        void SetInt(string key, int value);

        void SetString(string key, string value);
    }
}