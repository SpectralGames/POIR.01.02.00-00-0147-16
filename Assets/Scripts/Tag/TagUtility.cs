using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FortacoQE.ProcedureEditor.Utilities
{
    [Serializable]
    public class TagUtility
    {
        [SerializeField] private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public static implicit operator string (TagUtility tag)
        {
            return tag.Name;
        }
    }
}