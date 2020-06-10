/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.LocalAuthentication;

namespace PTV.IdentityUserManager.Controllers
{
    //[Authorize] // ensures that the user is authenticated to STS application
    public class HomeController : Controller
    {
        private readonly SignInManager<StsUser> _signInManager;
        private IUserOrganizationService userOrganizationService;
        private UserManager<StsUser> userManager;
        private RoleManager<StsRole> roleManager;

        /// <summary>
        /// Logger for controller.
        /// </summary>
        private readonly ILogger logger;

        public HomeController(SignInManager<StsUser> signInManager,
            IUserOrganizationService userOrganizationService,
            UserManager<StsUser> userManager,
            RoleManager<StsRole> roleManager,
            ILogger<HomeController> logger)
        {
            _signInManager = signInManager;
            this.userOrganizationService = userOrganizationService;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
        }

        [Route("/")]
        public IActionResult Index()
        {
            var user = User;
            // fix to show different page to users if they are not logged in to STS application but have logged to an application that uses STS for authentication
            // the policy "PolicyDefinition.AuthorizedUserForSTS" checks that the user is authenticated and belongs to certain roles and all these are true for PTV users currently
            // but the users are not logged in to STS application, so we show a different page for them here
            if (!_signInManager.IsSignedIn(User))
            {
                // TODO : Check can we log something identifying about the use
                logger.LogInformation(AuthenticationServerLogEvents.General, "User is authenticated but not logged-in to STS-application (logged-in to STS-application situation).");
                return Redirect("/Login");
            }
            var roles = roleManager.Roles.ToDictionary(x => x.Id, x => x.Name);

            // TODO: shouldn't we get the user the same way everywhere in the application? Like in AccountController
            // using the ClaimTypes.NameIdentifier and FindByIdAsync
            var authUser = userManager.FindByEmailAsync(User.Identity.Name).Result;

            if (authUser == null)
            {
                string message = $"Logged-in user '{User.Identity.Name}' not found from database.";
                logger.LogError(AuthenticationServerLogEvents.General, message);
                throw new Exception(message);
            }
            var userOrgs = userOrganizationService.GetUsersWithOrganizations(new List<Guid> {userManager.FindByEmailAsync(User.Identity.Name).Result.Id});
           var result = userOrgs.Select(connectedUser => new VmUserOrganizationSts
           {
                    UserName = User.Identity.Name,
                    UserId = connectedUser.UserId,
                    OrganizationId = connectedUser.OrganizationId,
                    OrganizationName = connectedUser.OrganizationName,
                    RoleId = connectedUser.RoleId,
                    Role = connectedUser.RoleId.IsAssigned() ? roles[connectedUser.RoleId] : string.Empty
                }).ToList();
            return View(result);
        }
    }
}
