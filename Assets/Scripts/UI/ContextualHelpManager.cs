using UnityEngine;

namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// Class to control the visibility of all contextual info tags
        /// that could be attached to children gameobjects.
        /// </summary>
        public class ContextualHelpManager : MonoBehaviour
        {
            public GameObject leftControllerModel;
            public GameObject rightControllerModel;

            private InfoTagManager[] leftInfoTagManagers;
            private InfoTagManager[] rightInfoTagManagers;

            private void Awake()
            {
                leftInfoTagManagers = leftControllerModel.GetComponentsInChildren<InfoTagManager>();
                rightInfoTagManagers = rightControllerModel.GetComponentsInChildren<InfoTagManager>();
            }

            public void BroadcastControlModeSwitchToLeftController(int _controlModeID)
            {
                foreach (InfoTagManager _infoTag in leftInfoTagManagers)
                {
                    _infoTag.SwitchControlMode(_controlModeID);
                }
            }
            public void BroadcastControlModeSwitchToRightController(int _controlModeID)
            {
                foreach (InfoTagManager _infoTag in rightInfoTagManagers)
                {
                    _infoTag.SwitchControlMode(_controlModeID);
                }
            }

            public void ContextualHelpGlobalHide()
            {
                foreach (InfoTagManager _infoTag in leftInfoTagManagers)
                {
                    _infoTag.GlobalHide();
                }
                foreach (InfoTagManager _infoTag in rightInfoTagManagers)
                {
                    _infoTag.GlobalHide();
                }
            }
        }
    }
}

