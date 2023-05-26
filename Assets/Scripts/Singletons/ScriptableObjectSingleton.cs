using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Singleton
{
    public class ScriptableObjectSingleton<SingletonType> : ScriptableObject where SingletonType : ScriptableObject
    {
        private const string _path = "Singletons/{0}";
        private static SingletonType _instance = null;
        public static SingletonType Instance
        {
            get
            {
                if (_instance == null)
                    _instance = (SingletonType)Resources.Load(string.Format(_path, typeof(SingletonType).Name));

                return _instance;
            }
        }
    }
}