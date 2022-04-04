using BoardMan.Web.Auth;
using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BoardMan.Web.Controllers
{
	public class ListsController : SiteControllerBase
	{
        private readonly IListManager listManager;

		public ListsController(UserManager<AppUser> userManager, IAuthorizationService authorizationService, IConfiguration configuration, ILogger<ListsController> logger, IStringLocalizer<SharedResource> sharedLocalizer, IListManager listManager) : base(userManager, authorizationService, configuration, logger, sharedLocalizer)
		{
			this.listManager = listManager;
		}

        [HttpPost]
        public async Task<ActionResult> GetListsByBoardId(Guid boardId)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                return JsonResponse(ApiResponse.List(await this.listManager.GetListsAsync(boardId)));
            }, new EntityResource { Id = boardId, Type = EntityType.Board }, Policies.BoardReaderPolicy );            
        }

        [HttpPost]
        public async Task<ActionResult> CreateList(List list)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    var record = await this.listManager.CreateListAsync(list);
                    return JsonResponse(ApiResponse.Single(record));
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = list.BoardId, Type = EntityType.Board }, Policies.BoardContributorPolicy);            
        }

        [HttpPost]
        public async Task<ActionResult> UpdateList(List list)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    var record = await this.listManager.UpdateListAsync(list);
                    return JsonResponse(ApiResponse.Single(record));
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = list.BoardId, Type = EntityType.Board }, Policies.BoardContributorPolicy);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteList(Guid id)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    await this.listManager.DeleteListAsync(id);
                    return JsonResponse(ApiResponse.Success());
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = id, Type = EntityType.List }, Policies.BoardContributorPolicy);
        }
    }
}
