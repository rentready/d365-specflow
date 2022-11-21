using BoDi;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using RR.Dynamics365.SpecFlow.Helpers;
using System.Drawing;
using Vermaat.Crm.Specflow;

namespace RR.Dynamics365.SpecFlow.Hooks
{
    [Binding]
    public class Setup
    {
        private readonly IObjectContainer _objectContainer;
        private static readonly IObjectContainer _globalContainer;

        static Setup()
        {
            _globalContainer = new ObjectContainer();
        }

        public Setup(IObjectContainer objectContainer, Vermaat.Crm.Specflow.CrmTestingContext context)
        {
            _objectContainer = objectContainer;
            _objectContainer.RegisterInstanceAs<ICrmTestingContext>(context);
        }

        [BeforeScenario]
        public void BeforeScenario(IObjectContainer objectContainer)
        {
            _objectContainer.RegisterInstanceAs<IObjectConverter>(_globalContainer.Resolve<IObjectConverter>());
            _objectContainer.RegisterInstanceAs<IQueryWithConditionOperatorsHelper>(_globalContainer.Resolve<IQueryWithConditionOperatorsHelper>());
        }

        [BeforeTestRun]
        public static void InitializeGlobalInstances()
        {
            _globalContainer.RegisterTypeAs<Helpers.ObjectConverter, IObjectConverter>();
            _globalContainer.RegisterTypeAs<QueryWithConditionOperatorsHelper, IQueryWithConditionOperatorsHelper>();
        }
    }
}
