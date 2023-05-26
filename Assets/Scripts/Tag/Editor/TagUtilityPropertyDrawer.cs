using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FortacoQE.ProcedureEditor.Utilities
{
    [CustomPropertyDrawer(typeof(TagUtility))]
    public class TagUtilityPropertyDrawer : PropertyDrawer
    {
        private List<string> list;
        private SerializedProperty name;

        private int index = -1;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            list = new List<string>(UnityEditorInternal.InternalEditorUtility.tags);
            name = property.FindPropertyRelative("name");
            if ((index = list.IndexOf(name.stringValue)) < 0)
                index = 0;
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            index = EditorGUI.Popup(position, property.displayName, index, list.ToArray());
            name.stringValue = list[index];
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}