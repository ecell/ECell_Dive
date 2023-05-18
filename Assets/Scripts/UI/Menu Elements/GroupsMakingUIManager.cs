using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.SceneManagement;
using ECellDive.Utility;

namespace ECellDive.UI
{
    public class GroupsMakingUIManager : MonoBehaviour
    {
        public GameObject refUICanvas;
        public TMP_InputField refGroupNameInputField;

        public void CloseUI()
        {
            refUICanvas.SetActive(false);
        }

        public void NewGroupUiElement(IHighlightable[] _highlitables)
        {
            //Create a groupUI component
            StaticReferencer.Instance.refGroupsMenu.AddGroupUI(new GroupData
            {
                value = refGroupNameInputField.text,
                color = Random.ColorHSV(),
                members = _highlitables
            },
                0);
        }


        /// <summary>
        /// Manages the visibility of the gameobject <see cref="refUICanvas"/>
        /// containing the canvas of the UI used to validate or cancel the
        /// creation of the group.
        /// </summary>
        public void ManageUIConfirmationCanvas(int _nbMembers)
        {
            if (_nbMembers == 0)
            {
                refUICanvas.SetActive(false);
            }
            else
            {
                refUICanvas.SetActive(true);
                Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 1.5f, 0.2f);
                transform.position = pos;
                GetComponent<ILookAt>().LookAt();
            }
        }

        
    }
}

