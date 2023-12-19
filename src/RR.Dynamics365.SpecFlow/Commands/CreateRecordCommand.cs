using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow.EasyRepro;
using Vermaat.Crm.Specflow.Commands;
using Vermaat.Crm.Specflow;
using OpenQA.Selenium;
using RR.Dynamics365.SpecFlow.Helpers;

namespace RR.Dynamics365.SpecFlow.Commands
{
    public class CreateRecordCommand : BrowserCommandFunc<EntityReference>
    {
        private readonly string _entityLogicalName;
        private readonly Table _criteria;
        private readonly string _alias;

        public event EventHandler<EventArgs>? OnFormFillStart;

        public CreateRecordCommand(ICrmTestingContext crmContext, ISeleniumTestingContext seleniumContext,
            string entityLogicalName, Table criteria, string alias)
            : base(crmContext, seleniumContext)
        {
            _entityLogicalName = entityLogicalName;
            _criteria = criteria;
            _alias = alias;
        }

        protected override EntityReference ExecuteApi()
        {
            Entity toCreate = _crmContext.RecordBuilder.SetupEntityWithDefaults(_entityLogicalName, _criteria);
            GlobalTestingContext.ConnectionManager.CurrentConnection.Create(toCreate, _alias, _crmContext.RecordCache);
            return toCreate.ToEntityReference();
        }

        protected override EntityReference ExecuteBrowser()
        {
            var formData = RetryPolicies.ElementNotInteractableOnOpenRecord.Execute(() => _seleniumContext.GetBrowser().OpenRecord(new OpenFormOptions(_entityLogicalName)));

            var tableWithDefaults = _crmContext.RecordBuilder.AddDefaultsToTable(_entityLogicalName, _criteria);

            OnFormFillStart?.Invoke(this, EventArgs.Empty);

            formData.FillForm(_crmContext, tableWithDefaults);

            try
            {
                formData.Save(true);
            }
            catch (TestExecutionException ex)
            {
                if (ex.ErrorCode == Constants.ErrorCodes.FORM_SAVE_FAILED)
                {
                    Console.WriteLine(ex.Message);
                    HelperMethods.WaitForFormLoad(_seleniumContext.GetBrowser().App.WebDriver);
                    _seleniumContext.GetBrowser().App.Client.Browser.ThinkTime(10000);
                    OnFormFillStart?.Invoke(this, EventArgs.Empty);
                    formData.FillForm(_crmContext, tableWithDefaults);
                    formData.Save(true);
                }
            }

            var record = new EntityReference(_entityLogicalName, formData.GetRecordId());
            _crmContext.RecordCache.Add(_alias, record);
            return record;
        }
    }
}
