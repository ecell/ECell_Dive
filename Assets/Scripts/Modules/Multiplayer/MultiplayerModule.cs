using UnityEngine;

using ECellDive.UI;
using ECellDive.Utility;

namespace ECellDive.Modules
{
    /// <summary>
    /// The Module to control multiplayer connection.
    /// </summary>
    [RequireComponent(typeof(AnimationLoopWrapper), typeof(ColorFlash))]
    public class MultiplayerModule : Module
    {
        static public MultiplayerModule Instance;

        [SerializeField] private MultiplayerMenuManager multiplayerMenuManager;

        [SerializeField] private AnimationLoopWrapper alw;
        [SerializeField] private ColorFlash colorFlash;

        [SerializeField] private Renderer[] renderers;
        private MaterialPropertyBlock mpb;
        private int colorID;

        private void Start()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            mpb = new MaterialPropertyBlock();
            colorID = Shader.PropertyToID("_Color");
            mpb.SetVector(colorID, defaultColor);
            foreach (Renderer _renderer in renderers)
            {
                _renderer.SetPropertyBlock(mpb);
            }
        }

        public void OnConnectionStart()
        {
            alw.PlayLoop("MultiplayerModule");
        }

        public void OnConnectionFails()
        {
            alw.StopLoop();
            colorFlash.Flash(0);//red fail flash
        }
        public void OnConnectionSuccess()
        {
            alw.StopLoop();
            colorFlash.Flash(1);//Green fail flash
        }

        #region - IHighlightable -
        public override void ApplyColor(Color _color)
        {
            mpb.SetVector(colorID, _color);
            foreach (Renderer _renderer in renderers)
            {
                _renderer.SetPropertyBlock(mpb);
            }
        }

        public override void SetHighlight()
        {
            ApplyColor(highlightColor);
        }

        public override void UnsetHighlight()
        {
            if (!forceHighlight)
            {
                ApplyColor(defaultColor);
            }
        }
        #endregion
    }
}