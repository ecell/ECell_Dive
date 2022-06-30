using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ECellDive.Multiplayer;

namespace ECellDive.CustomEditors
{
    [CustomEditor(typeof(ClientGameNetPortal))]
    public class ClientGameNetPortalEditor : Editor
    {
        string buttonDisabledReasonSuffix;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ClientGameNetPortal clientGameNetPortal = (ClientGameNetPortal)target;

            if (!EditorApplication.isPlaying)
            {
                buttonDisabledReasonSuffix = ". This can only be done in play mode.";
                GUI.enabled = false;
            }

            if (GUILayout.Button(
                new GUIContent("Start Client",
                                "Starts a client instance" + buttonDisabledReasonSuffix)))
            {
                clientGameNetPortal.StartClient();
            }

            if (!EditorApplication.isPlaying)
            {
                GUI.enabled = true;
            }
        }
    }
}