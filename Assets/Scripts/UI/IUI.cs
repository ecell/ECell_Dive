using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ECellDive
{
    namespace Interfaces
    {
        public interface IDropDown
        {
            GameObject refDropDownImageCollapsed { get; }
            GameObject refDropDownImageExpanded { get; }
            bool isExpanded {get;}
            List<GameObject> content { get; }

            public void AddItem(GameObject _item);
            public void DrawContent();
            public void HideContent();
        }
    }
}

