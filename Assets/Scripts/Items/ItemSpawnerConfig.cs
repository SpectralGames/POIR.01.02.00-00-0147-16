using System;
using UnityEngine;

namespace Items.Spawn
{
    [Serializable]
    public class ItemSpawnerConfig
    {
        [SerializeField] private bool autoCollect = true;
        public bool AutoCollect { get { return autoCollect; } }

        [SerializeField] private float collectSpeed = 5f;
        public float CollectSpeed { get { return collectSpeed; } }

        [SerializeField] private float collectionDelay = 3f;
        public float CollectionDelay { get { return collectionDelay; } }

        [SerializeField] private float collectDitance = .1f;
        public float CollectDitance { get { return collectDitance; } }

        [SerializeField] private Vector3 offset = Vector3.zero;
        public Vector3 Offset { get { return offset; } }
    }
}