using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ECellDive
{
    namespace UI
    {
        public class DropDownSwitchManager : MonoBehaviour
        {
            public GameObject refDropDownImageCollapsed;
            public GameObject refDropDownImageExpanded;
            public bool isExpanded = false;
            public List<Toggle> content { get; protected set; }

            private void Awake()
            {
                content = new List<Toggle>();
            }

            public void AddItem(Toggle _item)
            {
                content.Add(_item);
            }

            private void DrawContent()
            {
                foreach(Toggle _item in content)
                {
                    _item.gameObject.SetActive(true);
                }
            }

            private void HideContent()
            {
                foreach (Toggle _item in content)
                {
                    _item.gameObject.SetActive(false);
                }
            }

            public void ManageExpansion()
            {
                if (isExpanded)
                {
                    refDropDownImageExpanded.SetActive(false);
                    refDropDownImageCollapsed.SetActive(true);
                    HideContent();
                }
                else
                {
                    refDropDownImageCollapsed.SetActive(false);
                    refDropDownImageExpanded.SetActive(true);
                    DrawContent();
                }
                isExpanded = !isExpanded;
            }
        }
    }
}

