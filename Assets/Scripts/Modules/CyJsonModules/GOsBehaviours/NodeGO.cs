using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using ECellDive.Utility.SettingsModels;
using ECellDive.UI;
using ECellDive.IInteractions;
using ECellDive.INetworkComponents;

namespace ECellDive
{
    namespace Modules
    {
        public class NodeGO : Module,
                              INodeGO
        {
            public INode nodeData { get; protected set; }
            public string informationString { get; protected set; }

            private Renderer refRenderer;
            private Material refSharedMaterial;

            private void Start()
            {

                refRenderer = GetComponent<Renderer>();
                refSharedMaterial = refRenderer.sharedMaterial;
            }

            public void Initialize(NetworkGO _masterPathway, in INode _node)
            {
                InstantiateInfoTags(new string[] { "" });
                SetNodeData(_node);
                Vector3 nodePos = new Vector3(nodeData.position.x,
                                              nodeData.position.z,
                                              nodeData.position.y) / _masterPathway.networkGOSettingsModel.PositionScaleFactor;
                gameObject.SetActive(true);
                gameObject.transform.position = nodePos;
                gameObject.transform.localScale /= _masterPathway.networkGOSettingsModel.SizeScaleFactor;
                gameObject.name = $"{nodeData.ID}";

                m_refInfoTags[0].transform.localScale *= _masterPathway.networkGOSettingsModel.SizeScaleFactor;
            }

            public void SetNodeData(INode _INode)
            {
                nodeData = _INode;
                informationString = $"SUID: {nodeData.ID} \n" +
                                    $"name: {nodeData.NAME}";
                m_refInfoTags[0].GetComponent<InfoDisplayManager>().SetText(informationString);
            }

            #region - IHighlightable -
            public override void SetHighlight()
            {
                refRenderer.material.SetFloat("Vector1_66D21324", 1);
            }

            public override void UnsetHighlight()
            {
                refRenderer.sharedMaterial = refSharedMaterial;
            }
            #endregion

        }
    }
}

