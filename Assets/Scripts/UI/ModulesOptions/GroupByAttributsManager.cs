using System.Collections;
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
        public class GroupByAttributsManager : MonoBehaviour
        {
            public GroupByModule refGroupByModule;

            public GameObject dataUIPrefab;
            public GameObject attributsUIPrefab;
            public GameObject allAttributsContainer;

            private List<DropDownSwitchManager> dataUIManagers = new List<DropDownSwitchManager>();

            private void AddAttributsUI(string _attributName, IDropDown _parent)
            {
                GameObject newAttributUI = Instantiate(attributsUIPrefab, allAttributsContainer.transform);
                newAttributUI.SetActive(false);
                newAttributUI.name = _attributName;
                newAttributUI.GetComponentInChildren<TMP_Text>().text = _attributName;
                _parent.AddItem(newAttributUI);
            }

            public void AddDataUI(string _dataName, JObject _dataNodeSample)
            {
                GameObject newDataUI = Instantiate(dataUIPrefab, allAttributsContainer.transform);
                newDataUI.SetActive(true);
                newDataUI.GetComponentInChildren<TMP_Text>().text = _dataName;
                
                DropDownSwitchManager refDDSM = newDataUI.GetComponent<DropDownSwitchManager>();
                dataUIManagers.Add(refDDSM);

                foreach (JProperty _property in _dataNodeSample.Properties())
                {
                    AddAttributsUI(_property.Name, refDDSM);
                }
            }

            public void ScanForRequiredGroups()
            {
                int currentDataUI = 0;
                foreach(DropDownSwitchManager _ddsm in dataUIManagers)
                {
                    foreach(GameObject _attributUI in _ddsm.content)
                    {
                        Toggle attributUIToggle = _attributUI.GetComponent<Toggle>();
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

