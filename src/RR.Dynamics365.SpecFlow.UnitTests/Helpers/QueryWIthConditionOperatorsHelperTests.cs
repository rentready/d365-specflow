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
        public void Should_create_query()
        {
            // Arrange
            var criteria = new Table(Constants.SpecFlow.TABLE_KEY, QueryWithConditionOperatorsHelper.TABLE_CONDITION_OPERATOR, Constants.SpecFlow.TABLE_VALUE);
            var value = "John Snow";
            criteria.AddRow("name", "Equal", value);
            var container = new ObjectContainer();
            var converter = A.Fake<IObjectConverter>();

            A.CallTo(converter).Where(x => x.Method.Name == nameof(IObjectConverter.ToCrmObject))
                .WithReturnType<object>()
                .Returns(value);

            container.RegisterInstanceAs(converter);
            container.RegisterTypeAs<QueryWithConditionOperatorsHelper, IQueryWithConditionOperatorsHelper>();
            var queryHelper = container.Resolve<QueryWithConditionOperatorsHelper>();

            // Act
            var expression = queryHelper.CreateQueryExpressionFromTable(A.Dummy<string>(), criteria, A.Dummy<ICrmTestingContext>());

            // Assert
            Assert.Single(expression.Criteria.Conditions);
            Assert.Equal("name", expression.Criteria.Conditions[0].AttributeName);
            Assert.Equal(ConditionOperator.Equal, expression.Criteria.Conditions[0].Operator);
            Assert.Equal("John Snow", expression.Criteria.Conditions[0].Values[0]);
        }
    }
}
