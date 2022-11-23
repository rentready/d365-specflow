using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Vermaat.Crm.Specflow;
using Vermaat.Crm.Specflow.Connectivity;
using Vermaat.Crm.Specflow.Entities;

namespace RR.Dynamics365.SpecFlow.Specs.Fixtures
{
    internal class FakeCrmService : ICrmService
    {
        private readonly IOrganizationService _service;
        private UserSettings _userSettings;
        private Guid _userId;
        private Guid _callerId;

        public FakeCrmService(IOrganizationService serivce)
        {
            _service = serivce;
        }

        public UserSettings UserSettings => _userSettings;
        public Guid UserId => _userId;


        public Guid CallerId
        {
            get => _callerId;
            set
            {
                _callerId = value;
                _userSettings = GetUserSettings();
                _userId = GetUserId();
            }
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            _service.Associate(entityName, entityId, relationship, relatedEntities);
        }

        public void Create(Entity entity, string alias, AliasedRecordCache recordCache)
        {
            entity.Id = CreateRecord(entity);
            recordCache.Add(alias, entity.ToEntityReference());
        }

        public Guid Create(Entity entity)
        {
            return CreateRecord(entity);
        }

        public void Delete(EntityReference entityReference)
        {
            _service.Delete(entityReference.LogicalName, entityReference.Id);
        }

        public void Delete(string entityName, Guid id)
        {
            _service.Delete(entityName, id);
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            _service.Disassociate(entityName, entityId, relationship, relatedEntities);
        }

        public T Execute<T>(OrganizationRequest request) where T : OrganizationResponse
        {
            return (T)ExecuteRequest(request);
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return ExecuteRequest(request);
        }

        public Guid GetUserId()
        {
            if (_callerId != Guid.Empty)
                return _callerId;

            return Execute<WhoAmIResponse>(new WhoAmIRequest()).UserId;
        }

        public UserSettings GetUserSettings()
        {
            var query = new QueryExpression("usersettings")
            {
                TopCount = 1,
                ColumnSet = { AllColumns = true }
            };
            query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, UserId);
            var settingsEntity = RetrieveMultiple(query).Entities[0];

            query = new QueryExpression("timezonedefinition")
            {
                TopCount = 1
            };
            query.ColumnSet.AddColumn("standardname");
            query.Criteria.AddCondition("timezonecode", ConditionOperator.Equal, settingsEntity["timezonecode"]);
            var timeZoneEntity = RetrieveMultiple(query).Entities[0];
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneEntity.GetAttributeValue<string>("standardname"));

            return new UserSettings(settingsEntity, timeZoneInfo);
        }

        public Entity Retrieve(EntityReference entityReference, ColumnSet columnSet)
        {
            return _service.Retrieve(entityReference.LogicalName, entityReference.Id, columnSet);
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return _service.Retrieve(entityName, id, columnSet);
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            return _service.RetrieveMultiple(query);
        }

        public void Update(Entity entity)
        {
            throw new NotImplementedException();
        }

        private OrganizationResponse ExecuteRequest(OrganizationRequest request)
        {
            return _service.Execute(request);
        }

        private Guid CreateRecord(Entity entity)
        {
            return _service.Create(entity);
        }
    }
}
