using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Converters;
using BoardMan.Web.Infrastructure.Utils;
using BoardMan.Web.Models;
using BoardMan.Web.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BoardMan.Web.Controllers
{
	public abstract class SiteControllerBase : Controller
	{		
		protected readonly UserManager<AppUser> userManager;
		protected readonly IConfiguration configuration;
		protected readonly ILogger logger;
        protected readonly IStringLocalizer<SharedResource> sharedLocalizer;
        protected readonly IAuthorizationService authorizationService;

        protected SiteControllerBase(UserManager<AppUser> userManager, IAuthorizationService authorizationService, IConfiguration configuration, ILogger logger, IStringLocalizer<SharedResource> sharedLocalizer)
		{
			this.userManager = userManager;
			this.configuration = configuration;
			this.logger = logger;
			this.sharedLocalizer = sharedLocalizer;
            this.authorizationService = authorizationService;
        }

        protected ActionResult RedirectWithMessage(string actionName, string controllerName, string message)
        {
            TempData["ServerSideMessage"] = message;
            return RedirectToAction(actionName, controllerName);
        }

        protected ActionResult RedirectWithMessage(string url, string message)
        {
            TempData["ServerSideMessage"] = message;
            return Redirect(url);
        }

        #region Json methods

        protected virtual ActionResult JsonResponse(object data)
        {
            // Retrieves the requested culture
            var requestFeature = Request.HttpContext.Features.Get<IRequestCultureFeature>();            

            var content = JsonConvert.SerializeObject(data,
                new JsonSerializerSettings
                {
                    Converters = new JsonConverter[] { new FormattedDateTimeZoneConverter(requestFeature.RequestCulture.Culture), new StringEnumConverter() },
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            return Content(content, "application/json; charset=utf-8");
        }

        protected virtual async Task<ActionResult> SecureJsonActionAsync(Func<Task<ActionResult>> method)
        {
            try
            {
                return await method();
            }
            catch (DbUpdateException efEx)
            {
                logger.LogError(efEx.GetBaseException() ?? efEx, efEx.Message);

                if (efEx.GetBaseException() is SqlException ex && new List<int> { (int)SqlErrors.KeyViolation, (int)SqlErrors.UniqueIndex }.Contains(ex.Number))
                {
                    return Json(ApiResponse.Error(Messages.CannotAddDuplicateRecord));
                }

                
                return Json(ApiResponse.Error(Messages.UnexpectedInput));
            }
			catch (InsufficientDataToProcessException ex)
			{
				logger.LogError(ex, ex.Message);
                return Json(ApiResponse.Error(ex.Message));
            }
			catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Json(ApiResponse.Error(Messages.UnexpectedInput));
            }
        }

        protected async Task<ActionResult> GetJsonAsync(Func<Task<ApiResponse>> method)
        {
            return await SecureJsonActionAsync(async () =>
            {
                var response = await method();
                if (!response.Succeeded)
                {
                    logger.LogError(JsonConvert.SerializeObject(response));
                }

                return Json(response);
            });
        }

        protected virtual async Task<ActionResult> AuthorizedResposeAsync(Func<Task<ActionResult>> method, object? resource, string policyName)
		{
            var authorizationResult = await this.authorizationService.AuthorizeAsync(User, resource, policyName);

            if (authorizationResult.Succeeded)
            {
                return await method();
            }
            else if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }
            else
            {
                return new ChallengeResult();
            }            
        }

        protected virtual async Task<ActionResult> AuthorizedJsonResposeAsync(Func<Task<ActionResult>> method, object? resource, string policyName)
        {
            var authorizationResult = await this.authorizationService.AuthorizeAsync(User, resource, policyName);

            if (authorizationResult.Succeeded)
            {
                return await method();
            }
            else
            {
                return JsonResponse(ApiResponse.Error("You are not authorized to perform this action"));
            }
        }

        protected virtual async Task<ActionResult> AuthorizedJsonAsync(Func<Task<ActionResult>> method, object? resource, string policyName)
        {
            var authorizationResult = await this.authorizationService.AuthorizeAsync(User, resource, policyName);

            if (authorizationResult.Succeeded)
            {
                return await method();
            }
            else
            {
                return Json(ApiResponse.Error("You are not authorized to perform this action"));
            }
        }

        #endregion
    }

	public class SharedResource
	{
	}
}
