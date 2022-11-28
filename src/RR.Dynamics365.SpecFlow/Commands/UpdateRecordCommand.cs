using Microsoft.Xrm.Sdk;
using System.Linq;
using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow.EasyRepro;
using Vermaat.Crm.Specflow.Commands;
using Vermaat.Crm.Specflow;
using RR.Dynamics365.SpecFlow.Helpers;
using ObjectConverter = Vermaat.Crm.Specflow.ObjectConverter;

namespace RR.Dynamics365.SpecFlow.Commands
{
    public class UpdateRecordCommand : BrowserCommand
    {
        private static readonly string[] _apiOnlyEntities = new string[] { "usersettings" };

        private readonly EntityReference _toUpdate;
        private readonly Table _criteria;

        public UpdateRecordCommand(ICrmTestingContext crmContext, ISeleniumTestingContext seleniumContext, EntityReference toUpdate,
            Table criteria)
            : base(crmContext, seleniumContext)
        {
            _toUpdate = toUpdate;
            _criteria = criteria;
        }

        protected override void ExecuteApi()
        {
            Entity toUpdate = new Entity(_toUpdate.LogicalName)
            {
                Id = _toUpdate.Id
            };

            foreach (TableRow row in _criteria.Rows)
            {
                toUpdate[row[Constants.SpecFlow.TABLE_KEY]] = ObjectConverter.ToCrmObject(_toUpdate.LogicalName,
                    row[Constants.SpecFlow.TABLE_KEY], row[Constants.SpecFlow.TABLE_VALUE], _crmContext);
            }

            GlobalTestingContext.ConnectionManager.CurrentConnection.Update(toUpdate);
        }

        protected override void ExecuteBrowser()
        {
            if (_apiOnlyEntities.Contains(_toUpdate.LogicalName))
            {
                ExecuteApi();
            }
            else
            {
                FormData formData;
                if (_seleniumContext.GetBrowser().LastFormData?.GetRecordId() == _toUpdate.Id)
                {
                    formData = _seleniumContext.GetBrowser().LastFormData;
                }
                else
                {
                    formData = RetryPolicies.ElementNotInteractableOnOpenRecord.Execute(
                        () => _seleniumContext.GetBrowser().OpenRecord(new OpenFormOptions(_toUpdate))
                    );
                }

                formData.FillForm(_crmContext, _criteria);

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
                        formData.FillForm(_crmContext, _criteria);
                        formData.Save(true);
                    }
                }
            }
        }
    }
}
