using UnityEngine;

namespace DVDNights
{
    public class ChalkboardShake : MonoBehaviour
    {
        public float rotationAmount = 0.5f;
        public float speed = 10f;
        public float movementAmount = 0.5f;

        private float _seed;
        Vector3 _originalPosition;

        private void Start()
        {
            _seed = Random.Range(0f, 100f);
            _originalPosition = transform.localPosition;



        }

        private void Update()
        {
            float angle = Mathf.PerlinNoise(Time.time * speed, _seed);
            angle = (angle - 0.5f) * rotationAmount * 2f;

            transform.localRotation = Quaternion.Euler(0, 0, angle);
            
            float x = (Mathf.PerlinNoise(Time.time * speed, _seed) - 0.5f) * movementAmount;
            float y = (Mathf.PerlinNoise(_seed, Time.time * speed) - 0.5f) * movementAmount;
            transform.localPosition = _originalPosition + new Vector3(x, y, 0);
        }
    }
}