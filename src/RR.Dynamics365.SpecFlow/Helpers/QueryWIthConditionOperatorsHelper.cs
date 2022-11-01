using Microsoft.Xrm.Sdk.Query;
using System.Runtime.CompilerServices;
using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow;

[assembly: InternalsVisibleTo("RR.Dynamics365.SpecFlow.UnitTests")]
namespace RR.Dynamics365.SpecFlow.Helpers
{
    internal class QueryWithConditionOperatorsHelper : IQueryWithConditionOperatorsHelper
    {
        private IObjectConverter _objectConverter;
        public const string TABLE_CONDITION_OPERATOR = "Condition";

        public QueryWithConditionOperatorsHelper(IObjectConverter objectConverter)
        {
            _objectConverter = objectConverter;
        }

        public QueryExpression CreateQueryExpressionFromTable(string entityName, Table criteria, ICrmTestingContext context)
        {
            Logger.WriteLine($"Creating Query for {entityName}");
            QueryExpression qe = new QueryExpression(entityName)
            {
                ColumnSet = new ColumnSet()
            };

            foreach (var row in criteria.Rows)
            {
                var crmValue = _objectConverter.ToCrmObject(entityName, row[Constants.SpecFlow.TABLE_KEY], row[Constants.SpecFlow.TABLE_VALUE], context, ConvertedObjectType.Primitive);

                row.TryGetValue(TABLE_CONDITION_OPERATOR, out string operationStr);
                var conditionOperator = (ConditionOperator)(!string.IsNullOrWhiteSpace(operationStr) ? Enum.Parse(typeof(ConditionOperator), operationStr) : ConditionOperator.Equal);

                if (crmValue == null)
                {
                    Logger.WriteLine($"Adding condition {row[Constants.SpecFlow.TABLE_KEY]} IS NULL");
                    qe.Criteria.AddCondition(row[Constants.SpecFlow.TABLE_KEY], ConditionOperator.Null);
                }
                else
                {
                    Logger.WriteLine($"Adding condition {row[Constants.SpecFlow.TABLE_KEY]} {conditionOperator} {crmValue}");
                    qe.Criteria.AddCondition(row[Constants.SpecFlow.TABLE_KEY], conditionOperator, crmValue);
                }
            }

            return qe;
        }
    }
}
