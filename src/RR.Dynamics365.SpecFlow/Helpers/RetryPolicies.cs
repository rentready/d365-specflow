using OpenQA.Selenium;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vermaat.Crm.Specflow;
using Vermaat.Crm.Specflow.EasyRepro;

namespace RR.Dynamics365.SpecFlow.Helpers
{
    internal class RetryPolicies
    {
        /// <summary>
        /// Dynamics 365 may not load a form when user iteracts with it. 
        /// Users typically refresh the page. We are repeating the operation.
        /// </summary>
        public static ISyncPolicy<FormData> ElementNotInteractableOnOpenRecord = Policy
            .Handle<ElementNotInteractableException>()
            .WaitAndRetry(5, (retryCount) => TimeSpan.Zero, (exception, timeSpan) => {
                Console.WriteLine("Failed to open a record.");
                Console.WriteLine(exception.Message);
            })
            .AsPolicy<FormData>();
    }
}
