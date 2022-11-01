using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow;

namespace RR.Dynamics365.SpecFlow
{
    internal static class QueryWIthConditionOperatorsHelper
    {
        public const string TABLE_CONDITION_OPERATOR = "Condition";

        public static QueryExpression CreateQueryExpressionFromTable(string entityName, Table criteria, CrmTestingContext context)
        {
            Logger.WriteLine($"Creating Query for {entityName}");
            QueryExpression qe = new QueryExpression(entityName)
            {
                ColumnSet = new ColumnSet()
            };

            foreach (var row in criteria.Rows)
            {
                var crmValue = ObjectConverter.ToCrmObject(entityName, row[Constants.SpecFlow.TABLE_KEY], row[Constants.SpecFlow.TABLE_VALUE], context, ConvertedObjectType.Primitive);

                row.TryGetValue(TABLE_CONDITION_OPERATOR, out string operationStr);
                var conditionOperator = (ConditionOperator)(!String.IsNullOrWhiteSpace(operationStr) ? Enum.Parse(typeof(ConditionOperator), operationStr) : ConditionOperator.Equal);

                if (crmValue == null)
                {
                    Logger.WriteLine($"Adding condition {row[Constants.SpecFlow.TABLE_KEY]} IS NULL");
                    qe.Criteria.AddCondition(row[Constants.SpecFlow.TABLE_KEY], ConditionOperator.Null);
                }
                else
                {
                    Logger.WriteLine($"Adding condition {row[Constants.SpecFlow.TABLE_KEY]} Equals {crmValue}");
                    qe.Criteria.AddCondition(row[Constants.SpecFlow.TABLE_KEY], conditionOperator, crmValue);
                }
            }

            return qe;
        }
    }
}
