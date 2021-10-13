using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace ECellDive
{
    namespace UI
    {
        [CustomEditor(typeof(CylindricLayoutGroup), true)]
        [CanEditMultipleObjects]
        public class CylindricLayoutGroupEditor : HorizontalOrVerticalLayoutGroupEditor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("startRotation"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("radius"), true);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

