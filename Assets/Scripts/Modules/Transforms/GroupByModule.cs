using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECellDive
{
    namespace Modules
    {
        public class GroupByModule : Module
        {
            private Renderer refRenderer;
            private MaterialPropertyBlock mpb;
            private int colorID;

            private void Awake()
            {
                NetworkGO _refNetworkGO = FindObjectOfType<NetworkGO>();
                Debug.Log(_refNetworkGO);
            }

            private void OnEnable()
            {
                refRenderer = GetComponentInChildren<Renderer>();
                mpb = new MaterialPropertyBlock();
                colorID = Shader.PropertyToID("_Color");
                mpb.SetVector(colorID, defaultColor);
                refRenderer.SetPropertyBlock(mpb);
            }

            #region - IHighlightable -
            public override void SetHighlight()
            {
                mpb.SetVector(colorID, highlightColor);
                refRenderer.SetPropertyBlock(mpb);
            }

            public override void UnsetHighlight()
            {
                mpb.SetVector(colorID, defaultColor);
                refRenderer.SetPropertyBlock(mpb);
            }
            #endregion
        }
    }
}

