using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow;

namespace RR.Dynamics365.SpecFlow.Hooks
{
    /// <summary>
    /// Used to handle hooks and redirect them to the Vermaat hooks. We use Vermaat libs to reuse steps implementations and not steps themselves.
    /// To make work Vermaat hooks, we need to refer the assembly in specflow.json. We don't want to.
    /// </summary>
    [Binding]
    public class VermaatHooksAdapter
    {
        private readonly Vermaat.Crm.Specflow.Hooks _vermaatHooks;

        public VermaatHooksAdapter(SeleniumTestingContext seleniumTestingContext, CrmTestingContext crmContext,
                     FeatureContext featureContext, ScenarioContext scenarioContext)

        {
            _vermaatHooks = new Vermaat.Crm.Specflow.Hooks(seleniumTestingContext, crmContext, featureContext, scenarioContext);
        }

        /// <summary>
        /// This is a temporary fix as due to another issue EasyRepro can't be updated right now
        /// </summary>
        [BeforeScenario]
        public void FixXPaths() => _vermaatHooks.FixXPaths();

        [BeforeScenario("API")]
        public void APISetup() => _vermaatHooks.APISetup();

        [BeforeScenario("Chrome")]
        public void ChromeSetup() => _vermaatHooks.ChromeSetup();

        [BeforeScenario("Edge")]
        public void EdgeSetup() => _vermaatHooks.EdgeSetup();

        [BeforeScenario("Firefox")]
        public void FirefoxSetup() => _vermaatHooks.FirefoxSetup();

        [BeforeScenario("IE")]
        public void IESetup() => _vermaatHooks.IESetup();

        [BeforeScenario("ForceAPI")]
        public void ForceApi() => _vermaatHooks.ForceApi();

        [BeforeScenario("ForceBrowser")]
        public void ForceBrowser() => _vermaatHooks.ForceBrowser();

        [BeforeScenario("PreferAPI")]
        public void PreferApi() => _vermaatHooks.PreferApi();

        [BeforeScenario("PreferBrowser")]
        public void PreferBrowser() => _vermaatHooks.PreferBrowser();


        [BeforeScenario]
        public void SetDefaultConnection() => _vermaatHooks.SetDefaultConnection();

        [AfterScenario("Cleanup")]
        public void Cleanup() => _vermaatHooks.Cleanup();


        [AfterScenario]
        public void AfterWebTest() => _vermaatHooks.AfterWebTest();

        [BeforeTestRun]
        public static void BeforeTestRunXPathFixes() => Vermaat.Crm.Specflow.Hooks.BeforeTestRunXPathFixes();

        [AfterTestRun]
        public static void AfterTestRunCleanup() => Vermaat.Crm.Specflow.Hooks.AfterTestRunCleanup();
    }
}
