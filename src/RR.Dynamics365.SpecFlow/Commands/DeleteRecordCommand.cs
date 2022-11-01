using Microsoft.Xrm.Sdk;
using Vermaat.Crm.Specflow;
using Vermaat.Crm.Specflow.Commands;

namespace RR.Dynamics365.SpecFlow.Commands
{
    public class DeleteRecordCommand : ApiOnlyCommand
    {
        private readonly EntityReference _toDelete;

        public DeleteRecordCommand(ICrmTestingContext crmContext, EntityReference entity)
            : base(crmContext)
        {
            _toDelete = entity;
        }

        public override void Execute()
        {
            GlobalTestingContext.ConnectionManager.CurrentConnection.Delete(_toDelete);
        }
    }
}
