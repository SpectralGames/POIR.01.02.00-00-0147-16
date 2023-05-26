using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Items
{
    [Serializable] public class OnItemEvent : UnityEvent<GameObject> { } 

    public class Item : MonoBehaviour
    {
        public OnItemEvent OnDrop = new OnItemEvent();
        public OnItemEvent OnPickUp = new OnItemEvent();
        public OnItemEvent OnCollect = new OnItemEvent();

        public virtual void Drop()
        {
            OnDrop.Invoke(gameObject);
        }

        public virtual void PickUp()
        {
            OnPickUp.Invoke(gameObject);
        }

        public virtual void Collect()
        {
            OnCollect.Invoke(gameObject);
        }
    }
}
