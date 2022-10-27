using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace RR.Dynamics365.SpecFlow.Steps
{
    [Binding]
    public class GenericSteps
    {
        private readonly ScenarioContext _scenarioContext;
        public GenericSteps(ScenarioContext scenarioContext) => _scenarioContext = scenarioContext;

        [Given(@"I have an existing '([^\\s]+)' with primary name '([^\\s]+)'")]
        public void GivenExistingEntity(string entityLogicalName, string primaryName)
        {

        }
    }
}
