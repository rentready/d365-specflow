using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow.EasyRepro;
using Vermaat.Crm.Specflow.Commands;
using Vermaat.Crm.Specflow;
using RR.Dynamics365.SpecFlow.Helpers;

namespace RR.Dynamics365.SpecFlow.Commands
{
    public class OpenRecordCommand : BrowserCommand
    {
        private readonly EntityReference _toOpen;

        public OpenRecordCommand(ICrmTestingContext crmContext, ISeleniumTestingContext seleniumContext, EntityReference toOpen)
            : base(crmContext, seleniumContext)
        {
            _toOpen = toOpen;
        }

        protected override void ExecuteApi()
        {
            throw new InvalidOperationException("This is browser-only command");
        }

        protected override void ExecuteBrowser()
        {
            if (_seleniumContext.GetBrowser().LastFormData?.GetRecordId() == _toOpen.Id)
            {
                Console.WriteLine("Entity is already opened");
            }
            else
            {
                RetryPolicies.ElementNotInteractableOnOpenRecord.Execute(() => _seleniumContext.GetBrowser().OpenRecord(new OpenFormOptions(_toOpen)));
            }
        }
    }
}
