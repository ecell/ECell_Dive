using UnityEngine;
using ECellDive.UI;
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

            public Color defaultColor;
            public Color highlightColor;

            private Renderer refRenderer;
            private MaterialPropertyBlock mpb;
            //private int highlightFloatID;
            private int colorID;
            //private Material refSharedMaterial;

            private void Start()
            {

                
                
                //refSharedMaterial = refRenderer.sharedMaterial;
            }

            private void OnEnable()
            {
                refRenderer = GetComponentInChildren<Renderer>();
                mpb = new MaterialPropertyBlock();
                //highlightFloatID = Shader.PropertyToID("Vector1_66D21324");
                colorID = Shader.PropertyToID("_Color");
                mpb.SetVector(colorID, defaultColor);
                refRenderer.SetPropertyBlock(mpb);
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
                //refRenderer.material.SetFloat("Vector1_66D21324", 1);
                //mpb.SetFloat(highlightFloatID, 1);
                mpb.SetVector(colorID, highlightColor);
                refRenderer.SetPropertyBlock(mpb);

            }

            public override void UnsetHighlight()
            {
                //refRenderer.sharedMaterial = refSharedMaterial;
                //mpb.SetFloat(highlightFloatID, 0);
                mpb.SetVector(colorID, defaultColor);
                refRenderer.SetPropertyBlock(mpb);
            }
            #endregion

        }
    }
}

