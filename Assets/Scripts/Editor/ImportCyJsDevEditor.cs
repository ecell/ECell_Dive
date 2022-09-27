using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ECellDive.CustomEditors
{
    /// <summary>
    /// The custom inspector for <see cref="ImportCyJsDev"/>.
    /// </summary>
    [CustomEditor(typeof(ImportCyJsDev))]
    public class ImportCyJsDevEditor : Editor
    {
        ImportCyJsDev m_importCyJsDev;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            m_importCyJsDev = (ImportCyJsDev)target;

            if (GUILayout.Button("Import Target Model"))
            {
                m_importCyJsDev.ImportModelCyJs();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

