namespace DataDisplay.Scripts
{
    using UnityEngine;
    public class RandomMovement : MonoBehaviour
    {
        public Quaternion moveTo;

        public float waitTime = 5f, smooth = 5f;
        private float time = 0f;

        // Start is called before the first frame update
        private void Start()
        {
            moveTo = RandomQ();
        }

        // Update is called once per frame
        private void Update()
        {
            //decide random quaternion

            //lerp towards point

            transform.rotation = Quaternion.Lerp(transform.rotation, moveTo, smooth * Time.deltaTime);
            time += Time.deltaTime;

            if (time > waitTime)
            {
                moveTo = RandomQ();
                time = 0;
            }


            // if time > wait time {

            //create another random quaternion

            // }

        }

        private Quaternion RandomQ()
        {

            var x = Random.Range(-1f, 1f);
            var y = Random.Range(-1f, 1f);
            var z = Random.Range(-1f, 1f);
            var w = Random.Range(-1f, 1f);

            var quaternion = new Quaternion(x, y, z, w);

            return quaternion;
        }
    }
}
