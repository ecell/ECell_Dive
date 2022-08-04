using Unity.Netcode;
using UnityEngine;
using ECellDive.UI;
using ECellDive.Interfaces;

namespace ECellDive
{
    namespace Modules
    {
        public class NodeGO : GameNetModule,
                              INodeGO
        {
            public INode nodeData { get; protected set; }
            public string informationString { get; protected set; }

            public override void OnNetworkSpawn()
            {
                base.OnNetworkSpawn();
                mpb = new MaterialPropertyBlock();
                colorID = Shader.PropertyToID("_Color");
            }

            protected override void ApplyCurrentColorChange(Color _previous, Color _current)
            {
                mpb.SetVector(colorID, _current);
                m_Renderer.SetPropertyBlock(mpb);
            }

            public void Initialize(CyJsonPathwaySettingsData _pathwaySettings, in INode _node)
            {
                InstantiateInfoTags(new string[] { "" });
                SetNodeData(_node);
                Vector3 nodePos = new Vector3(nodeData.position.x,
                                              nodeData.position.z,
                                              nodeData.position.y) / _pathwaySettings.PositionScaleFactor;
                SetName(nodeData.label);
                HideName();
                gameObject.SetActive(true);
                if (nodeData.isVirtual)
                {
                    m_Renderer.enabled = false;
                }
                gameObject.transform.position = nodePos;
                gameObject.transform.localScale /= _pathwaySettings.SizeScaleFactor;
                gameObject.name = $"{nodeData.ID}";

                m_refInfoTags[0].transform.localScale *= _pathwaySettings.SizeScaleFactor;
                m_nameTextFieldContainer.transform.localScale *= _pathwaySettings.SizeScaleFactor;
                m_nameTextFieldContainer.transform.localPosition = 1.5f*Vector3.up;
            }

            public void SetNodeData(INode _INode)
            {
                nodeData = _INode;
                informationString = $"SUID: {nodeData.ID} \n" +
                                    $"name: {nodeData.name} \n" +
                                    $"label: {nodeData.label}";
                m_refInfoTags[0].GetComponent<InfoDisplayManager>().SetText(informationString);
            }

            #region - IHighlightable -
            [ServerRpc(RequireOwnership = false)]
            public override void SetHighlightServerRpc()
            {
                base.SetHighlightServerRpc();
                m_Renderer.enabled = true;
            }

            [ServerRpc(RequireOwnership = false)]
            public override void UnsetHighlightServerRpc()
            {
                if (!forceHighlight)
                {
                    m_Renderer.enabled = true;
                    currentColor.Value = defaultColor;

                    if (nodeData.isVirtual)
                    {
                        m_Renderer.enabled = false;
                    }
                }
            }
            #endregion

            #region - INamed -
            public override void DisplayName()
            {
                if (!nodeData.isVirtual)
                {
                    base.DisplayName();
                }
            }
            #endregion

            #region - MlprVisibility -
            public override void NetShow()
            {
                m_Collider.enabled = true;

                if (!nodeData.isVirtual)
                {
                    m_Renderer.enabled = true;
                }
            }
            #endregion

        }
    }
}

