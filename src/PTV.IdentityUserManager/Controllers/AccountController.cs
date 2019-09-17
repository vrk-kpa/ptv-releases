/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
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
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebSockets.Internal;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.IdentityUserManager.Models;
using PTV.IdentityUserManager.Services;
using PTV.LocalAuthentication;

namespace PTV.IdentityUserManager.Controllers
{
    /// <summary>
    /// Class responsible for action related to account
    /// </summary>
    [Authorize] // ensures that the user is authenticated to STS application
    [Route("/Account")]
    public class AccountController : StsController
    {
        private readonly IUserService userService;
        private readonly SignInManager<StsUser> signInManager;
        private readonly UserManager<StsUser> userManager;
        private readonly RoleManager<StsRole> roleManager;
        private readonly ILogger logger;
        private readonly IUserOrganizationService userOrganizationService;
        private readonly IStringLocalizer<SharedResources> localizer;
        private const string InvalidLoginAttemptMessage = "Virheellinen kirjautumisyritys.";
        private const string AccessDeniedForUserMessage = "Pääsy estetty!";
        private const string ChPassForUserPeteNotConnectedMessage = "Tunnustasi ei ole yhdistetty organisaation. Et voi vaihtaa käyttäjien salasanaa.";
        private const string PeteTriedToResetPasswordForEevaMessage = "Tunnuksellasi ei ole tarvittavia käyttöoikeuksia kyseisen käyttäjän salasanan resetointiin. Pyydä organisaatiosi pääkäyttäjää resetoimaan salasana.";
        private const string PeteTriedToResetPasswordForUserOutsideHisOrganization = "Tunnuksellasi ei ole tarvittavia käyttöoikeuksia muuttaa toisen organisaation käyttäjän salasanaa.";

        /// <summary>
        /// Log message for authenticate user StsUser object missing.
        /// </summary>
        private const string AuthenticatedUserMissingErrorMessage = "Request is authenticated but the current user StsUser object can not be found.";

        public AccountController(
            IHttpContextAccessor contextAccessor,
            ITokenService tokenService,
            IUserIdentification userIdentification,
            IUserService userService,
            SignInManager<StsUser> signInManager,
            UserManager<StsUser> userManager,
            RoleManager<StsRole> roleManager,
            ILogger<AccountController> logger,
            IOrganizationService organizationService,
			IUserOrganizationService userOrganizationService,
            ICommonService commonService,
            IStringLocalizer<SharedResources> localizer) : base(contextAccessor, tokenService, userIdentification, userOrganizationService)
        {
            this.userService = userService;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.localizer = localizer;
            this.userOrganizationService = userOrganizationService;
            this.logger = logger;
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, NoStore = true)]
        [HttpGet("/Login")]
        public IActionResult Login(SimpleLoginViewModel model)
        {
            var vm = new LoginViewModel();

            return View("Index", vm);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, NoStore = true)]
        [HttpGet("/Logout")]
        public IActionResult Index(string id)
        {
            HttpContext.SignOutAsync().Wait();
            var vm = new LoggedOutViewModel();
            return View("LoggedOut", vm);
        }

        private void UpdateClaim(List<Claim> claims, StsUser user, string claimType)
        {
            var oldRole = userManager.GetClaimsAsync(user).Result.FirstOrDefault(i => i.Type == claimType);
            if (oldRole != null)
            {
                userManager.RemoveClaimAsync(user, oldRole).Wait();
            }

            userManager.AddClaimAsync(user, claims.FirstOrDefault(i => i.Type == claimType)).Wait();
        }

        [AllowAnonymous]
        [HttpPost("/Login")]
        [HttpPost("Login")]
        [ResponseCache(Duration = 0, NoStore = true)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var canSignIn = await userManager.CheckPasswordAsync(user, model.Password);
                    if (canSignIn)
                    {
                        if (userManager.IsLockedOutAsync(user).Result)
                        {
                            logger.LogInformation($"Account '{user.UserName}' is locked out.");
                            return View(new LoginViewModel { Id = model.SignInId });
                        }
                        var roles = roleManager.Roles.ToDictionary(x => x.Id, x => x.Name);
                        var allRolesAndOrgs = userOrganizationService.GetUserOrganizationsWithRoles(user.Id);
                        var allRoles = allRolesAndOrgs.Select(i => i.RoleId).Distinct().Select(i => roles[i]).ToList();
                        var highestRole = allRoles.Any(i => i.Is(UserRoleEnum.Eeva.ToString())) ? UserRoleEnum.Eeva.ToString() :
                            allRoles.Any(i => i.Is(UserRoleEnum.Pete.ToString())) ? UserRoleEnum.Pete.ToString() : UserRoleEnum.Shirley.ToString();
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Role, highestRole),
                            new Claim(ClaimTypes.UserData, (allRolesAndOrgs.FirstOrDefault(i => roles[i.RoleId] == highestRole)?.OrganizationId ?? Guid.Empty).ToString()),
                        };
                        UpdateClaim(claims, user,ClaimTypes.Role);
                        UpdateClaim(claims, user,ClaimTypes.UserData);
                       
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)).Wait();
                        signInManager.SignInAsync(user, new AuthenticationProperties(), CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                        Response.Redirect("/");
                        return View(new LoginViewModel {Id = model.SignInId}); 
                    }
                    else
                    {
                        // invalid password
                        logger.LogInformation($"Invalid password for login account: '{model.Email}'.");
                    }
                }
                else
                {
                    // no user found with email
                    logger.LogInformation($"No user found with email: '{model.Email}'.");

                    ModelState.AddModelError(string.Empty, InvalidLoginAttemptMessage);
                    return View(new LoginViewModel { Id = model.SignInId });
                }
            }
            else
            {
                // log invalid login attempt as the UI model validation has been by passed
                logger.LogInformation("Login model invalid (user input).");
            }

            ModelState.AddModelError(string.Empty, InvalidLoginAttemptMessage);
            return View(new LoginViewModel { Id = model.SignInId });
        }
        
        public IActionResult AccessDenied()
        {
            // this action is for authenticated users but who are not authorized to a controller or action
            // this action is application wide and defined in IdentityServerManager\IdentityServerManager.cs
            // method: GetEntityFrameworkOptions()

            return CreateAccessToActionDeniedResult();
        }

        /// <summary>
        /// Creates the current user not found error view result.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is intended to be used with: var user = GetCurrentUser(); in cases when the returned user is null and the action requires authenticated user.
        /// </para>
        /// </remarks>
        /// <param name="errorMessage">optional additional error message to be displayed for end user.</param>
        /// <returns>returns the error view result</returns>
        protected IActionResult CreateCurrentUserNotFoundErrorResult(string errorMessage = null)
        {
            var vr = View("CurrentUserNotFoundError", errorMessage);
            vr.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            return vr;
        }

        /// <summary>
        /// Returns the access to action denied view.
        /// </summary>
        protected IActionResult CreateAccessToActionDeniedResult(string errorMessage = null)
        {
            var vr = View("AccessToActionDenied", errorMessage);
            vr.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
            return vr;
        }

        /// <summary>
        /// Creates a general UI error view.
        /// </summary>
        /// <param name="errorDescription">description of the error</param>
        protected IActionResult CreateGeneralErrorResult(string errorDescription)
        {
            // this whole general error view stuff is just a temporary quick way of doing this
            var vr = View("GeneralError", errorDescription);
            vr.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;

            return vr;
        }

        //
        // GET: /Account/Register
        [HttpGet("Register")]
        [AuthorizeRoles(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        public IActionResult Register()
        {
            var roles = roleManager.Roles.ToDictionary(x => x.Name, x => x.Id);
            var peteRoleId = roles[UserRoleEnum.Pete.ToString()];
            var allOrganizations = User.HasRole(UserRoleEnum.Pete)
                ? userService.GetUserOrganizationsForUserName(User.Identity.Name).Where(i => i.Value == peteRoleId).Select(x => x.Key).ToList()
                : (User.HasRole(UserRoleEnum.Eeva) ? userOrganizationService.GetAllOrganizations() : null);
            var model = new RegisterViewModel()
            {
                Organizations = userOrganizationService.GetFullOrganizationsForIds(allOrganizations).OrderBy(x => x.Text).ToList()
            };
            if (model.Organizations.Count == 1)
            {
                model.Organizations.First().Selected = true;
            }
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost("Register")]
        [AuthorizeRoles(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userRoleRight = GetRoleForOrganizations(new List<Guid>() {model.Organization.ParseToGuidWithExeption()});

                if  (!User.HasRole(UserRoleEnum.Eeva) && (userRoleRight == null || userRoleRight == UserRoleEnum.Shirley))
                {
                    logger.LogCreateUserNotAllowed($"Username '{User.Identity.Name}' is trying to create a user (new user email: {model.Email}, first name: {model.GivenName}, last name: {model.SurName}) to organization '{model.Organization}' she doesn't have required role rights (role for organization: '{(userRoleRight == null ? "empty" : userRoleRight.ToString())}').");
                    return CreateAccessToActionDeniedResult();
                }

                // if user is in Pete role they should be only allowed to create a user in the same organization
               if (userRoleRight == UserRoleEnum.Pete){
                    if (string.Compare(model.Role, UserRoleEnum.Eeva.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // Pete role is trying to create a user with role Eeva
                        logger.LogCreateUserNotAllowed($"Username '{User.Identity.Name}' with Pete role is trying to create a user with role Eeva (new user email: {model.Email}, first name: {model.GivenName}, last name: {model.SurName}).");
                        return CreateAccessToActionDeniedResult();
                    }
                    
                    var orgs = userService.GetUserAllSubOrganizationsForUserName(User.Identity.Name);
                    // Pete role is not allowed to add new users to other organizations than his own organization
                   

                    // just trusting that Guid strings are in the same format
                    if (orgs == null || orgs.Count == 0 || !orgs.Any(x => string.Compare(x.ToString(), model.Organization, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        logger.LogCreateUserNotAllowed($"Username '{User.Identity.Name}' with Pete role is trying to create a user (new user email: {model.Email}, first name: {model.GivenName}, last name: {model.SurName}, organization: {model.Organization}) to another organization than his own organization.");
                        return CreateAccessToActionDeniedResult();
                    }
                }

                var user = new StsUser { UserName = model.Email,
                                                Email = model.Email,
                                                Name = $"{model.GivenName} {model.SurName}" };

                var result = userService.CreateNewUser(user, model.Password).Result;

                if (result.Succeeded)
                {
                    var orgDic = new Dictionary<Guid, Guid> {{ model.Organization.ParseToGuidWithExeption(), roleManager.Roles.FirstOrDefault(x => x.Name == model.Role).Id } };
                    var savedUser = userManager.FindByEmailAsync(model.Email).Result;
                    userService.MapUserToOrganization(savedUser.Id.ToString(), orgDic);

                    logger.LogCreatedUser($"User '{User.Identity.Name}' created a new account. UserName: '{savedUser.UserName}', Role: {savedUser.UserName}, Organization id: {model.Organization}.");

                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                else
                {
                    // failed to create the user
                    string createErrors = GetIdentityErrors(result.Errors);
                    logger.LogFailedToCreateUser($"User '{User.Identity.Name}' tried to create a new account. E-mail: '{model.Email}', Role: {model.Role}, Organization id: {model.Organization}. Create errors: {createErrors}.");

                    AddErrors(result);
                }
            }
            var roles = roleManager.Roles.ToDictionary(x => x.Name, x => x.Id);
//            var peteRoleId = roles[UserRoleEnum.Pete];
//            var allOrganizations = User.HasRole(UserRoleEnum.Pete)
//                ? userService.GetUserOrganizationsForUserName(User.Identity.Name).Where(i => i.Value == peteRoleId).Select(x => x.Key).ToList()
//                : (User.HasRole(UserRoleEnum.Eeva) ? userOrganizationService.GetAllOrganizations() : null);
//            model.Organizations = userOrganizationService.GetFullOrganizationsForIds(allOrganizations).OrderBy(x => x.Text).ToList();
            var selected = model.Organizations.FirstOrDefault(i => i.Value == model.Organization);
            if (selected != null)
            {
                selected.Selected = true;
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet("ListUsers")]
        [AuthorizeRoles(UserRoleEnum.Eeva)]
        public IActionResult ListUsers()
        {
            var allAppUsers = userService.GetConnectedUsers();
            var model = new ListUsersViewModel()
            {
                Users = allAppUsers
            };
            return View(model);
        }

        [HttpGet("ChangeUserInfo")]
        [AuthorizeRoles(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        public IActionResult ChangeUserInfo(string userId)
        {
            Guid? userIdToBeFetched = null;
            bool userIdSupplied = false;

            // quick validation for the userId if it is supplied
            if (!string.IsNullOrWhiteSpace(userId))
            {
                userIdToBeFetched = userId.ParseToGuid();

                if (!userIdToBeFetched.HasValue || userIdToBeFetched == Guid.Empty)
                {
                    // desc text is: Invalid input.
                    return CreateGeneralErrorResult("Virheellinen syöte.");
                }

                userIdSupplied = true;
            }

            var user = GetCurrentUser();

            if (user == null)
            {
                logger.LogError(AuthenticationServerLogEvents.AuthenticatedUserMissingError, AuthenticatedUserMissingErrorMessage);
                return CreateCurrentUserNotFoundErrorResult();
            }
            bool isPeteRole = User.HasRole(UserRoleEnum.Pete);
            bool isEevaRole = User.HasRole(UserRoleEnum.Eeva);
            var allUsers = (!isEevaRole && !isPeteRole) ? new List<StsUser>() { user } : userService.GetAllUsers();
            if (!isEevaRole && isPeteRole)
            {
                var roles = roleManager.Roles.ToDictionary(x => x.Name, x => x.Id);
                var peteRoleId = roles[UserRoleEnum.Pete.ToString()];
                var coUsers = userOrganizationService.GetAllCoAndSubUsers(user.Id, peteRoleId);
                allUsers = allUsers.Where(i => coUsers.Contains(i.Id)).ToList();
            }

            var model = new ChangeUserInfoViewModel();
            var allOrg = User.HasRole(UserRoleEnum.Eeva) ? userOrganizationService.GetAllOrganizations() : userService.GetUserOrganizations(user.Id)/*.Where(i => i.Value == roles[UserRoleEnum.Pete])*/.Select(i => i.Key).ToList();
            var userToChange = user;
            if ((isEevaRole || isPeteRole) && userIdSupplied)
            {
                userToChange = userManager.FindByIdAsync(userId).Result;
            }
            else
            {
                model.UserId = user.Id.ToString();
            }

            var claims = userManager.GetClaimsAsync(userToChange).Result;
            model.Email = userToChange.UserName;
            model.FullName = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ?? string.Empty;

            model.Users = allUsers.Select(userToList => new SelectListItem() {Value = userToList.Id.ToString(), Text = userToList.Email}).ToList();
            model.OrganizationsToList = userOrganizationService.GetFullOrganizationsForIds(allOrg).OrderBy(x => x.Text).ToList();
            model.OrganizationsAuthenticatedUser = userService.GetUserOrganizations(user.Id);
            model.OrganizationsChangedUser = userOrganizationService.GetUserDirectlyAssignedOrganizations(userToChange.Id);
            model.Organizations = model.OrganizationsChangedUser;
            model.RoleIds = roleManager.Roles.ToDictionary(x => x.Name, x => x.Id);
            model.IsEevaRole = isEevaRole;
            model.IsPeteRole = isPeteRole;
            return View(model);
        }

        [HttpPost("ChangeUserInfoSave")]
        [AuthorizeRoles(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeUserInfoSave(ChangeUserInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roles = roleManager.Roles.ToDictionary(x => x.Name, x => x.Id);

                var authUser = GetCurrentUser();

                if (authUser == null)
                {
                    logger.LogError(AuthenticationServerLogEvents.AuthenticatedUserMissingError, AuthenticatedUserMissingErrorMessage);
                    return CreateCurrentUserNotFoundErrorResult();
                }

                Guid? userIdToBeModified = model.UserId.ParseToGuid();

                if (!userIdToBeModified.IsAssigned())
                {
                    // this really shouldn't happen because the model is validated already
                    logger.LogWarning(AuthenticationServerLogEvents.ChangeUserInfo,
                        $"Failed to parse guid string '{model.UserId}' to Guid (user to be modified). Failing because cannot check if the modifying user {authUser.UserName} (Pete role) is in same organization as the user being modified.");
                    return RedirectToAction(nameof(AccountController.ChangeUserInfo), "Account");
                }
                if (!User.HasRole(UserRoleEnum.Eeva))
                {
                    // Pete role is not allowed to modify users in other organizations
                    if (!userOrganizationService.GetCoUsersOfUser(authUser.Id, roles[UserRoleEnum.Pete.ToString()]).Contains(userIdToBeModified.Value))
                    {
                        logger.LogWarning(AuthenticationServerLogEvents.ChangeUserInfo,
                            $"Access denied for user {authUser.UserName} (Pete role) to change user info for user id {model.UserId} because users are not in the same organization.");
                        return CreateAccessToActionDeniedResult();
                    }

                    var modifiedUserOrgs = userService.GetUserOrganizations(userIdToBeModified.Value);
                    var authUserOrgs = userService.GetUserOrganizations(authUser.Id).Where(i => i.Value == roles[UserRoleEnum.Pete.ToString()]);
                    var modelOrgs = model.Organizations;


                    var modifiedUserOrgsIds = modifiedUserOrgs.Select(i => i.Key).ToList();
                    var authUserOrgsIds = authUserOrgs.Select(i => i.Key).ToList();


                    var changedOrgs = modelOrgs.Cross(modifiedUserOrgs).Where(i => i.Value.Item1 != i.Value.Item2).ToList();
                    if (changedOrgs.Any(i => i.Value.Item1 == roles[UserRoleEnum.Eeva.ToString()] || i.Value.Item2 == roles[UserRoleEnum.Eeva.ToString()]) || changedOrgs.Any(i => !authUserOrgsIds.Contains(i.Key)))
                    {
                        logger.LogWarning(AuthenticationServerLogEvents.ChangeUserInfo,
                            $"Access denied for user {authUser.UserName} to change user info for user id {model.UserId} because user has insufficient access rights for this organization.");
                        return CreateAccessToActionDeniedResult();
                    }
                    var addedOrgs = modelOrgs.Keys.Except(modifiedUserOrgsIds).ToList();
                    if (addedOrgs.Any(i => modelOrgs[i] == roles[UserRoleEnum.Eeva.ToString()]) || addedOrgs.Any(i => !authUserOrgsIds.Contains(i)))
                    {
                        logger.LogWarning(AuthenticationServerLogEvents.ChangeUserInfo,
                            $"Access denied for user {authUser.UserName} to change user info for user id {model.UserId} because user has insufficient access rights for this organization.");
                        return CreateAccessToActionDeniedResult();
                    }
                    var userToChange = userManager.FindByIdAsync(userIdToBeModified.Value.ToString()).Result;
                    if (userToChange.Email != model.Email)
                    {
                        if (!modifiedUserOrgs.Keys.Intersect(authUserOrgsIds).Any())
                        {
                            logger.LogWarning(AuthenticationServerLogEvents.ChangeUserInfo,
                                $"Access denied for user {authUser.UserName} to change user's email (user id: '{model.UserId}', e-mail: {userToChange.Email}).");
                            return CreateAccessToActionDeniedResult("You are not allowed to change user's email.");
                        }
                    }
                }

                var result = userService.SaveUserInfo(model.UserId, model.Organizations, new StsUser() {UserName = model.Email, Email = model.Email, Name = model.FullName}).Result;

                // get organization role mapping ids as string
                string orgRoleMappingMessage = GetOrganizationRoleMappingString(model.Organizations);

                if (result.Succeeded)
                {
                    logger.LogChangeUserInfo($"User {authUser.UserName} changed user's {model.UserId} information. New information UserName: {model.Email}, Email: {model.Email}, FullName: {model.FullName} and organization role mappings: {orgRoleMappingMessage}.");
                    return RedirectToAction(nameof(AccountController.ChangeUserInfo), "Account", new {model.UserId});
                }

                string changeErrors = GetIdentityErrors(result.Errors);
                logger.LogError(AuthenticationServerLogEvents.ChangeUserInfo, $"User {authUser.UserName} tried to change information for user {model.UserId} but the operation failed with errors: {changeErrors}. New information used for the user: UserName: {model.Email}, Email: {model.Email}, FullName: {model.FullName} and organization role mappings: {orgRoleMappingMessage}.");

                AddErrors(result);
            }
            return RedirectToAction(nameof(AccountController.ChangeUserInfo), "Account");
        }

        [HttpGet("SetUserAccessRights")]
        [AuthorizeRoles(UserRoleEnum.Eeva)]
        public async Task<IActionResult> SetUserAccessRights(string userId = "")
        {
            var allUsers = userService.GetAllUsers();
            var currentUser = GetCurrentUser();

            // NOTE: Uncomment if Pete role will be allowed to call this method and list only his organizations users access rights
            //if (User.HasRole(UserRoleEnum.Pete))
            //{
            //    var coUsers = userOrganizationService.GetCoUsersOfUser(currentUser.Id);
            //    allUsers = allUsers.Where(x => coUsers.Contains(x.Id)).ToList();
            //}

            if (string.IsNullOrEmpty(userId))
            {
                var user = allUsers.FirstOrDefault()?.Id;
                if (!user.IsAssigned()) Redirect("/");
                return await SetUserAccessRightsSelect(user.Value.ToString(), allUsers);
            }

            return await SetUserAccessRightsSelect(userId, allUsers);
        }

        [HttpPost("SetUserAccessRights")]
        [AuthorizeRoles(UserRoleEnum.Eeva)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetUserAccessRights(SetUserAccessRightsViewModel model)
        {
            if (!ModelState.IsValid) return await SetUserAccessRights();

            model.SelectedAccessRights = model.SelectedAccessRights ?? new List<string>();

            var currentUser = GetCurrentUser();


            var allUsers = userService.GetAllUsers();
            

            var userId = model.UserId.ParseToGuid();
            if (userId == null) return await SetUserAccessRights();
            if (allUsers.All(x => x.Id != userId.Value)) return await SetUserAccessRights();

            var user = await userManager.FindByIdAsync(model.UserId);

            if (user != null)
            {
                if (!user.IsBuiltInEeva())
                {
                    var claims = await userManager.GetClaimsAsync(user);
                    var accessRightClaim = claims.FirstOrDefault(x => x.Type.ToLower() == PtvClaims.UserAccessRights.ToLower());
                    if (accessRightClaim != null)
                    {
                        logger.LogInformation(AuthenticationServerLogEvents.SetAccessRightsForUser, $"Removing existing access right claim type '{accessRightClaim.Type}' with value '{accessRightClaim.Value}' before storing new access rights.");
                        await userManager.RemoveClaimAsync(user, accessRightClaim);
                    }

                    var accessRights = string.Join(",",
                        model.SelectedAccessRights.Select(i => i.ToLower()).ToList()
                        .Intersect(Enum.GetValues(typeof(AccessRightEnum)).Cast<AccessRightEnum>().Select(i => i.ToString().ToLower()))
                        .ToList()
                        );

                    logger.LogInformation(AuthenticationServerLogEvents.SetAccessRightsForUser, $"User '{currentUser?.UserName}' is setting new access rights for user '{user.UserName}'. New access rights are: '{accessRights}'.");
                    await userManager.AddClaimAsync(user, new Claim(PtvClaims.UserAccessRights, accessRights));
                    logger.LogInformation(AuthenticationServerLogEvents.SetAccessRightsForUser, $"New access rights set '{accessRights}' for user '{user.UserName}'.");

                    return await SetUserAccessRightsSelect(user.Id.ToString(), allUsers);
                }
                else
                {
                    logger.LogWarning(AuthenticationServerLogEvents.SetAccessRightsForUser, $"User '{currentUser?.UserName}' tried to change access rights of built-in account '{Constants.BuiltInEeva.UserName}' which is not prohibited.");
                }
            }

            return await SetUserAccessRights();
        }

        [HttpPost("SetUserAccessRightsSelect")]
        [AuthorizeRoles(UserRoleEnum.Eeva)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetUserAccessRightsSelect(string userId, IList<StsUser> allUsers)
        {
            if (string.IsNullOrEmpty(userId)) return await SetUserAccessRights();
            if (allUsers == null || allUsers.Count == 0) return await SetUserAccessRights(userId);

            var user = await userManager.FindByIdAsync(userId);

            if (user != null && !user.IsBuiltInEeva())
            {
                var claims = await userManager.GetClaimsAsync(user);
                var accessRightClaim = claims.FirstOrDefault(x => x.Type.ToLower() == PtvClaims.UserAccessRights.ToLower());

                var users = allUsers.Select(u => new SelectListItem() {Value = u.Id.ToString(), Text = u.Email, Selected = u.Id.ToString() == userId}).ToList();
                var accessRights = Enum.GetValues(typeof(AccessRightEnum)).Cast<AccessRightEnum>().Select(i => i.ToString().ToLower()).Select(i => new SelectListItem()
                {
                    Selected = accessRightClaim == null || (accessRightClaim.Value ?? string.Empty).Contains(i),
                    Disabled = false,
                    Text = i,
                    Value = i
                }).ToList();
                var result = new SetUserAccessRightsViewModel() {AccessRights = accessRights, Users = users};
                return View("SetAccessRightsToUser", result);
            }

            return await SetUserAccessRights();
        }

        [HttpGet("ResetPasswordForUser")]
        [AuthorizeRoles(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        public IActionResult ResetPasswordForUser()
        {
            bool isEeva = User.HasRole(UserRoleEnum.Eeva);

            // change in how user is connected to an organization, so it is possible to have old accounts that don't have connection to an organization
            bool isAssignedToOrganization = userService.IsUserAssignedToOrganization(User.Identity.Name);

            // Pete and not assigned to organization, cannot reset password for anyone
            if (!isEeva && !isAssignedToOrganization)
            {
                return View("GeneralError", ChPassForUserPeteNotConnectedMessage);
            }

            var result = userService.GetAllUsers();

            // filter for Pete role
            if (!isEeva)
            {
                var role = roleManager.FindByNameAsync(UserRoleEnum.Pete.ToString()).Result;

                // get co-users of the current user from organizations where current user has pete role
                var allowedUsers = userService.GetCoUsersForUserName(User.Identity.Name, role.Id);

                // Pete roled user cannot reset password for Eeva roles used but that is not checked for the listing
                // resetting password does the check for the user (so list will show Eeva roled users even though Pete cannot reset the password for the user)
                result = result.Where(i => allowedUsers.Contains(i.Id)).ToList();
            }

            var model = new ResetPasswordForUserViewModel()
            {
                Users = result.Select(user => new SelectListItem() { Value = user.Id.ToString(), Text = user.Email }).ToList(),
            };

            return View(model);
        }

        [HttpPost("ResetPasswordForUser")]
        [AuthorizeRoles(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordForUser(ResetPasswordForUserViewModel model)
        {
            if (!ModelState.IsValid || (!User.HasRole(UserRoleEnum.Eeva) && !userService.IsUserAssignedToOrganization(User.Identity.Name))) return ResetPasswordForUser();

            var userToBeModified = await userManager.FindByIdAsync(model.UserId);

            if (userToBeModified != null)
            {
                if (!userToBeModified.IsBuiltInEeva())
                {
                    // if current user is Pete roled, then check that the user to be modified doesn't have Eeva role and also that the user is in same org as currentuser
                    if (User.HasRole(UserRoleEnum.Pete))
                    {
                        var peteRole = roleManager.FindByNameAsync(UserRoleEnum.Pete.ToString()).Result;

                        if (peteRole == null)
                        {
                            // this really shouldn't happen
                            logger.LogError(AuthenticationServerLogEvents.General, $"Pete role not found from role definitions (RoleManager.FindByNameAsync({UserRoleEnum.Pete})).");
                            return View("GeneralError", "System required Pete role missing from definitions. Please contact system support with this error message.");
                        }

                        // users in same organization as current user
                        var sameOrgUsers = userService.GetCoUsersForUserName(User.Identity.Name, peteRole.Id);

                        if (!sameOrgUsers.Contains(userToBeModified.Id))
                        {
                            // passed in user id is not in current users organization(s) users, access denied
                            logger.LogWarning(AuthenticationServerLogEvents.ChangePasswordForUser, $"User '{User.Identity.Name}' with Pete role tried to reset password for username '{userToBeModified.UserName}' (user id: {model.UserId}) who is not in their organization.");
                            return CreateAccessToActionDeniedResult(PeteTriedToResetPasswordForUserOutsideHisOrganization);
                        }

                        var usrOrgsAndRoles = userOrganizationService.GetUserOrganizationsWithRoles(userToBeModified.Id);

                        if (usrOrgsAndRoles != null && usrOrgsAndRoles.Count > 0)
                        {
                            // user to be modified has connected organizations and roles, so check if there is Eeva role in any organization

                            var eevaRole = roleManager.FindByNameAsync(UserRoleEnum.Eeva.ToString()).Result;

                            if (eevaRole == null)
                            {
                                // this really shouldn't happen
                                logger.LogError(AuthenticationServerLogEvents.General, $"Eeva role not found from role definitions (RoleManager.FindByNameAsync({UserRoleEnum.Eeva})).");
                                return View("GeneralError", "System required Eeva role missing from definitions. Please contact system support with this error message.");
                            }

                            if (usrOrgsAndRoles.Any(or => or.RoleId == eevaRole.Id))
                            {
                                // Pete roled user cannot reset password for a user that has Eeva role
                                logger.LogWarning(AuthenticationServerLogEvents.ChangePasswordForUser, $"User '{User.Identity.Name}' with Pete role tried to reset password for username '{userToBeModified.UserName}' (user id: {model.UserId}) who has Eeva role. Pete roled users cannot reset password for users with Eeva role.");
                                return CreateAccessToActionDeniedResult(PeteTriedToResetPasswordForEevaMessage);
                            }
                        }
                    }

                    var token = await userManager.GeneratePasswordResetTokenAsync(userToBeModified);

                    string rndPass = CreatePassword();

                    var result = await userManager.ResetPasswordAsync(userToBeModified, token, rndPass);

                    if (result.Succeeded)
                    {
                        logger.LogResetPasswordForUser($"User '{User.Identity.Name}' resetted password for username '{userToBeModified.UserName}' (user id: {model.UserId}).");

                        ViewData["newpass"] = rndPass;
                        // view name is the same as the action name
                        return View(nameof(AccountController.ResetPasswordConfirmation));
                    }
                    else
                    {
                        string errors = GetIdentityErrors(result.Errors);
                        logger.LogError(AuthenticationServerLogEvents.ChangePasswordForUser, $"User '{User.Identity.Name}' tried to reset password for username '{userToBeModified.UserName}' (user id: {model.UserId}) but the operation failed. Operation errors: {errors}");
                        return View("GeneralError", $"Salasanan resetointi epäonnistui ({errors}).");
                    }
                }
                else
                {
                    // not allowed to reset password for eevaadmin account
                    logger.LogWarning(AuthenticationServerLogEvents.ChangePasswordForUser, $"User '{User.Identity.Name}' tried to reset password for built-in user '{Constants.BuiltInEeva.UserName}' which is not prohibited.");
                    return CreateAccessToActionDeniedResult();
                }
            }

            // user not found
            logger.LogWarning(AuthenticationServerLogEvents.ChangePasswordForUser, $"User '{User.Identity.Name}' tried to reset password for user id '{model.UserId}' but user with the id was not found from the system.");
            return View("GeneralError");
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet("ResetPasswordConfirmation")]
        [AuthorizeRoles(UserRoleEnum.Eeva, UserRoleEnum.Pete)]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet("LogOff")]
        //[HttpGet(Constants.RoutePaths.Logout, Name = "Logout")]
        public IActionResult LogOff(string id)
        {
            return View(new LogoutViewModel { SignOutId = id });
        }

        [HttpPost("LogOff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            // first log message that we are logging out so that we get entry to log automatically with the logged in user
            logger.LogInformation("User logging out.");

            await signInManager.SignOutAsync();

            await HttpContext.SignOutAsync();

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // and then log message after ok logout (here we don't know anymore who the user was)
            logger.LogInformation("User logged out.");

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        
        // This method should be protected by the controller Authorize attribute, no role required to change password
        // GET: /Account/ChangePassword
        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // This method should be protected by the controller Authorize attribute, no role required to change password
        // POST: /Account/ChangePassword
        [HttpPost("ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = GetCurrentUser();

            if (user == null)
            {
                // NOTE: Currently the user will be null if the user has logged in to PTV application and then just browses to sts address to change his/her password
                // this is because when user logs to PTV application the claim type http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier is set
                // and that value is used to load the user and that is now missing
                logger.LogError(AuthenticationServerLogEvents.AuthenticatedUserMissingError, AuthenticatedUserMissingErrorMessage);
                return CreateCurrentUserNotFoundErrorResult();
            }

            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);

            if (result.Succeeded)
            {
                logger.LogInformation(AuthenticationServerLogEvents.ChangePassword, $"User '{user.UserName}' changed their password.");

                return RedirectToAction(nameof(AccountController.ChangePasswordConfirmation), "Account");
            }

            string chpwdErrors = GetIdentityErrors(result.Errors);
            logger.LogWarning(AuthenticationServerLogEvents.ChangePassword, $"User '{user.UserName}' tried to change their password but it failed with errors: {chpwdErrors}");

            AddErrors(result);
            return View();
        }

        // This method should be protected by the controller Authorize attribute, no role required to change password
        // GET: /Account/ResetPasswordConfirmation
        [AuthorizeRoles(UserRoleEnum.Eeva, UserRoleEnum.Pete, UserRoleEnum.Shirley)]
        [HttpGet("ChangePasswordConfirmation")]
        public IActionResult ChangePasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Gets the authenticated users StsUser object.
        /// </summary>
        /// <returns>StsUser object or null if the user was not found</returns>
        private StsUser GetCurrentUser()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                logger.LogWarning(AuthenticationServerLogEvents.General, "No authenticated user. This might indicate that the method was called from context where there doesn't need to be an authenticated user present.");
                return null;
            }

            var userIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdentifier == null)
            {
                logger.LogWarning(AuthenticationServerLogEvents.General, "No authenticated useridentifier found from HttpContext.User property claims collection.");
                return null;
            }

            var user = userManager.FindByIdAsync(userIdentifier).Result;

            if (user == null)
            {
                logger.LogError(AuthenticationServerLogEvents.AuthenticatedUserMissingError, $"Authenticated user with id '{userIdentifier}' was not found.");
            }

            return user;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        /// <summary>
        /// Concatenates IdentityError description and code properties to a string separated with colon.
        /// </summary>
        /// <param name="errors">IEnumerable{IdentityError}</param>
        /// <returns>null if errors is null otherwise a string containing the error descriptions and codes</returns>
        private static string GetIdentityErrors(IEnumerable<IdentityError> errors)
        {
            if (errors == null)
            {
                return null;
            }

            return string.Join(", ", errors.Select(x => $"{x.Description} (Code: {x.Code})"));
        }

        private static string GetOrganizationRoleMappingString(Dictionary<Guid, Guid> orgRoleMapping)
        {
            if (orgRoleMapping == null || orgRoleMapping.Count == 0)
            {
                return null;
            }

            return string.Join(", ", orgRoleMapping.Select(x => $"Organization id: {x.Key} and role id: {x.Value}"));
        }

        private const string PasswordAlphabets = "AaBbCcDdEeFfGgHhJjKkLMmNPpQqRrSsTtUuVvWwXxYyZz";
        private const string PasswordNumbers = "23456789";
        private const string PasswordSpecialCharacters = "+-!*@&?";

        private static string CreatePassword()
        {
            Random rnd = new Random();
            int alphasMaxIndex = PasswordAlphabets.Length - 1;

            // this is not trying to be a secure password generator
            // password length needs to be 11 characters
            // will create a password 9 alphas + number + special
            char[] passChars = new char[11];
            int passCharIdx = 0;

            // get 9 alphas
            for (int i = 0; i < 9; i++)
            {
                passChars[passCharIdx] = PasswordAlphabets[rnd.Next(0, alphasMaxIndex)];
                passCharIdx++;
            }

            // get number and increase output array character index
            passChars[passCharIdx++] = PasswordNumbers[rnd.Next(0, PasswordNumbers.Length - 1)];

            // get special char
            passChars[passCharIdx] = PasswordSpecialCharacters[rnd.Next(0, PasswordSpecialCharacters.Length - 1)];

            return new string(passChars);
        }
//        private Dictionary<string, string> GetTranslatedRoles()
//        {
//            return new Dictionary<string, string>()
//            {
//                { UserRoleEnum.Eeva, localizer["sts_roleEeva"] },
//                { UserRoleEnum.Pete, localizer["sts_rolePete"] },
//                { UserRoleEnum.Shirley, localizer["sts_roleShirley"] }
//            };
//        }
    }

    public static class StsExtensions
    {
        public static Guid? ParseToGuid(this string str)
        {
            Guid id;
            if (Guid.TryParse(str, out id) && id != Guid.Empty)
            {
                return id;
            }
            return null as Guid?;
        }
    }
}
