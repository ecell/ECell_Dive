using Unity.Netcode;
using UnityEngine;
using ECellDive.UI;
using ECellDive.Interfaces;
using TMPro;

namespace ECellDive
{
    namespace Modules
    {
        public class NodeGO : GameNetModule,
                              INodeGO
        {
            #region - INodeGO Members -
            public INode nodeData { get; protected set; }
            public string informationString { get; protected set; }
            #endregion

            protected override void ApplyCurrentColorChange(Color _previous, Color _current)
            {
                mpb.SetVector(colorID, _current);
                m_Renderer.SetPropertyBlock(mpb);
            }

            public void Initialize(GraphScalingData _pathwaySettings, in INode _node)
            {
#if UNITY_EDITOR
                m_Renderer = GetComponent<Renderer>();
                if (nameTextFieldContainer != null)
                {
                    nameField = nameTextFieldContainer.GetComponentInChildren<TextMeshProUGUI>();
                }
                mpb = new MaterialPropertyBlock();
                colorID = Shader.PropertyToID("_Color");
                mpb.SetVector(colorID, defaultColor);
                m_Renderer.SetPropertyBlock(mpb);
#endif
                InstantiateInfoTags(new string[] { "" });
                SetNodeData(_node);
                Vector3 nodePos = new Vector3(nodeData.position.x,
                                              nodeData.position.z,
                                              nodeData.position.y) / _pathwaySettings.positionScaleFactor;
                SetName(nodeData.label);
                HideName();
                gameObject.SetActive(true);
                if (nodeData.isVirtual)
                {
                    m_Renderer.enabled = false;
                }
                gameObject.transform.position = nodePos;
                gameObject.transform.localScale /= _pathwaySettings.sizeScaleFactor;
                gameObject.name = $"{nodeData.ID}";

                m_refInfoTagsContainer.transform.GetChild(0).localScale *= _pathwaySettings.sizeScaleFactor;
                m_nameTextFieldContainer.transform.localScale *= _pathwaySettings.sizeScaleFactor;
                m_nameTextFieldContainer.transform.localPosition = 1.5f*Vector3.up;
            }

            #region - INodeGO Methods -
            public void SetNodeData(INode _INode)
            {
                nodeData = _INode;
                informationString = $"SUID: {nodeData.ID} \n" +
                                    $"name: {nodeData.name} \n" +
                                    $"label: {nodeData.label}";
                m_refInfoTagsContainer.transform.GetChild(0).GetComponent<InfoDisplayManager>().SetText(informationString);
            }
            #endregion

            #region - IHighlightable Methods -
            [ServerRpc(RequireOwnership = false)]
            public override void SetCurrentColorToHighlightServerRpc()
            {
                base.SetCurrentColorToHighlightServerRpc();
                m_Renderer.enabled = true;
            }

            public override void UnsetHighlight()
            {
                if (!forceHighlight)
                {
                    m_Renderer.enabled = true;
                    SetCurrentColorToDefaultServerRpc();

                    if (nodeData.isVirtual)
                    {
                        m_Renderer.enabled = false;
                    }
                }
            }
            #endregion

            #region - INamed Methods -
            /// <inheritdoc/>
            public override void DisplayName()
            {
                if (!nodeData.isVirtual)
                {
                    base.DisplayName();
                }
            }
            #endregion

            #region - MlprVisibility Methods -
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

