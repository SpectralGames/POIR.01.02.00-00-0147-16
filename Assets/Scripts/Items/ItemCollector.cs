using System.Collections;
using UnityEngine;

namespace Items.Spawn
{
    public class ItemCollector : MonoBehaviour
    {
        private static ItemCollector instance = null;
        public static ItemCollector Instance
        {
            get
            {
                if (instance == null)
                {
                    var managerType = typeof(ItemCollector);
                    GameObject gameObject = new GameObject(managerType.Name, managerType);
                    instance = gameObject.GetComponent(managerType) as ItemCollector;
                }
                return instance;
            }
        }

        private PlayerController Player { get { return ObjectPool.Instance.player; } }

        private IEnumerator CollectCorutine(GameObject gameObject, ItemSpawnerConfig cofig)
        {
            Item item = gameObject.GetComponent<Item>();
            yield return new WaitForSeconds(cofig.CollectionDelay);


            item?.PickUp();

            var distance = float.PositiveInfinity;
            while (distance > cofig.CollectDitance)
            {
                var playerPosition = Player.transform.position + cofig.Offset;
                distance = Vector3.Distance(gameObject.transform.position, playerPosition);
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, playerPosition, cofig.CollectSpeed * Time.deltaTime);
                yield return null;
            }

            item?.Collect();
        }

        internal void Collect(GameObject instance, ItemSpawnerConfig cofig)
        {
            StartCoroutine(CollectCorutine(instance, cofig));
        }
    }
}