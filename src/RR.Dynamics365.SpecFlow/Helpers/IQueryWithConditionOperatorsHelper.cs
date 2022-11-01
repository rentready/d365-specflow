using Microsoft.Xrm.Sdk.Query;
using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow;

namespace RR.Dynamics365.SpecFlow.Helpers
{
    public interface IQueryWithConditionOperatorsHelper
    {
        QueryExpression CreateQueryExpressionFromTable(string entityName, Table criteria, ICrmTestingContext context);
    }
}