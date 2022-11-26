﻿using Microsoft.Xrm.Sdk;
using System.Linq;
using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow.EasyRepro;
using Vermaat.Crm.Specflow.Commands;
using Vermaat.Crm.Specflow;

namespace RR.Dynamics365.SpecFlow.Commands
{
    public class UpdateRecordCommand : BrowserCommand
    {
        private static readonly string[] _apiOnlyEntities = new string[] { "usersettings" };

        private readonly EntityReference _toUpdate;
        private readonly Table _criteria;

        public UpdateRecordCommand(ICrmTestingContext crmContext, ISeleniumTestingContext seleniumContext, EntityReference toUpdate,
            Table criteria)
            : base(crmContext, seleniumContext)
        {
            _toUpdate = toUpdate;
            _criteria = criteria;
        }

        protected override void ExecuteApi()
        {
            Entity toUpdate = new Entity(_toUpdate.LogicalName)
            {
                Id = _toUpdate.Id
            };

            foreach (TableRow row in _criteria.Rows)
            {
                toUpdate[row[Constants.SpecFlow.TABLE_KEY]] = ObjectConverter.ToCrmObject(_toUpdate.LogicalName,
                    row[Constants.SpecFlow.TABLE_KEY], row[Constants.SpecFlow.TABLE_VALUE], _crmContext);
            }

            GlobalTestingContext.ConnectionManager.CurrentConnection.Update(toUpdate);
        }

        protected override void ExecuteBrowser()
        {
            if (_apiOnlyEntities.Contains(_toUpdate.LogicalName))
            {
                ExecuteApi();
            }
            else
            {
                var formData = _seleniumContext.GetBrowser().LastFormData.GetRecordId() == _toUpdate.Id 
                    ? _seleniumContext.GetBrowser().LastFormData 
                    : _seleniumContext.GetBrowser().OpenRecord(new OpenFormOptions(_toUpdate));
                formData.FillForm(_crmContext, _criteria);
                formData.Save(true);
            }
        }
    }
}
