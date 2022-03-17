using System.Collections;
using System.Collections.Generic;
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

        public class GroupManager : MonoBehaviour
        {
            public Toggle refToggle;
            public TMP_Text refValueText;
            public Button refButtonColorPicker;

            private GroupData groupData;

            private void DistributeColorToMembers(Color _color)
            {
                foreach(IHighlightable _member in groupData.members)
                {
                    //Debug.Log(_member);
                    _member.SetDefaultColor(_color);
                    _member.UnsetHighlight();
                }
            }

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

