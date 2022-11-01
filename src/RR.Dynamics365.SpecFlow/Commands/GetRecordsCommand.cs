using Microsoft.Xrm.Sdk;
using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow;
using Vermaat.Crm.Specflow.Commands;

namespace RR.Dynamics365.SpecFlow.Commands
{
    public class GetRecordsCommand : ApiOnlyCommandFunc<DataCollection<Entity>>
    {
        private readonly string _entityName;
        private readonly Table _criteria;

        public GetRecordsCommand(CrmTestingContext crmContext, string entityName, Table criteria) : base(crmContext)
        {
            _entityName = entityName;
            _criteria = criteria;
        }

        public override DataCollection<Entity> Execute()
        {
            Microsoft.Xrm.Sdk.Query.QueryExpression query = QueryWIthConditionOperatorsHelper.CreateQueryExpressionFromTable(_entityName, _criteria, _crmContext);
            return GlobalTestingContext.ConnectionManager.CurrentConnection.RetrieveMultiple(query).Entities;
        }
    }
}
