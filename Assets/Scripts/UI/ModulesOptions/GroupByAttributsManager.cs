using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ECellDive.Interfaces;
using ECellDive.Modules;


namespace ECellDive
{
    namespace UI
    {
        /// <summary>
        /// Manages the GUI attached to a GroupByModule that the user
        /// interacts with in order to request processing groups.
        /// </summary>
        public class GroupByAttributsManager : MonoBehaviour
        {
            public GroupByModule refGroupByModule;

            public OptimizedVertScrollList allDataUIContainer;
            private List<IDropDown> dataUIManagers = new List<IDropDown>();

            /// <summary>
            /// Adds the GUI element that represent a key (an attribut of an object) the
            /// user may select later to request that groups are made according to it.
            /// </summary>
            /// <param name="_attributName">The displayed name of the field.</param>
            /// <param name="_parent">A reference to the parent drop down container.</param>
            private void AddAttributsUI(string _attributName, IDropDown _parent)
            {
                GameObject newAttributUI = _parent.AddItem(); ;
                newAttributUI.SetActive(true);
                newAttributUI.name = _attributName;
                newAttributUI.GetComponentInChildren<TMP_Text>().text = _attributName;
                _parent.scrollList.UpdateScrollList();

            }

            /// <summary>
            /// Adds the GUI element that acts as a container for every field the user
            /// can use to request groups to be made out from.
            /// </summary>
            /// <param name="_dataName">The displayed name of the container.</param>
            /// <param name="_dataNodeSample">A JObject representing a Json node with 
            /// every field the user will be allowed to used to make groups from.</param>
            public void AddDataUI(string _dataName, JObject _dataNodeSample)
            {
                //Creating the drop down button
                GameObject newDataUI = allDataUIContainer.AddItem();
                newDataUI.SetActive(true);
                newDataUI.GetComponentInChildren<TMP_Text>().text = _dataName;
                IDropDown ddComponent = newDataUI.GetComponent<IDropDown>();

                //Creating the scroll list that will be the container for the objects
                //controlled by the drop down.
                ddComponent.InstantiateContent();
                ddComponent.content.transform.parent = refGroupByModule.transform;
                dataUIManagers.Add(ddComponent);

                foreach (JProperty _property in _dataNodeSample.Properties())
                {
                    AddAttributsUI(_property.Name, ddComponent);
                }
            }

            /// <summary>
            /// Executes the Group By operation on the data for every attributs
            /// the user selected throught the GUI.
            /// </summary>
            public void ScanForRequiredGroups()
            {
                int currentDataUI = 0;
                foreach(SimpleDropDown _ddsm in dataUIManagers)
                {
                    foreach(RectTransform _attributUI in _ddsm.scrollList.refContent)
                    {
                        Toggle attributUIToggle = _attributUI.gameObject.GetComponent<Toggle>();
                        if (attributUIToggle.isOn)
                        {
                            refGroupByModule.Execute(currentDataUI, attributUIToggle.name);
                        }
                    }
                    currentDataUI++;
                }
            }
        }
    }
}

