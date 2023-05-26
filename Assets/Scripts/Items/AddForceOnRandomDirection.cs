using UnityEngine;

namespace Items.Spawn
{
    public class AddForceOnRandomDirection : SpawnPostPtocess
    {
        [SerializeField] private float minAngle = -20f;
        [SerializeField] private float maxAngle = 20f;

        [SerializeField] private float minVelocity = 1f;
        [SerializeField] private float maxVelocity = 5f;

        public override void Process(GameObject gameObject)
        {
            var rigidbody = gameObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                float angle = Random.Range(minAngle, maxAngle);
                float velocity = Random.Range(minVelocity, maxVelocity);

                rigidbody.velocity = (Quaternion.Euler(angle, 0, angle) * Vector3.up).normalized * velocity;
            }
        }
    }
}