using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ECellDive.Interfaces;


namespace ECellDive
{
    namespace UI
    {
        public struct GroupData
        {
            public string value;
            public Color color;
            public IHighlightable[] members;
        }

        /// <summary>
        /// Manages the GUI element exposing a Group data (name, color & activation)
        /// in the scene to the user.
        /// </summary>
        public class GroupManager : MonoBehaviour
        {
            public Toggle refToggle;
            public TMP_Text refValueText;
            public Button refButtonColorPicker;

            private GroupData groupData;

            /// <summary>
            /// Internal method to update the color of the GameObjects in the group.
            /// </summary>
            /// <param name="_color"></param>
            private void DistributeColorToMembers(Color _color)
            {
                foreach(IHighlightable _member in groupData.members)
                {
                    _member.SetDefaultColor(_color);
                    _member.UnsetHighlight();
                }
            }

            /// <summary>
            /// Public method intended to be called back whenever the user wants to
            /// activate or deactivate the visualization of the group.
            /// </summary>
            public void ManageMembersVisual()
            {
                if (refToggle.isOn)
                {
                    groupData.color = refButtonColorPicker.colors.normalColor;
                }
                else
                {
                    groupData.color = Color.white;
                }
                DistributeColorToMembers(groupData.color);
            }

            /// <summary>
            /// Public method to be used by external objects that globally controls the
            /// activation or deactivation of the visualization of the group.
            /// </summary>
            /// <param name="_forceValue">If True, the visuals are forcefully activated; If 
            /// False, the visuals are forcefully deactivated.</param>
            public void ForceDistributeColor(bool _forceValue)
            {
                if (_forceValue)
                {
                    DistributeColorToMembers(groupData.color);
                }
                else
                {
                    DistributeColorToMembers(Color.white);
                }
            }

            /// <summary>
            /// Sets the data about the group represented by the GUI this script is attached to.
            /// </summary>
            /// <param name="_data"></param>
            public void SetData(GroupData _data)
            {
                groupData = _data;
                refValueText.text = groupData.value;

                ColorBlock colors = refButtonColorPicker.colors;
                colors.normalColor = groupData.color;
                colors.highlightedColor = groupData.color;
                colors.pressedColor = groupData.color;
                refButtonColorPicker.colors = colors;
            }
        }
    }
}

