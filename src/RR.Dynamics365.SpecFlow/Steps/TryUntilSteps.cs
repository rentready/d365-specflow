using Microsoft.Xrm.Sdk;
using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow;
using Vermaat.Crm.Specflow.Commands;

namespace RR.Dynamics365.SpecFlow.Steps
{
    [Binding]
    public class TryUntilSteps
    {
        private readonly CrmTestingContext _crmContext;
        private readonly SeleniumTestingContext _seleniumContext;

        public TryUntilSteps(CrmTestingContext crmContext, SeleniumTestingContext seleniumContext)
        {
            _crmContext = crmContext;
            _seleniumContext = seleniumContext;
        }

        #region Given

        [When(@"I wait an existing ([^\s]+) named ([^\s]+) with the following values is available within ([0-9]+) seconds")]
        [Given(@"an existing ([^\s]+) named ([^\s]+) with the following values is available within ([0-9]+) seconds")]
        public void GivenExistingWithValues(string entityName, string alias, int seconds, Table criteria)
        {
            Entity entity = ThenRecordExists(seconds, entityName, criteria);
            _crmContext.RecordCache.Add(alias, entity, false);
        }

        #endregion

        #region Then

        [Given(@"within ([0-9]+) seconds ([^\s]+) has the following values")]
        [When(@"I wait within ([0-9]+) seconds ([^\s]+) has the following values")]
        [Then(@"I expect within ([0-9]+) seconds ([^\s]+) has the following values")]
        public void ThenAliasHasValues(int seconds, string alias, Table criteria)
        {
            EntityReference aliasRef = _crmContext.RecordCache[alias];
            _crmContext.TableConverter.ConvertTable(aliasRef.LogicalName, criteria);

            TryUntil(new AssertCrmRecordCommand(_crmContext, aliasRef, criteria), seconds);
        }

        [When(@"I wait within ([0-9]+) seconds a ([^\s]+) exists with the following values")]
        [When(@"I wait within ([0-9]+) seconds an ([^\s]+) exists with the following values")]
        [Then(@"I expect within ([0-9]+) seconds a ([^\s]+) exists with the following values")]
        [Then(@"I expect within ([0-9]+) seconds an ([^\s]+) exists with the following values")]
        public Entity ThenRecordExists(int seconds, string entityName, Table criteria)
        {
            return ThenRecordCountExists(seconds, 1, entityName, criteria)[0];
        }

        [When(@"I esure within ([0-9]+) seconds no ([^\s]+) exists with the following values")]
        [Then(@"I expect within ([0-9]+) seconds no ([^\s]+) exists with the following values")]
        public void ThenNoRecordExists(int seconds, string entityName, Table criteria)
        {
            ThenRecordCountExists(seconds, 0, entityName, criteria);
        }

        [When(@"I ensure within ([0-9]+) seconds ([0-9]+) ([^\s]+) records exist with the following values")]
        [Then(@"I expect within ([0-9]+) seconds ([0-9]+) ([^\s]+) records exist with the following values")]
        public DataCollection<Entity> ThenRecordCountExists(int seconds, int amount, string entityName, Table criteria)
        {
            _crmContext.TableConverter.ConvertTable(entityName, criteria);

            return TryUntil<GetRecordsCommand, DataCollection<Entity>>(new GetRecordsCommand(_crmContext, entityName, criteria), seconds,
                r => r.Count == amount,
                r => $"When looking for records for {entityName}, expected {amount}, but found {r.Count} records");
        }

        #endregion

        private void TryUntil<TCommand>(TCommand command, int seconds) where TCommand : ICommand
        {
            _crmContext.CommandProcessor.Execute(new TryUntilCommand<TCommand>(_crmContext, command, TimeSpan.FromSeconds(seconds), TimeSpan.FromSeconds(5)));
        }

        private TResult TryUntil<TCommand, TResult>(TCommand command, int seconds, Func<TResult, bool> assertFunc, Func<TResult, string> timeoutMessageFunc) where TCommand : ICommandFunc<TResult>
        {
            return _crmContext.CommandProcessor.Execute(new TryUntilCommandFunc<TCommand, TResult>(_crmContext, command,
                TimeSpan.FromSeconds(seconds), TimeSpan.FromSeconds(5), assertFunc, timeoutMessageFunc));
        }
    }
}
