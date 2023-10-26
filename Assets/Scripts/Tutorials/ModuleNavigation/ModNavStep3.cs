using ECellDive.Modules;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 3 of the Tutorial on Modules Navigation.
    /// Import a data module.
    /// </summary>
    public class ModNavStep3 : Step
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

            //We collect the Remote importer module (created at the previous step) to
            //make sure that it is cleaned up when the user quits the tutorial.
            ModNavTutorialManager.tutorialGarbage.Add(httpSIM.gameObject);
        }

        private void ProcessResult(bool _result, string _modelName)
        {
            if (_modelName == "iJO1366")
            {
                moduleImported = _result;
            }
        }
    }
}