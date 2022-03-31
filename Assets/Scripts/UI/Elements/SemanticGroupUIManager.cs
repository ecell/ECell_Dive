using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
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

            //private List<GameObject> m_content = new List<GameObject> ();
            //public List<GameObject> content
            //{
            //    get => m_content;
            //    protected set => m_content = value;
            //}
            private GameObject m_content;
            public GameObject content
            {
                get => m_content;
                set => m_content = value;
            }
            
            //[SerializeField] private GameObject m_itemPrefab;
            //public GameObject itemPrefab
            //{
            //    get => m_itemPrefab;
            //    set => m_itemPrefab = value;
            //}
            
            [SerializeField] private GameObject m_scrollListPrefab;
            public GameObject scrollListPrefab
            {
                get => m_scrollListPrefab;
                set => m_scrollListPrefab = value;
            }
            
            private OptimizedVertScrollList m_scrollList;
            public OptimizedVertScrollList scrollList
            {
                get => m_scrollList;
                set => m_scrollList = value;
            }
            #endregion

            public void DestroySelf()
            {
                foreach (RectTransform _child in scrollList.refContent)
                {
                    GroupUIManager refGM = _child.gameObject.GetComponent<GroupUIManager>();
                    refGM.ForceDistributeColor(false);
                }
                Destroy(content);
                gameObject.SetActive(false);
                transform.parent = null;
                OnDestroy?.Invoke();
                Destroy(gameObject);
            }

            /// <summary>
            /// To be called back when the user wants to expand of collapse the
            /// view of the groups stored in the container.
            /// </summary>
            public void ManageExpansion()
            {
                isExpanded = !isExpanded;
                Debug.Log("Managing Expansion of " + gameObject.name + $" with isExpanded={isExpanded}");
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

            public void SwitchExpansionStatus()
            {
                Debug.Log("Switching Extension status of "+gameObject.name);
                isExpanded = !isExpanded;
            }

            #region - IDropDown Methods -

            public GameObject AddItem()
            {
                return scrollList.AddItem();
            }

            public void DisplayContent()
            {
                //foreach (RectTransform _item in content.refContent)
                //{
                //    _item.gameObject.SetActive(true);
                //}
                m_content.SetActive(true);
                Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 1.5f, 0.8f);
                m_content.transform.position = pos;
                m_content.GetComponent<FaceCamera>().ShowBackToPlayer();
            }

            public void HideContent()
            {
                //foreach (RectTransform _item in m_content.refContent)
                //{
                //    _item.gameObject.SetActive(false);
                //}
                m_content.SetActive(false);
            }

            public void InstantiateContent()
            {
                m_content = Instantiate(m_scrollListPrefab);
                Vector3 pos = Positioning.PlaceInFrontOfTarget(Camera.main.transform, 1.5f, 0.8f);
                m_content.transform.position = pos;
                m_content.GetComponent<FaceCamera>().ShowBackToPlayer();
                m_scrollList = m_content.GetComponentInChildren<OptimizedVertScrollList>();
            }
            #endregion
        }
    }
}
