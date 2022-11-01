using BoDi;
using Microsoft.Xrm.Sdk;
using RR.Dynamics365.SpecFlow.Helpers;
using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow;
using Vermaat.Crm.Specflow.Commands;

namespace RR.Dynamics365.SpecFlow.Commands
{
    public class GetRecordsCommand : ApiOnlyCommandFunc<DataCollection<Entity>>
    {
        private readonly string _entityName;
        private readonly Table _criteria;
        private readonly IQueryWithConditionOperatorsHelper _queryHelper;

        public GetRecordsCommand(ICrmTestingContext crmContext, string entityName, Table criteria, IObjectContainer objectContainer) : base(crmContext)
        {
            _entityName = entityName;
            _criteria = criteria;
            _queryHelper = objectContainer.Resolve<IQueryWithConditionOperatorsHelper>();
        }

        public override DataCollection<Entity> Execute()
        {
            Microsoft.Xrm.Sdk.Query.QueryExpression query = _queryHelper.CreateQueryExpressionFromTable(_entityName, _criteria, _crmContext);
            return GlobalTestingContext.ConnectionManager.CurrentConnection.RetrieveMultiple(query).Entities;
        }
    }
}
