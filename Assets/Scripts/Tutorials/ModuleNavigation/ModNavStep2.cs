using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ECellDive.Modules;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 2 of the Tutorial on Modules Navigation.
    /// Import a data module.
    /// </summary>
    public class ModNavStep2 : Step
    {
        private HttpServerImporterModule httpSIM;
        private bool moduleImported;

        public override bool CheckCondition()
        {
            return moduleImported;
        }

        public override void Conclude()
        {
            base.Conclude();

            httpSIM.OnDataModuleImport -= ProcessResult;
        }

        public override void Initialize()
        {
            base.Initialize();

            httpSIM = FindObjectOfType<HttpServerImporterModule>();
            httpSIM.OnDataModuleImport += ProcessResult;
        }

        private void ProcessResult(bool _result)
        {
            moduleImported = _result;
        }
    }
}

