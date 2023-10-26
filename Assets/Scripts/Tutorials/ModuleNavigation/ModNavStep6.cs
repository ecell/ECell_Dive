using ECellDive.Modules;

namespace ECellDive.Tutorials
{
    /// <summary>
    /// The step 6 of the Tutorial on Modules Navigation.
    /// Suggest to KO reactions and re-simulate to see the
    /// changes.
    /// </summary>
    public class ModNavStep6 : Step
    {
        private HttpServerFbaModule httpFbaM;
        private bool moduleImported;

        public override bool CheckCondition()
        {
            return moduleImported;
        }

        public override void Conclude()
        {
            base.Conclude();

            httpFbaM.OnFbaResultsReceive -= ProcessResult;
        }

        public override void Initialize()
        {
            base.Initialize();

            httpFbaM = FindObjectOfType<HttpServerFbaModule>();
            httpFbaM.OnFbaResultsReceive += ProcessResult;

            //We collect the FBA module (created at the previous step) to
            //make sure that it is cleaned up when the user quits the tutorial.
            ModNavTutorialManager.tutorialGarbage.Add(httpFbaM.gameObject);
        }

        private void ProcessResult(bool _result)
        {
            moduleImported = _result;
        }
    }
}