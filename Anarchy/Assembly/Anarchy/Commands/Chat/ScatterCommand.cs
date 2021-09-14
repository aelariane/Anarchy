using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Anarchy.Commands.Chat
{
    public class ScatterCommand : ChatCommand
    {
        public ScatterCommand() : base("scatter", true)
        {

        }

        public override bool Execute(string[] args)
        {
            var points = new List<Vector3>(Optimization.Caching.RespawnPositions.TitanPositions);

            foreach(var titan in FengGameManagerMKII.Titans)
            {
                int index = UnityEngine.Random.Range(0, points.Count);
                titan.baseT.position = points[index];
                points.RemoveAt(index);

                if(points.Count <= 0)
                {
                    points = new List<Vector3>(Optimization.Caching.RespawnPositions.TitanPositions);
                }
            }

            return true;
        }
    }
}
