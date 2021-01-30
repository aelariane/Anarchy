using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Anarchy.Replays
{
    public class ReplayWorld
    {
        private int nextIdentifier;

        public ReplayResources Resources { get; } = new ReplayResources();
        public List<ReplayGameObject> ActiveObjects { get; } = new List<ReplayGameObject>();

        /// <summary>
        /// Gets next availible ObjectId
        /// </summary>
        public int NextIdentifier
        {
            get
            {
                return nextIdentifier++; 
            }
        }

        public ReplayWorld()
        {
        }

        public ReplayGameObject SpawnObject(GameObject go)
        {

            int newObjectId = NextIdentifier;

            var result = new ReplayGameObject(newObjectId, go);

            ActiveObjects.Add(result);

            return result;
        }

        public ReplayGameObject Find(int objectId)
        {
            ReplayGameObject result = ActiveObjects.FirstOrDefault(x => x.Id == objectId);
            return result;
        }

        public ReplayGameObject Find(GameObject go)
        {
            ReplayGameObject result = ActiveObjects.FirstOrDefault(x => x.SourceObject == go);
            return result;
        }

        public bool DestroyObject(GameObject go)
        {
            var resObj = ActiveObjects.FirstOrDefault(x => x.SourceObject == go);

            if(resObj != null)
            {
                ActiveObjects.Remove(resObj);
                return true;
            }

            return false;
        }
    }
}
