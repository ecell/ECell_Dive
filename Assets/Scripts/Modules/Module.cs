using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using ECellDive.Utility;
using ECellDive.UI;
using ECellDive.SceneManagement;
using ECellDive.IInteractions;

namespace ECellDive
{
    namespace Modules
    {
        /// <summary>
        /// Base class holding references and methods used to manipulate
        /// the game object representation of a module.
        /// </summary>
        public class Module : MonoBehaviour,
                                IFocus,
                                IHighlightable,
                                IInfoTags
        {
            [Header("Module Info")]
            public TextMeshProUGUI refName;

            public ControllersSymetricAction diveActions;

            //[Tooltip("The interaction layer used to respond to the dive input")]
            //public LayerMask refInteractorTargetLayer;

            public bool finalLayer = false;

            #region - IFocus Members -
            private bool m_isFocused = false;
            public bool isFocused
            {
                get => m_isFocused;
                set => isFocused = m_isFocused;
            }

            #endregion

            #region - IInfo Tags Members -
            public bool areVisible { get; set; }

            [Header("Info Tags Data")]
            public ControllersSymetricAction m_displayInfoTagsActions;
            public ControllersSymetricAction displayInfoTagsActions
            {
                get => m_displayInfoTagsActions;
                set => displayInfoTagsActions = m_displayInfoTagsActions;
            }
            public GameObject m_refInfoTagPrefab;
            public GameObject refInfoTagPrefab
            {
                get => m_refInfoTagPrefab;
                set => refInfoTagPrefab = m_refInfoTagPrefab;
            }
            public GameObject m_refInfoTagsContainer;
            public GameObject refInfoTagsContainer
            {
                get => m_refInfoTagsContainer;
                set => refInfoTagsContainer = m_refInfoTagsContainer;
            }

            public List<GameObject> m_refInfoTags;
            public List<GameObject> refInfoTags
            {
                get => m_refInfoTags;
                set => refInfoTags = m_refInfoTags;
            }
            #endregion

            private void Awake()
            {
                areVisible = false;

                diveActions.leftController.action.performed += DiveIn;
                diveActions.rightController.action.performed += DiveIn;

                m_displayInfoTagsActions.leftController.action.performed += ManageInfoTagsDisplay;
                m_displayInfoTagsActions.rightController.action.performed += ManageInfoTagsDisplay;
            }

            private void OnDestroy()
            {
                diveActions.leftController.action.performed -= DiveIn;
                diveActions.rightController.action.performed -= DiveIn;

                m_displayInfoTagsActions.leftController.action.performed -= ManageInfoTagsDisplay;
                m_displayInfoTagsActions.rightController.action.performed -= ManageInfoTagsDisplay;
            }

            //private void OnEnable()
            //{
            //    diveActions.leftController.action.Enable();
            //    diveActions.rightController.action.Enable();

            //    m_displayInfoTagsActions.leftController.action.Enable();
            //    m_displayInfoTagsActions.rightController.action.Enable();
            //}

            //private void OnDisable()
            //{
            //    diveActions.leftController.action.Disable();
            //    diveActions.rightController.action.Disable();

            //    m_displayInfoTagsActions.leftController.action.Disable();
            //    m_displayInfoTagsActions.rightController.action.Disable();
            //}

            /// <summary>
            /// Base method to dive in a module.
            /// </summary>
            private void DiveIn(InputAction.CallbackContext _ctx)
            {
                StartCoroutine(DiveInC());
            }

            protected virtual IEnumerator DiveInC()
            {
                yield return null;
            }

            /// <summary>
            /// Input call back action on which the floating display
            /// is turned on or off.
            /// </summary>
            /// <param name="_ctx">The input action callback context</param>
            private void ManageInfoTagsDisplay(InputAction.CallbackContext _ctx)
            {
                if (isFocused)
                {
                    areVisible = !areVisible;
                    if (areVisible)
                    {
                        DisplayInfoTags();
                    }
                    else
                    {
                        HideInfoTags();
                    }
                }
            }

            public void SetName(string _name)
            {
                refName.text = _name;
            }

            /// <summary>
            /// Makes sure the name of the module's name faces the
            /// Player's POV and is therefore readable.
            /// </summary>
            public void ShowNameToPlayer()
            {
                Positioning.UIFaceTarget(refName.gameObject.transform.parent.gameObject, Camera.main.transform);
            }

            #region - IFocus Methods -
            public void SetFocus()
            {
                m_isFocused = true;
            }

            public void UnsetFocus()
            {
                m_isFocused = false;
            }
            #endregion

            #region - IHighlightable -
            public virtual void SetHighlight()
            {
            }

            public virtual void UnsetHighlight()
            {
            }
            #endregion

            #region - IInfoTags Methods-
            public void DisplayInfoTags()
            {
                foreach (GameObject _infoTag in refInfoTags)
                {
                    _infoTag.SetActive(true);
                }
            }

            public void HideInfoTags()
            {
                foreach (GameObject _infoTag in refInfoTags)
                {
                    _infoTag.SetActive(false);
                }
            }

            public void InstantiateInfoTag(Vector2 _xyPosition, string _content)
            {
                GameObject infoTag = Instantiate(refInfoTagPrefab, refInfoTagsContainer.transform);
                infoTag.transform.localPosition = new Vector3(_xyPosition.x, _xyPosition.y, 0f);
                infoTag.GetComponent<InfoDisplayManager>().SetText(_content);
                refInfoTags.Add(infoTag);
            }

            public void InstantiateInfoTags(string[] _content)
            {
                float angle = 360 / _content.Length;
                float radius = 1.25f * Mathf.Max(new float[]{transform.localScale.x,
                                                             transform.localScale.y,
                                                             transform.localScale.z });

                for (int i = 0; i < _content.Length; i++)
                {
                    Vector2 xyPosition = Positioning.RadialPosition(radius, i * angle);
                    InstantiateInfoTag(xyPosition, _content[i]);
                }
            }

            public void ShowInfoTags()
            {
                foreach (GameObject _infoTag in refInfoTags)
                {
                    _infoTag.GetComponent<InfoDisplayManager>().ShowInfoToPlayer();
                }
            }
            #endregion
            
        }
    }
}
