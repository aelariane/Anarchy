using UnityEngine;

namespace Cache
{

    internal static class RespawnPositions
    {
        private static Vector3[] _heroPositions;
        private static Vector3[] _titanPositions;

        public static void Dispose()
        {
            _heroPositions = null;
            _titanPositions = null;
        }

        public static Vector3 RandomHeroPosition
        {
            get
            {
                if (_heroPositions != null)
                {
                    return _heroPositions[Random.Range(0, _heroPositions.Length)];
                }
                GameObject[] gos = GameObject.FindGameObjectsWithTag("playerRespawn");
                if (gos != null)
                {
                    _heroPositions = new Vector3[gos.Length];
                    for (int i = 0; i < gos.Length; i++)
                    {
                        _heroPositions[i] = gos[i].transform.position;
                    }
                }
                if (_heroPositions != null)
                {
                    return _heroPositions[Random.Range(0, _heroPositions.Length)];
                }
                return Vectors.Zero;
            }
        }

        public static Vector3 RandomTitanPosition
        {
            get
            {
                if (_titanPositions != null)
                {
                    return _titanPositions[Random.Range(0, _titanPositions.Length)];
                }
                GameObject[] gos = GameObject.FindGameObjectsWithTag("titanRespawn");
                if (gos != null)
                {
                    _titanPositions = new Vector3[gos.Length];
                    for (int i = 0; i < gos.Length; i++)
                    {
                        _titanPositions[i] = gos[i].transform.position;
                    }
                }
                if (_titanPositions != null)
                {
                    return _titanPositions[Random.Range(0, _titanPositions.Length)];
                }
                return Vectors.Zero;
            }
        }

        public static Vector3[] TitanPositions
        {
            get
            {
                if (_titanPositions != null)
                {
                    return _titanPositions;
                }
                GameObject[] gos = GameObject.FindGameObjectsWithTag("titanRespawn");
                if (gos != null)
                {
                    _titanPositions = new Vector3[gos.Length];
                    for (int i = 0; i < gos.Length; i++)
                    {
                        _titanPositions[i] = gos[i].transform.position;
                    }
                    return _titanPositions;
                }
                return new Vector3[0];
            }
        }
    }
}
