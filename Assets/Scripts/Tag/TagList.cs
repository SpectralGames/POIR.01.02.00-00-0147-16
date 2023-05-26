using System;
using UnityEngine;

namespace FortacoQE.ProcedureEditor.Utilities
{
    [Serializable]
    public class TagList
    {
        [SerializeField] private TagUtility[] tags = null;

        public bool IsTagOnList(GameObject gameObject)
        {
            return IsTagOnList(gameObject.tag);
        }

        private bool IsTagOnList(string tag)
        {
            if (tags.Length == 0)
                return true;

            foreach (var item in tags)
                if (tag == item)
                    return true;

            return false;
        }
    }
}