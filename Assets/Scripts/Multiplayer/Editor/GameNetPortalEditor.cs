using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ECellDive.Multiplayer;

namespace ECellDive.CustomEditors
{
    [CustomEditor(typeof(GameNetPortal))]
    public class GameNetPortalEditor : Editor
    {
        string buttonDisabledReasonSuffix;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GameNetPortal gameNetPortal = (GameNetPortal)target;

            if (!EditorApplication.isPlaying)
            {
                buttonDisabledReasonSuffix = ". This can only be done in play mode.";
                GUI.enabled = false;
            }

            if (GUILayout.Button(
                new GUIContent("Start Host",
                                "Starts a host instance" + buttonDisabledReasonSuffix)))
            {
                gameNetPortal.StartHost();
            }

            if (!EditorApplication.isPlaying)
            {
                GUI.enabled = true;
            }
        }
    }
}