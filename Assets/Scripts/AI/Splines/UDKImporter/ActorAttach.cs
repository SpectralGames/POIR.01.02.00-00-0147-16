using UnityEngine;
using System.Collections;

public class ActorAttach : MonoBehaviour
{
    [System.Serializable]
    public class AttachTo
    {
        public string baseClassName;
        public string baseActorName;
        public string boneName;
        public Vector3 offset;
        public Quaternion rotation;
    }

    public AttachTo attachTo;
}
