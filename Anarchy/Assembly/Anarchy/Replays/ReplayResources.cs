using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.Replays
{
    public class ReplayResources
    {
        private List<KeyValuePair<string, int>> resourceMap = new List<KeyValuePair<string, int>>();

        public List<KeyValuePair<string, int>> ResourceMap => resourceMap;

        public int this[string prefabName]
        {
            get
            {
                var pair = resourceMap.Find(x => x.Key == prefabName);
                if(pair.Key == prefabName)
                {
                    return pair.Value;
                }
                throw new InvalidOperationException($"Resource with name {prefabName} does not exists");
            }
        }

        public string this[int prefabId]
        {
            get
            {
                var pair = resourceMap.Find(x => x.Value == prefabId);
                if (pair.Value == prefabId)
                {
                    return pair.Key;
                }
                throw new InvalidOperationException($"Resource with Id {prefabId} does not exists");
            }
        }

        public bool Contains(string prefabName)
        {
            return resourceMap.Where(x => x.Key == prefabName).Count() > 0;
        }

        public bool Contains(int prefabId)
        {
            return resourceMap.Where(x => x.Value == prefabId).Count() > 0;
        }

        public void AddPrefab(KeyValuePair<string, int> pair)
        {
            resourceMap.Add(pair);
        }
    }
}
