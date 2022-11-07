using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow;
using Vermaat.Crm.Specflow.Commands;
using Vermaat.Crm.Specflow.EasyRepro;
using Constants = Vermaat.Crm.Specflow.Constants;

namespace RR.Dynamics365.SpecFlow.Steps
{
    [Binding]
    public class UISteps
    {

        private readonly CrmTestingContext _crmContext;
        private readonly SeleniumTestingContext _seleniumContext;


        public UISteps(SeleniumTestingContext seleniumContext, CrmTestingContext crmContext)
        {
            _crmContext = crmContext;
            _seleniumContext = seleniumContext;
        }

        [When(@"I click button ""(.*)""")]
        public void WhenIClickButton(string buttonName)
        {
            _seleniumContext.GetBrowser().App.Client.ClickCommand(buttonName);
        }

        [Then("I expect (.*)'s form has the following form state")]
        public void ThenFieldsAreVisibleOnForm(string alias, Table table)
        {
            var aliasRef = _crmContext.RecordCache[alias];
            _crmContext.TableConverter.ConvertTable(aliasRef.LogicalName, table);

            _crmContext.CommandProcessor.Execute(new AssertFormStateCommand(_crmContext, _seleniumContext, aliasRef, table));

        }

        [Then(@"I expect (.*) has the following form notifications")]
        public void ThenFormNotificationExist(string alias, Table formNotifications)
        {
            _crmContext.CommandProcessor.Execute(new AssertFormNotificationsCommand(_crmContext, _seleniumContext, alias, formNotifications));
        }

        [Then(@"I expect the following form notifications are on the current form")]
        public void ThenCurrentFormNotificationExist(Table formNotifications)
        {
            _crmContext.CommandProcessor.Execute(new AssertFormNotificationsCommand(_crmContext, _seleniumContext, null, formNotifications));
        }


        [Then(@"I expect the following localized form notifications are on the current form")]
        public void ThenCurrentLocalizedFormNotificationExist(Table formNotifications)
        {
            _crmContext.TableConverter.LocalizeColumn(formNotifications, Constants.SpecFlow.TABLE_FORMNOTIFICATION_MESSAGE, GlobalTestingContext.ConnectionManager.CurrentConnection.UserSettings.UILanguage);
            _crmContext.CommandProcessor.Execute(new AssertFormNotificationsCommand(_crmContext, _seleniumContext, null, formNotifications));
        }

        [Then(@"I expect the following error message appears: '(.*)'")]
        public void ThenErrorAppears(string errorMessage)
        {
            //_crmContext.CommandProcessor.Execute(new AssertErrorDialogCommand(_crmContext, _seleniumContext, errorMessage));
        }

        [Then("I expect (.*)'s form has the following ribbon state")]
        public void ThenFormHasRibbonItems(string alias, Table table)
        {
            _crmContext.CommandProcessor.Execute(new AssertRibbonStateCommand(_crmContext, _seleniumContext, alias, table));
        }
    }
}
