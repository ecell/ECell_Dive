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

            private Renderer refRenderer;
            private MaterialPropertyBlock mpb;
            private int colorID;

            private void OnEnable()
            {
                refRenderer = GetComponentInChildren<Renderer>();
                mpb = new MaterialPropertyBlock();
                colorID = Shader.PropertyToID("_Color");
                mpb.SetVector(colorID, defaultColor.Value);
                refRenderer.SetPropertyBlock(mpb);
            }

            public void Initialize(CyJsonPathwaySettingsData _pathwaySettings, in INode _node)
            {
                InstantiateInfoTags(new string[] { "" });
                SetNodeData(_node);
                Vector3 nodePos = new Vector3(nodeData.position.x,
                                              nodeData.position.z,
                                              nodeData.position.y) / _pathwaySettings.PositionScaleFactor;
                gameObject.SetActive(true);
                if (nodeData.isVirtual)
                {
                    refRenderer.enabled = false;
                }
                gameObject.transform.position = nodePos;
                gameObject.transform.localScale /= _pathwaySettings.SizeScaleFactor;
                gameObject.name = $"{nodeData.ID}";

                m_refInfoTags[0].transform.localScale *= _pathwaySettings.SizeScaleFactor;
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
                refRenderer.enabled = true;
                mpb.SetVector(colorID, highlightColor);
                refRenderer.SetPropertyBlock(mpb);
            }

            public override void UnsetHighlight()
            {
                if (!forceHighlight)
                {
                    refRenderer.enabled = true;
                    mpb.SetVector(colorID, defaultColor.Value);
                    refRenderer.SetPropertyBlock(mpb);

                    if (nodeData.isVirtual)
                    {
                        refRenderer.enabled = false;
                    }
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

