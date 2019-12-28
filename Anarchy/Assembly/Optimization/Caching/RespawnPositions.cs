using UnityEngine;

namespace Optimization.Caching
{
    internal static class RespawnPositions
    {
        private static Vector3[] heropos;
        private static Vector3[] titanpos;

        public static void Dispose()
        {
            heropos = null;
            titanpos = null;
        }

        public static Vector3 RandomHeroPos
        {
            get
            {
                if (heropos != null)
                {
                    return heropos[UnityEngine.Random.Range(0, heropos.Length)];
                }
                GameObject[] gos = GameObject.FindGameObjectsWithTag("playerRespawn");
                if (gos != null)
                {
                    heropos = new Vector3[gos.Length];
                    for (int i = 0; i < gos.Length; i++)
                    {
                        heropos[i] = gos[i].transform.position;
                    }
                }
                if (heropos != null)
                {
                    return heropos[UnityEngine.Random.Range(0, heropos.Length)];
                }
                return Vectors.zero;
            }
        }

        public static Vector3 RandomTitanPos
        {
            get
            {
                if (titanpos != null)
                {
                    return titanpos[UnityEngine.Random.Range(0, titanpos.Length)];
                }
                GameObject[] gos = GameObject.FindGameObjectsWithTag("titanRespawn");
                if (gos != null)
                {
                    titanpos = new Vector3[gos.Length];
                    for (int i = 0; i < gos.Length; i++)
                    {

                        titanpos[i] = gos[i].transform.position;
                    }
                }
                if (titanpos != null)
                {
                    return titanpos[UnityEngine.Random.Range(0, titanpos.Length)];
                }
                return Vectors.zero;
            }
        }

        public static Vector3[] TitanPositions
        {
            get
            {
                if (titanpos != null)
                {
                    return titanpos;
                }
                GameObject[] gos = GameObject.FindGameObjectsWithTag("titanRespawn");
                if (gos != null)
                {
                    titanpos = new Vector3[gos.Length];
                    for (int i = 0; i < gos.Length; i++)
                    {
                        titanpos[i] = gos[i].transform.position;
                    }
                    return titanpos;
                }
                return new Vector3[0];
            }
        }
    }
}
