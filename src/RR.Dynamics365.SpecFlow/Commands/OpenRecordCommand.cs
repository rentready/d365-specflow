using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow.EasyRepro;
using Vermaat.Crm.Specflow.Commands;
using Vermaat.Crm.Specflow;

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
            FormData formData;
            if (_seleniumContext.GetBrowser().LastFormData?.GetRecordId() == _toOpen.Id)
            {
                Console.WriteLine("Entity is already opened");
            }
            else
            {
                try
                {
                    _seleniumContext.GetBrowser().OpenRecord(new OpenFormOptions(_toOpen));
                }
                catch (OpenQA.Selenium.ElementNotInteractableException ex)
                {
                    Console.WriteLine("Failed to open record for an Update command.");
                    Console.WriteLine(ex.Message);
                    HelperMethods.WaitForFormLoad(_seleniumContext.GetBrowser().App.WebDriver);
                    _seleniumContext.GetBrowser().App.Client.Browser.ThinkTime(10000);
                    _seleniumContext.GetBrowser().OpenRecord(new OpenFormOptions(_toOpen));
                }
            }
        }
    }
}
