using FakeXrmEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vermaat.Crm.Specflow.Connectivity;

namespace RR.Dynamics365.SpecFlow.Specs.Fixtures
{
    internal class FakeCrmConnection : CrmConnection
    {
        private readonly XrmFakedContext _context;

        public FakeCrmConnection(string identifier, XrmFakedContext context) : base(identifier)
        {
            _context = context;
        }

        public override ICrmService CreateCrmServiceInstance()
        {
            return new FakeCrmService(_context.GetOrganizationService());
        }

        public override BrowserLoginDetails GetBrowserLoginInformation()
        {
            throw new NotImplementedException();
        }
    }
}
