using UnityEngine;
using Utilities.Singleton;

namespace Items.Spawn
{
    [CreateAssetMenu(fileName = "ItemSpawnerConfigProvider", menuName = "Items/ItemSpawnerConfigProvider")]
    public class ItemSpawnerConfigProvider : ScriptableObjectSingleton<ItemSpawnerConfigProvider>
    {
        [SerializeField] private ItemSpawnerConfig config = new ItemSpawnerConfig();
        public ItemSpawnerConfig Config { get { return config; } }
    }
}