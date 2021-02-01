using UnityEngine;

namespace Anarchy.CustomLevelScripts
{
    public class RacingMovingObject : MonoBehaviour
    {
        private static int nextId;
        private Vector3 endPosition;
        private Vector3 startPosition;
        private bool toEnd = true;
        private bool isLaunched = false;

        public Transform CachedTransform { get; private set; }
        public int ID { get; private set; }
        public float Speed { get; set; } = 10f;
        public bool IsLaunched => isLaunched;

        private void Awake()
        {
            ID = nextId++;
            CachedTransform = GetComponent<Transform>();
        }

        private void Update()
        {
            if(isLaunched == false)
            {
                return;
            }
            if (toEnd)
            {
                CachedTransform.position = Vector3.MoveTowards(CachedTransform.position, endPosition, Speed * Time.deltaTime);
                if (Vector3.Distance(CachedTransform.position, endPosition) < 2f)
                {
                    this.toEnd = false;
                }
            }
            else
            {
                CachedTransform.position = Vector3.MoveTowards(CachedTransform.position, startPosition, Speed * Time.deltaTime);
                if (Vector3.Distance(CachedTransform.position, startPosition) < 2f)
                {
                    this.toEnd = true;
                }
            }
        }

        public static void ResetNextId()
        {
            nextId = 1;
        }

        public void Launch()
        {
            isLaunched = true;
            CachedTransform.position = startPosition;
            toEnd = true;
        }

        public void SetPosition(Vector3 startPoint, Vector3 destinationPosition)
        {
            startPosition = startPoint;
            endPosition = destinationPosition;
        }
    }
}