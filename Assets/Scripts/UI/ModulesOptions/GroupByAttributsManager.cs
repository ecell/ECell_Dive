using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

            private void AddAttributsUI(string _attributName, DropDownSwitchManager _parent)
            {
                GameObject newAttributUI = Instantiate(attributsUIPrefab, allAttributsContainer.transform);
                newAttributUI.SetActive(false);
                newAttributUI.name = _attributName;
                newAttributUI.GetComponentInChildren<TMP_Text>().text = _attributName;
                _parent.AddItem(newAttributUI.GetComponent<Toggle>());
            }

            public void AddDataUI(string _dataName, JObject _dataNodeSample)
            {
                GameObject newDataUI = Instantiate(dataUIPrefab, allAttributsContainer.transform);
                newDataUI.SetActive(true);
                newDataUI.GetComponentInChildren<TMP_Text>().text = _dataName;
                dataUIManagers.Add(newDataUI.GetComponent<DropDownSwitchManager>());

                DropDownSwitchManager refDDSM = newDataUI.GetComponent<DropDownSwitchManager>();

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
                    foreach(Toggle _attributUI in _ddsm.content)
                    {
                        if (_attributUI.isOn)
                        {
                            refGroupByModule.Execute(currentDataUI, _attributUI.name);
                        }
                    }
                    currentDataUI++;
                }
            }
        }
    }
}

