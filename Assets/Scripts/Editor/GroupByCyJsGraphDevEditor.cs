using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ECellDive.CustomEditors
{
    [CustomEditor(typeof(GroupByCyJsGraphDev))]
    public class GroupByCyJsGraphDevEditor : Editor
    {
        GroupByCyJsGraphDev groupByCyJsGraphDev;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            groupByCyJsGraphDev = (GroupByCyJsGraphDev)target;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Group Edges"))
            {
                groupByCyJsGraphDev.CreateGroupEdges();
            }

            if (GUILayout.Button("Create Group Nodes"))
            {
                groupByCyJsGraphDev.CreateGroupNodes();
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reuse Defined Edge Groups"))
            {
                groupByCyJsGraphDev.ReuseDefinedEdgeGroups();
            }

            if (GUILayout.Button("Reuse Defined Nodes Groups"))
            {
                groupByCyJsGraphDev.ReuseDefinedNodesGroups();
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reapply Edge Colors"))
            {
                groupByCyJsGraphDev.ApplyGroupColors(groupByCyJsGraphDev.edgesGroups);
            }

            if (GUILayout.Button("Reapply Nodes Colors"))
            {
                groupByCyJsGraphDev.ApplyGroupColors(groupByCyJsGraphDev.nodesGroups);
            }
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}

