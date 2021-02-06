using UnityEngine;

namespace Anarchy.Custom.Scripts
{
    //Huge help for helping to fix issues to Sadico! (https://github.com/Mi-Sad)
    public class RacingMovingObject : MonoBehaviour
    {
        private static int nextId;
        private Vector3 endPosition;
        private Vector3 startPosition;
        private bool isLaunched = false;


        float percentage_position = 0;
        float time_moltiplier = 0;

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
            if (isLaunched == false)
            {
                return;
            }

            this.percentage_position = this.get_clipped(this.percentage_position + (Time.deltaTime * this.time_moltiplier));
            //we use the f. lerp which makes a weight mean of the val. using our % as weight (Explanation by Sadico)
            CachedTransform.position = Vector3.Lerp(startPosition, endPosition, this.for_lerp(this.percentage_position));
        }


        //NExt 2 functions added by Sadico
        //return the value in a back and form way
        public float for_lerp(float value)
        {
            if (value > 1)
                return 2 - value;
            return value;
        }

        //clip the value between 0 and 2 so that it's a go and back animation
        public float get_clipped(float x)
        {
            if (x > 2)
                return get_clipped(x - 2);
            return x;
        }

        public static void ResetNextId()
        {
            nextId = 1;
        }

        public void Launch()
        {
            isLaunched = true;

            //Explanation by Sadico
            //we execute the Distance function just one to initialize the moltiplier and don't need to do the calculation anymore
            //instead we do a more simple math calculation, get_perc at each frame that just cap and cycle a counter
            time_moltiplier = this.Speed / Vector3.Distance(this.startPosition, this.endPosition);
            percentage_position = 0;
        }

        public void SetPosition(Vector3 startPoint, Vector3 destinationPosition)
        {
            startPosition = startPoint;
            endPosition = destinationPosition;
        }
    }
}
