﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Vermaat.Crm.Specflow;
using Vermaat.Crm.Specflow.Commands;
using GetRecordsCommand = RR.Dynamics365.SpecFlow.Commands.GetRecordsCommand;

namespace RR.Dynamics365.SpecFlow.Steps
{
    [Binding]
    public class UserSteps
    {
        private readonly ICrmTestingContext _crmContext;
        private readonly ISeleniumTestingContext _selenumContext;
        private readonly UserProfileHandler _userProfileHandler;

        public UserSteps(ICrmTestingContext crmContext, ISeleniumTestingContext selenumContext, UserProfileHandler userProfileHandler)
        {
            _crmContext = crmContext;
            _selenumContext = selenumContext;
            _userProfileHandler = userProfileHandler;
        }

        [Given(@"a logged in '(.*)'")]
        public void LoginWithUser(string profile)
        {
            _crmContext.CommandProcessor.Execute(new LoginWithUserCommand(_crmContext, _userProfileHandler.GetProfile(profile)));
        }

        [Given(@"a logged in '(.*)' named ([^\s]+)")]
        public void LoginWithUser(string profile, string alias)
        {
            LoginWithUser(profile);
            _crmContext.CommandProcessor.Execute(new GetCurrentUserCommand(_crmContext, alias));
        }

        [Given(@"the current logged in user named (.*)")]
        public void GetLoggedInUser(string alias)
        {
            _crmContext.CommandProcessor.Execute(new GetCurrentUserCommand(_crmContext, alias));
        }


        [Given(@"the current logged in user's settings named (.*)")]
        public void GetLoggedInUserSettings(string alias)
        {
            _crmContext.CommandProcessor.Execute(new GetCurrentUserSettingsCommand(_crmContext, alias));
        }

    }
}
