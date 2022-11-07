using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vermaat.Crm.Specflow;
using TechTalk.SpecFlow;
using RR.Dynamics365.SpecFlow.Helpers;
using BoDi;
using FakeItEasy;
using FakeItEasy.Creation;
using Microsoft.Xrm.Sdk.Query;

namespace RR.Dynamics365.SpecFlow.UnitTests.Helpers
{
    public class QueryWIthConditionOperatorsHelperTests
    {
        [Fact]
        public void Should_create_QueryExpression_with_Equal_conditionoperator()
        {
            // Arrange
            var value = "John Snow";
            var criteria = ArrangeCriteria("name", value);
            var queryHelper = ArrangeQueryHelper(value);

            // Act
            var expression = queryHelper.CreateQueryExpressionFromTable(A.Dummy<string>(), criteria, A.Dummy<ICrmTestingContext>());

            // Assert
            Assert.Single(expression.Criteria.Conditions);
            Assert.Equal("name", expression.Criteria.Conditions[0].AttributeName);
            Assert.Equal(ConditionOperator.Equal, expression.Criteria.Conditions[0].Operator);
            Assert.Equal("John Snow", expression.Criteria.Conditions[0].Values[0]);
        }

        [Fact]
        public void Should_create_QueryExpression_with_Null_conditionoperator_when_value_empty()
        {
            // Arrange
            var criteria = ArrangeCriteria("name", "");
            var queryHelper = ArrangeQueryHelper(null);

            // Act
            var expression = queryHelper.CreateQueryExpressionFromTable(A.Dummy<string>(), criteria, A.Dummy<ICrmTestingContext>());

            // Assert
            Assert.Single(expression.Criteria.Conditions);
            Assert.Equal("name", expression.Criteria.Conditions[0].AttributeName);
            Assert.Equal(ConditionOperator.Null, expression.Criteria.Conditions[0].Operator);
        }

        [Theory]
        [InlineData(ConditionOperator.Equal)]
        public void Should_create_QueryExpression_with_conditionoperators(ConditionOperator conditionOperator)
        {
            // Arrange
            var value = "John Snow";
            var queryHelper = ArrangeQueryHelper(value);
            var criteria = ArrangeCriteria("name", conditionOperator.ToString(), value);

            // Act
            var expression = queryHelper.CreateQueryExpressionFromTable(A.Dummy<string>(), criteria, A.Dummy<ICrmTestingContext>());

            // Assert
            Assert.Single(expression.Criteria.Conditions);
            Assert.Equal("name", expression.Criteria.Conditions[0].AttributeName);
            Assert.Equal(conditionOperator, expression.Criteria.Conditions[0].Operator);
            Assert.Equal("John Snow", expression.Criteria.Conditions[0].Values[0]);
        }

        private static QueryWithConditionOperatorsHelper ArrangeQueryHelper(object returnValue)
        {
            var container = new ObjectContainer();
            var converter = A.Fake<IObjectConverter>();

            A.CallTo(converter).Where(x => x.Method.Name == nameof(IObjectConverter.ToCrmObject))
                .WithReturnType<object>()
                .Returns(returnValue);

            container.RegisterInstanceAs(converter);
            container.RegisterTypeAs<QueryWithConditionOperatorsHelper, IQueryWithConditionOperatorsHelper>();
            return container.Resolve<QueryWithConditionOperatorsHelper>();
        }

        private static Table ArrangeCriteria(params string[] tableRow)
        {
            var criteria = tableRow.Length == 2 ?
                new Table(Constants.SpecFlow.TABLE_KEY, Constants.SpecFlow.TABLE_VALUE) :
                new Table(Constants.SpecFlow.TABLE_KEY, QueryWithConditionOperatorsHelper.TABLE_CONDITION_OPERATOR, Constants.SpecFlow.TABLE_VALUE);

            criteria.AddRow(tableRow);

            return criteria;
        }
    }
}
