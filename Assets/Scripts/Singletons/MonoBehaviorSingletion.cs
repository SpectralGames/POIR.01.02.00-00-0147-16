using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Singleton
{
    public class MonoBehaviorSingletion<SingletonType> : MonoBehaviour where SingletonType : MonoBehaviorSingletion<SingletonType>
    {
        public static SingletonType Instance = null;

        protected virtual void Awake()
        {
            if (Instance == null)
                Instance = this as SingletonType;
            else
                Destroy(this.gameObject);
        }

        protected virtual void Reset()
        {
            gameObject.name = typeof(SingletonType).Name;
        }
    }
}