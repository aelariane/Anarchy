using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Anarchy.Replays.Recorders
{
    public class ReplayObjectOperationRecorder
    {
        private List<ObjectOperationInformation> objectOperations = new List<ObjectOperationInformation>();
        private ReplayWorld replayWorld;
        private int _nextResourcesId = 1;

        private int NextResourcesId => _nextResourcesId++;

        public List<ObjectOperationInformation> ObjectOperations => objectOperations;

        public ReplayObjectOperationRecorder(ReplayWorld world)
        {
            replayWorld = world;
        }

        public void RecordSpawnOperation(ReplayObjectOperation opType, GameObject go, Vector3 position, Quaternion rotation)
        {
            string resourceName = go.name.Replace("(Clone)", string.Empty).Trim();
            if (replayWorld.Resources.Contains(resourceName) == false)
            {
                replayWorld.Resources.AddPrefab(new KeyValuePair<string, int>(resourceName, NextResourcesId));
            }

            ReplayGameObject res = replayWorld.SpawnObject(go);
            if (opType != ReplayObjectOperation.SpawnEffect)
            {
                res.IsObservableObject = true;
            }

            var info = new ObjectOperationInformation(
                opType,
                new object[] { position, rotation },
                FengGameManagerMKII.FGM.logic.RoundTime
            );

            objectOperations.Add(info);
        }


        public void RecordSetTitanOperation(GameObject go)
        {

        }

        public void RecordSetHeroOperation(GameObject go, ExitGames.Client.Photon.Hashtable hash)
        {
            //TODO
        }

        public void RecordSetHookOperation(GameObject go, GameObject owner, bool isLeft, bool isAhss)
        {
            ReplayGameObject obj = replayWorld.Find(go);

            if (obj != null)
            {
                byte flag = isLeft ? (byte)0b1 : (byte)0b0;
                if (isAhss)
                {
                    flag |= (byte)0b10;
                }

                ReplayGameObject ownerRGO = replayWorld.Find(owner);
                if(owner == null)
                {
                    RecordDestroyOperation(go);
                    return;
                }

                var info = new ObjectOperationInformation(
                    ReplayObjectOperation.SetHook,
                    new object[] { ownerRGO.Id, flag },
                    FengGameManagerMKII.FGM.logic.RoundTime
                );

                objectOperations.Add(info);
            }
        }

        public void RecordDestroyOperation(GameObject go)
        {
            ReplayGameObject obj = replayWorld.Find(go);

            if(obj != null)
            {
                var info = new ObjectOperationInformation(
                    ReplayObjectOperation.DestroyObject,
                    new object[] { obj.Id },
                    FengGameManagerMKII.FGM.logic.RoundTime
                );

                objectOperations.Add(info);
            }
        }
    }
}
