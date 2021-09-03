using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ECellDive.IO;
using ECellDive.Modules;
using ECellDive.Utility;
using ECellDive.NetworkComponents;

namespace ECellDive
{
    namespace SceneManagement
    {
        /// <summary>
        /// Instantiates the content of the active module in the Diving room.
        /// </summary>
        public class DiveRoomManager : MonoBehaviour
        {
            public GameObject refXRRig;
            public GameObject DiveContainer;
            public Utility.SettingsModels.NetworkComponentsReferences networkComponents;
            [HideInInspector] public GameObject LoadedNetwork;

            public Animator refDivingAnimator;
            public InputActionReference refBackToMainRoom;


            private void Awake()
            {
                refBackToMainRoom.action.performed += BackToMainRoom;
            }

            private void BackToMainRoom(InputAction.CallbackContext ctx)
            {
                refDivingAnimator.SetTrigger("DiveStart");
                StartCoroutine(Loading.SwitchScene(0, 1.5f));
            }

            private void OnEnable()
            {
                refBackToMainRoom.action.Enable();
            }

            private void OnDisable()
            {
                refBackToMainRoom.action.Disable();
            }

            private void OnDestroy()
            {
                refBackToMainRoom.action.performed -= BackToMainRoom;
            }

            void Start()
            {
                if (ModulesData.typeActiveModule == ModulesData.ModuleType.NetworkModule &&
                    NetworkModulesData.activeData != null)
                {
                    //Instantiate the loaded network in the scene based on
                    //the information retained in the data structures.
                    LoadedNetwork = NetworkLoader.Generate(NetworkModulesData.activeData,
                                                           networkComponents.networkGO,
                                                           networkComponents.layerGO,
                                                           networkComponents.nodeGO,
                                                           networkComponents.edgeGO);
                    LoadedNetwork.transform.parent = DiveContainer.transform;
                    refXRRig.transform.position = Positioning.GetGravityCenter(LoadedNetwork.GetComponent<NetworkGO>().NodeID_to_NodeGO.Values);
                }
            }

            public void ManageEdgesHighlight(NodeGO _refNodeGO)
            {
                NetworkGO refNetworkGO = LoadedNetwork.GetComponent<NetworkGO>();

                foreach (int _edgeID in _refNodeGO.nodeData.outgoingEdges)
                {
                    GameObject edgeGO = refNetworkGO.EdgeID_to_EdgeGO[_edgeID];
                    edgeGO.GetComponent<EdgeGO>().ManageHighlight();
                }
            }
        }
    }
}

