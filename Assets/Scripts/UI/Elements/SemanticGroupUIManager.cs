using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Utility;


namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// Manages the GUI container (drop down) storing every GUI element representing
        /// groups.
        /// </summary>
        public class SemanticGroupUIManager : MonoBehaviour, IDropDown
        {
            public Toggle refToggle;
            public TMP_Text refNameText;

            public UnityEvent OnDestroy;

            #region - IDropDown Members -
            [SerializeField]
            private GameObject m_refDropDownImageCollapsed;
            public GameObject refDropDownImageCollapsed
            {
                get => m_refDropDownImageCollapsed;
                set => m_refDropDownImageCollapsed = value;
            }
            [SerializeField]
            private GameObject m_refDropDownImageExpanded;
            public GameObject refDropDownImageExpanded
            {
                get => m_refDropDownImageExpanded;
                set => m_refDropDownImageExpanded = value;
            }
            private bool m_isExpanded = false;
            public bool isExpanded
            {
                get => m_isExpanded;
                protected set => m_isExpanded = value;
            }

            private GameObject m_content;
            public GameObject content
            {
                get => m_content;
                set => m_content = value;
            }

            [SerializeField] private GameObject m_scrollListHolderPrefab;
            public GameObject scrollListHolderPrefab
            {
                get => m_scrollListHolderPrefab;
                set => m_scrollListHolderPrefab = value;
            }

            [SerializeField] private GameObject m_scrollListPrefab;
            public GameObject scrollListPrefab
            {
                get => m_scrollListPrefab;
                set => m_scrollListPrefab = value;
            }

            private GameObject m_scrollListHolder;
            public GameObject scrollListHolder
            {
                get => m_scrollListHolder;
                set => m_scrollListHolder = value;
            }

            private OptimizedVertScrollList m_scrollList;
            public OptimizedVertScrollList scrollList
            {
                get => m_scrollList;
                set => m_scrollList = value;
            }
            #endregion

            /// <summary>
            /// The procedure when destroying this semantic group.
            /// </summary>
            public void DestroySelf()
            {
                //Resetting the group info (color) of every child.
                foreach (RectTransform _child in scrollList.refContent)
                {
                    GroupUIManager refGM = _child.gameObject.GetComponent<GroupUIManager>();
                    refGM.ForceDistributeColor(false);
                }

                //Destroying the scroll list of the content of the drop down (semantic group). 
                Destroy(scrollListHolder);

                //Hiding the object.
                gameObject.SetActive(false);

                //Remove the object from the scroll list containing every semantic group.
                transform.parent = null;

                //Invoking external functions.
                OnDestroy?.Invoke();

                //Finally, destroying the object.
                Destroy(gameObject);
            }

            /// <summary>
            /// To be called back when the user wants to expand of collapse the
            /// view of the groups stored in the container.
            /// </summary>
            public void ManageExpansion()
            {
                isExpanded = !isExpanded;
                if (isExpanded)
                {
                    refDropDownImageCollapsed.SetActive(false);
                    refDropDownImageExpanded.SetActive(true);
                    DisplayContent();
                }
                else
                {
                    refDropDownImageExpanded.SetActive(false);
                    refDropDownImageCollapsed.SetActive(true);
                    HideContent();
                }
            }

            /// <summary>
            /// To be called back when the user wants to forcefully activate or deactivate
            /// every group stored in the container.
            /// </summary>
            public void OnToggleValueChange()
            {
                foreach(RectTransform _child in scrollList.refContent)
                {
                    GroupUIManager refGM = _child.gameObject.GetComponent<GroupUIManager>();
                    refGM.ForceDistributeColor(refToggle.isOn);
                    refGM.refToggle.interactable = refToggle.isOn;
                }
            }

            #region - IDropDown Methods -

            public GameObject AddItem()
            {
                return scrollList.AddItem();
            }

            public void DisplayContent()
            {
                m_content.SetActive(true);

                //Repositioning the scrollListHolder in front of the user.
                Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 1.5f, 0.2f);
                m_scrollListHolder.transform.position = pos;
                m_scrollListHolder.GetComponent<ILookAt>().LookAt();
            }

            public void HideContent()
            {
                m_content.SetActive(false);
            }

            public void InstantiateContent()
            {
                m_scrollListHolder = Instantiate(m_scrollListHolderPrefab);
                m_content = m_scrollListHolder.transform.GetChild(0).gameObject;
                //m_content.transform.SetParent(m_scrollListHolder.transform, false);
                //m_scrollListHolder.GetComponent<XRGrabInteractable>().colliders.Add(m_content.GetComponentInChildren<BoxCollider>());
                m_scrollListHolder.GetComponent<XRGrabInteractable>().enabled = true;

                //Positioning the scrollListHolder in front of the user.
                Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 1.5f, 0.2f);
                m_scrollListHolder.transform.position = pos;
                m_scrollListHolder.GetComponent<ILookAt>().LookAt();

                m_scrollList = m_content.GetComponentInChildren<OptimizedVertScrollList>();
            }
            #endregion
        }
    }
}
