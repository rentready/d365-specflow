using BoDi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow;
using Vermaat.Crm.Specflow.Commands;
using GetRecordsCommand = RR.Dynamics365.SpecFlow.Commands.GetRecordsCommand;

namespace RR.Dynamics365.SpecFlow.Steps
{
    [Binding]
    [DeploymentItem("DefaultData.xml")]
    public class GeneralSteps
    {
        private readonly CrmTestingContext _crmContext;
        private readonly SeleniumTestingContext _seleniumContext;
        private readonly IObjectContainer _objectContainer;
        private readonly CommandAction _action;

        public GeneralSteps(CrmTestingContext crmContext, SeleniumTestingContext seleniumContext, IObjectContainer objectContainer, ScenarioContext scenarioContext)
        {
            _crmContext = crmContext;
            _seleniumContext = seleniumContext;
            _objectContainer = objectContainer;
            _action = scenarioContext.StepContext.StepInfo.StepDefinitionType == TechTalk.SpecFlow.Bindings.StepDefinitionType.Given ? CommandAction.ForceApi : CommandAction.Default;
        }


        #region Given

        [Given(@"an ([^\s]+) named ([^\s]+) exists with the following values")]
        [Given(@"a ([^\s]+) named ([^\s]+) exists with the following values")]
        public void GivenExistingWithValues(string entityName, string alias, Table criteria)
        {
            Entity entity = ThenRecordExists(entityName, criteria);
            _crmContext.RecordCache.Add(alias, entity, false);
        }

        [Given(@"a ([^\s]+) named (.*) created with the following values")]
        [Given(@"an ([^\s]+) named (.*) created with the following values")]
        [When(@"I create a ([^\s]+) named (.*) with the following values")]
        [When(@"I create an ([^\s]+) named (.*) with the following values")]
        public void GivenEntityWithValues(string entityName, string alias, Table criteria)
        {
            _crmContext.TableConverter.ConvertTable(entityName, criteria);
            _crmContext.CommandProcessor.Execute(new CreateRecordCommand(_crmContext, _seleniumContext, entityName, criteria, alias), _action);
        }

        [Given(@"(.*) has the process stage (.*)")]
        [When(@"I change the process stage of (.*) to (.*)")]
        public void SetProcessStage(string alias, string stageName)
        {
            _crmContext.CommandProcessor.Execute(new MoveToBusinessProcessStageCommand(_crmContext, alias, stageName), _action);
        }

        [Given(@"a related ([^\s]+) from (.*) named (.*) created with the following values")]
        [When(@"I create a related ([^\s]+) from (.*) named (.*) with the following values")]
        public void GivenRelatedEntityWithValues(string entityName, string parentAlias, string childAlias, Table criteria)
        {
            _crmContext.TableConverter.ConvertTable(entityName, criteria);
            _crmContext.CommandProcessor.Execute(new CreateRelatedRecordCommand(_crmContext, _seleniumContext, entityName, criteria, childAlias, parentAlias), _action);
        }

        private const int MAX_RECORDS_TO_DELETE = 5;

        [Given(@"entities ([^\s]+) with the following values are deleted")]
        public void GivenDeleteRelatedEntities(string entityName, Table criteria)
        {
            _crmContext.TableConverter.ConvertTable(entityName, criteria);
            var records = _crmContext.CommandProcessor.Execute(new GetRecordsCommand(_crmContext, entityName, criteria, _objectContainer));

            Assert.IsFalse(records.Count > MAX_RECORDS_TO_DELETE, $"There are {records.Count} entities while the delete operation is constrained to at most {MAX_RECORDS_TO_DELETE} to prevent unintentional mass delete operation because of invalid criteria.");

            foreach (var record in records)
            {
                _crmContext.CommandProcessor.Execute(new Commands.DeleteRecordCommand(_crmContext, record.ToEntityReference()));
            }
        }

        #endregion

        #region When

        [When(@"I move (.*) to the next process stage")]
        public void MoveToNextStage(string alias)
        {
            _crmContext.CommandProcessor.Execute(new MoveToNextBusinessProcessStageCommand(_crmContext, _seleniumContext, alias));
        }

        [When(@"I update (.*) with the following values")]
        public void WhenAliasIsUpdated(string alias, Table criteria)
        {
            EntityReference aliasRef = _crmContext.RecordCache[alias];
            _crmContext.TableConverter.ConvertTable(aliasRef.LogicalName, criteria);
            _crmContext.CommandProcessor.Execute(new UpdateRecordCommand(_crmContext, _seleniumContext, aliasRef, criteria));
        }

        [When(@"I wait when all asynchronous processes for (.*) are finished")]
        public void WhenAsyncJobsFinished(string alias)
        {
            _crmContext.CommandProcessor.Execute(new WaitForAsyncJobsCommand(_crmContext, alias));
        }

        [When(@"I update the status of (.*) to (.*)")]
        public void WhenStatusChanges(string alias, string newStatus)
        {
            _crmContext.CommandProcessor.Execute(new UpdateStatusCommand(_crmContext, alias, newStatus));
        }

        [When(@"I delete (.*)")]
        public void WhenAliasIsDeleted(string alias)
        {
            _crmContext.CommandProcessor.Execute(new DeleteRecordCommand(_crmContext, _seleniumContext, alias));
        }

        [When(@"I assign alias (.*) to (.*)")]
        public void WhenAliasIsAssignedToAlias(string aliasToAssign, string aliasToAssignTo)
        {
            _crmContext.CommandProcessor.Execute(new AssignRecordCommand(_crmContext, aliasToAssign, aliasToAssignTo));
        }

        [When(@"I merge (.*) fully into (.*)")]
        public void WhenRecordsAreMerged(string subordindateAlias, string targetAlias)
        {
            var targetRecord = _crmContext.RecordCache.Get(targetAlias);
            var subordinateRecord = _crmContext.RecordCache.Get(subordindateAlias);
            _crmContext.CommandProcessor.Execute(new MergeRecordsCommand(_crmContext, targetRecord, subordinateRecord));
        }

        [When(@"I merge the following fields of (.*) into (.*)")]
        public void WhenRecordsAreMergedPartial(string subordindateAlias, string targetAlias, Table mergeTable)
        {
            var targetRecord = _crmContext.RecordCache.Get(targetAlias);
            var subordinateRecord = _crmContext.RecordCache.Get(subordindateAlias);
            _crmContext.TableConverter.ConvertTable(subordinateRecord.LogicalName, mergeTable);
            _crmContext.CommandProcessor.Execute(new MergeRecordsCommand(_crmContext, targetRecord, subordinateRecord, mergeTable));
        }

        [Given(@"the following records of type ([^\s]+) are connected to ([^\s]+)")]
        [When(@"I connect the following records of type ([^\s]+) to (.*)")]
        public void AssociateRecordsViaNN(string relatedEntityName, string alias, Table records)
        {
            _crmContext.CommandProcessor.Execute(new AssociateToNNRelationshipCommand(_crmContext, alias, relatedEntityName, records));
        }

        [When(@"I execute workflow '(.*)' on (.*)")]
        public void ExecuteWorkflow(string workflowName, string alias)
        {
            _crmContext.CommandProcessor.Execute(new RunOnDemandWorkflow(_crmContext, workflowName, alias));
        }

        #endregion

        #region Then


        [Then(@"the process stage of (.*) is (.*)")]
        public void AssertProcessStage(string alias, string stageName)
        {
            _crmContext.CommandProcessor.Execute(new AssertBusinessProcessStageCommand(_crmContext, alias, stageName));
        }

        [Then(@"([^\s]+) has the following values")]
        public void ThenAliasHasValues(string alias, Table criteria)
        {
            EntityReference aliasRef = _crmContext.RecordCache[alias];
            _crmContext.TableConverter.ConvertTable(aliasRef.LogicalName, criteria);

            _crmContext.CommandProcessor.Execute(new AssertCrmRecordCommand(_crmContext, aliasRef, criteria));
        }

        [Then(@"a ([^\s]+) exists with the following values")]
        [Then(@"an ([^\s]+) exists with the following values")]
        public Entity ThenRecordExists(string entityName, Table criteria)
        {
            return ThenRecordCountExists(1, entityName, criteria)[0];
        }

        [Then(@"no ([^\s]+) exists with the following values")]
        public void ThenNoRecordExists(string entityName, Table criteria)
        {
            ThenRecordCountExists(0, entityName, criteria);
        }

        [Then(@"([0-9]+) ([^\s]+) records exist with the following values")]
        public DataCollection<Entity> ThenRecordCountExists(int amount, string entityName, Table criteria)
        {
            _crmContext.TableConverter.ConvertTable(entityName, criteria);
            DataCollection<Entity> records = _crmContext.CommandProcessor.Execute(new GetRecordsCommand(_crmContext, entityName, criteria, _objectContainer));
            Assert.AreEqual(amount, records.Count, $"When looking for records for {entityName}, expected {amount}, but found {records.Count} records");
            return records;
        }

        [Then(@"a ([^\s]+) named (.*) exists with the following values")]
        [Then(@"an ([^\s]+) named (.*) exists with the following values")]
        public Entity ThenRecordExistsAndGiveAlias(string entityName, string alias, Table criteria)
        {
            var records = ThenRecordCountExists(1, entityName, criteria);
            _crmContext.RecordCache.Add(alias, records[0], false);
            return records[0];
        }


        [Then(@"(.*) has the following connected records of type ([^\s]+)")]
        public void ThenRecordsAreConnectedViaNN(string alias, string relatedEntityName, Table records)
        {
            _crmContext.CommandProcessor.Execute(new AssertNNRelationshipCommand(_crmContext, alias, relatedEntityName, records));
        }

        [Given(@"that (.*)'s (.*) is named (.*)")]
        [Then(@"(.*)'s (.*) is named (.*)")]
        public void ThenAliasFieldIsAliased(string alias, string lookupField, string lookupAlias)
        {
            _crmContext.CommandProcessor.Execute(new SetLookupAsAliasCommand(_crmContext, alias, lookupField, lookupAlias));
        }

        #endregion

    }
}
