using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BoardMan.Web.Controllers
{
	public class ListsController : SiteControllerBase
	{
        private readonly IListManager listManager;

		public ListsController(UserManager<AppUser> userManager, IConfiguration configuration, ILogger<ListsController> logger, IStringLocalizer<SharedResource> sharedLocalizer, IListManager listManager) : base(userManager, configuration, logger, sharedLocalizer)
		{
			this.listManager = listManager;
		}

        [HttpPost]
        public async Task<ActionResult> GetListsByBoardId(Guid boardId)
        {
            return JsonResponse(ApiResponse.List(await this.listManager.GetListsAsync(boardId)));
        }

        [HttpPost]
        public async Task<ActionResult> CreateList(List list)
        {
            if (ModelState.IsValid)
            {
                var record = await this.listManager.CreateListAsync(list);
                return JsonResponse(ApiResponse.Single(record));
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }

        [HttpPost]
        public async Task<ActionResult> UpdateList(List list)
        {
            if (ModelState.IsValid)
            {
                var record = await this.listManager.UpdateListAsync(list);
                return JsonResponse(ApiResponse.Single(record));
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteList(Guid listId)
        {
            if (ModelState.IsValid)
            {
                await this.listManager.DeleteListAsync(listId);
                return JsonResponse(ApiResponse.Success());
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }
    }
}
