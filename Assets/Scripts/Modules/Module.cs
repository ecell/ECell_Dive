using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using ECellDive.Utility;
using ECellDive.UI;

namespace ECellDive
{
    namespace Modules
    {
        /// <summary>
        /// Base class holding references and methods used to manipulate
        /// the game object representation of a module.
        /// </summary>
        [RequireComponent(typeof(XRBaseInteractable))]
        public class Module : MonoBehaviour
        {
            [Header("Global References")]
            public ModulesManager refModuleManager;

            [Header("Module Info References")]
            public TextMeshProUGUI refName;
            public GameObject refInfoTagPrefab;
            public GameObject refInfoTagsContainer;
            public List<GameObject> refInfoTags;
            public InputActionReference refShowInfoAction;

            [Header("Diving System")]
            [Tooltip("The gameobject with the diving animator")]
            public Animator refDivingAnimator;

            [Tooltip("The minimum time we wait for the dive.")]
            [Min(1f)] public float divingTime;

            [Tooltip("Reference to the Action used to dive")]
            public InputActionReference refDiveAction;

            [Tooltip("The interaction layer used to respond to the dive input")]
            public LayerMask refInteractorTargetLayer;

            [Tooltip("Reference to the interactable attached to this object")]
            public XRBaseInteractable refInteractable;

            [Tooltip("Boolean reporting whether an interactor matching the" +
                "appropriate interaction layer is pointing at the module.")]
            public bool isFocused = false;

            //private bool isDivingTimePassed = false;

            private void Awake()
            {
                refShowInfoAction.action.performed += e => ShowInfoTags();
                refDiveAction.action.performed += e => DiveIn();
            }

            protected virtual void DiveIn()
            {
                if (isFocused)
                {
                    refDivingAnimator.SetTrigger("DiveStart");
                    refModuleManager.RegisterAllModulesPositions();
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
                    Vector2 xyPosition = Positioning.RadialPosition(radius, i*angle);
                    InstantiateInfoTag(xyPosition, _content[i]);
                }
            }

            private void OnEnable()
            {
                refDiveAction.action.Enable();
            }

            private void OnDisable()
            {
                refDiveAction.action.Disable();
            }

            public void SetFocus(bool _focused)
            {
                isFocused = _focused;
            }

            public void SetName(string _name)
            {
                refName.text = _name;
            }

            public void ShowInfoTags()
            {
                if (isFocused)
                {
                    foreach (GameObject _infoTag in refInfoTags)
                    {
                        _infoTag.SetActive(!_infoTag.activeSelf);
                    }
                }
            }

            public void ShowInfoTagsToPlayer()
            {
                foreach(GameObject _infoTag in refInfoTags)
                {
                    _infoTag.GetComponent<InfoDisplayManager>().ShowInfoToPlayer();
                }
            }

            public void ShowNameToPlayer()
            {
                Positioning.UIFaceTarget(refName.gameObject.transform.parent.gameObject, Camera.main.transform);
            }

            //private IEnumerator WaitForDivingEnter()
            //{
            //    yield return new WaitForSeconds(divingTime);
            //    isDivingTimePassed = true;
            //    Debug.Log("After WaitForDivingEnter");
            //}
        }
    }
}
