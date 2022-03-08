using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils;
using BoardMan.Web.Models;
using BoardMan.Web.Resources;
using Microsoft.AspNetCore.Identity;
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

		protected SiteControllerBase(UserManager<AppUser> userManager, IConfiguration configuration, ILogger logger, IStringLocalizer<SharedResource> sharedLocalizer)
		{
			this.userManager = userManager;
			this.configuration = configuration;
			this.logger = logger;
			this.sharedLocalizer = sharedLocalizer;
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
            var content = JsonConvert.SerializeObject(data,
                new JsonSerializerSettings
                {
                    Converters = new JsonConverter[] { new StringEnumConverter() },
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            return Content(content,
                "application/json; charset=utf-8");
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

                return JsonResponse(response);
            });
        }

        #endregion
    }

	public class SharedResource
	{
	}
}
