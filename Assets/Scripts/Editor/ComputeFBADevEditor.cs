using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ECellDive.CustomEditors
{
    [CustomEditor(typeof(ComputeFBADev))]
    public class ComputeFBADevEditor : Editor
    {
        ComputeFBADev m_ComputeFBADev;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            m_ComputeFBADev = (ComputeFBADev)target;

            if (GUILayout.Button("Compute FBA"))
            {
                m_ComputeFBADev.InitializeFbaAnalysisData();
                m_ComputeFBADev.RequestModelSolve();
            }
        }
    }
}

