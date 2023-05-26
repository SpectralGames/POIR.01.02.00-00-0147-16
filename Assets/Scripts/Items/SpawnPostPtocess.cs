using UnityEngine;

namespace Items.Spawn
{
    public abstract class SpawnPostPtocess : MonoBehaviour
    {
        public abstract void Process(GameObject gameObject);
    }
}