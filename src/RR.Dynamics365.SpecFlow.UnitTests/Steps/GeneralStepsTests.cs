using BoDi;
using FakeItEasy;
using FakeXrmEasy;
using RR.Dynamics365.SpecFlow.Helpers;
using RR.Dynamics365.SpecFlow.Steps;
using RR.Dynamics365.SpecFlow.UnitTests.Fixtures;
using RR.Dynamics365.Model;
using System.Reflection;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;
using Vermaat.Crm.Specflow;

namespace RR.Dynamics365.SpecFlow.UnitTests.Steps
{
    public class GeneralStepsTests
    {
        private readonly XrmFakedContext _context = new XrmFakedContext();
        private readonly IObjectContainer _container;

        public GeneralStepsTests()
        {
            var conn = new FakeCrmConnection("Fake", _context);
            GlobalTestingContext.ConnectionManager.SetAdminConnection(conn);
            GlobalTestingContext.ConnectionManager.SetCurrentConnection(conn);
            _container = new ObjectContainer();
            _container.RegisterInstanceAs<ScenarioContext>(CreateScenarioContext());
            _container.RegisterTypeAs<CrmTestingContext, ICrmTestingContext>();
            _container.RegisterInstanceAs<ISeleniumTestingContext>(A.Fake<ISeleniumTestingContext>());

            var assembly = Assembly.GetAssembly(typeof(Contact));
            _context.InitializeMetadata(assembly);
        }

        [Fact]
        public void Should_create_account()
        {
            // Arrange
            _container.RegisterTypeAs<GeneralSteps, GeneralSteps>();
            var generalSteps = _container.Resolve<GeneralSteps>();
            var table = new Table("Property", "Value");
            table.AddRow("name", "Awersome Account");

            // Act
            generalSteps.GivenEntityWithValues("account", "Account", table);

            // Assert
            var accounts = (from a in _context.CreateQuery<Account>()
                            where a.Name == "Awersome Account"
                            select a).ToList();
            
            Assert.Single(accounts);
        }

        private static ScenarioContext CreateScenarioContext()
        {
#pragma warning disable CS8603 // Possible null reference return.
            return Activator.CreateInstance(typeof(ScenarioContext), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] {
                A.Fake<IObjectContainer>(),
                new ScenarioInfo("Test", "Test", new string[0], null),
                A.Fake<ITestObjectResolver>()}, null) as ScenarioContext;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
