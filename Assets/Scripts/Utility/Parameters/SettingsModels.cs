using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ECellDive
{
    namespace Utility
    {
        namespace SettingsModels
        {
            #region - Data -
            [System.Serializable]
            public class Data
            {
                [Header("Cystoscape Data")]
                public string DefaultPath;
                //public string DefaultName;
            }
            #endregion

            #region - UI -
            [System.Serializable]
            public class InformationPanels
            {
                public GameObject FixedInformationPanel;
                public GameObject FloatingInformationPanel;
            }
            #endregion

            #region - Modules -
            [System.Serializable]
            public class ModulesTypesReferences
            {
                [Header("Prefabs References")]
                public GameObject networkModule;
            }
            #endregion

            #region - Network -

            #region - NetworkComponents -
            [System.Serializable]
            public class NetworkComponentsReferences
            {
                [Header("Prefabs References")]
                public GameObject networkGO;
                public GameObject layerGO;
                public GameObject nodeGO;
                public GameObject edgeGO;
            }
            #endregion

            #region - NetworkGO - 
            [System.Serializable]
            public class NetworkGOSettings
            {
                [Header("Scaling")]
                [Min(1)] public float PositionScaleFactor;
                [Min(1)] public float SizeScaleFactor;
                [Min(0)] public float InterLayersDistance;
            }
            #endregion

            #region - EdgeGO - 
            [System.Serializable]
            public class EdgeGOSettings
            {
                [Header("Line Default Width Modificators")]
                [Range(0, 1)] public float startWidthFactor;
                [Range(0, 1)] public float endWidthFactor;

                [Header("Line Highlight Width Modificators")]
                [Range(0, 1)] public float startHWidthFactor;
                [Range(0, 1)] public float endHWidthFactor;
            }
            #endregion

            #region - NodeGO - 
            [System.Serializable]
            public class NodeGOSettings
            {

            }
            #endregion

            #endregion
        }
    }
}
