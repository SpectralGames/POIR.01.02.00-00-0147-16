using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Items.Spawn
{
    public class ItemSpawner : MonoBehaviour
    {
        public ItemSpawnerConfig Cofig { get { return ItemSpawnerConfigProvider.Instance.Config; } }

        [SerializeField] private GameObject objectToSpawn = null;
        [SerializeField] private int count = 5;
        [SerializeField] private int countMax = 5;

        [SerializeField] private float radius = 5f;

        [SerializeField] private SpawnPostPtocess[] spawnPostPtocesses = null;

        public void Spawn()
        {
            int itemsTospawn = Random.Range(count, countMax);
            if (objectToSpawn != null)
            {
                for (int i = 0; i < itemsTospawn; i++)
                {
                    Vector3 position = transform.position + (Random.onUnitSphere * radius);
                    position.y = position.y < transform.position.y ? transform.position.y : position.y;
                    GameObject instance = GameObject.Instantiate(objectToSpawn, transform.position, Quaternion.identity);
                    foreach (var postprocess in spawnPostPtocesses)
                        postprocess.Process(instance);

                    if (Cofig.AutoCollect)
                        ItemCollector.Instance.Collect(instance, Cofig);
                }
            }
        }

        private void Awake()
        {
            spawnPostPtocesses = GetComponents<SpawnPostPtocess>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}