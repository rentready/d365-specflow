using BoDi;
using FakeItEasy;
using FakeXrmEasy;
using RR.Dynamics365.Model;
using RR.Dynamics365.SpecFlow.Specs.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vermaat.Crm.Specflow;

namespace RR.Dynamics365.SpecFlow.Specs.Hooks
{
    [Binding]
    internal class TestFixture
    {
        private readonly XrmFakedContext _context = new XrmFakedContext();

        [BeforeScenario(Order = 0)]
        public void BeforeScenarioRun(IObjectContainer objectContainer)
        {
            var conn = new FakeCrmConnection("Fake", _context);
            GlobalTestingContext.ConnectionManager.SetAdminConnection(conn);
            GlobalTestingContext.ConnectionManager.SetCurrentConnection(conn);
            objectContainer.RegisterTypeAs<CrmTestingContext, ICrmTestingContext>();
            objectContainer.RegisterInstanceAs<ISeleniumTestingContext>(A.Fake<ISeleniumTestingContext>());

            var assembly = Assembly.GetAssembly(typeof(Contact));
            _context.InitializeMetadata(assembly);
        }
    }
}
