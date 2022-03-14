﻿using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BoardMan.Web.Controllers
{
	public class TasksController : SiteControllerBase
	{
        private readonly ITaskManager taskManager;

		public TasksController(UserManager<AppUser> userManager, IConfiguration configuration, ILogger<TasksController> logger, IStringLocalizer<SharedResource> sharedLocalizer, ITaskManager taskManager) : base(userManager, configuration, logger, sharedLocalizer)
		{
			this.taskManager = taskManager;
		}

		[HttpPost]
        public async Task<ActionResult> GetTasksByListId(Guid listId)
        {
            return JsonResponse(ApiResponse.List(await this.taskManager.GetTasksAsync(listId)));
        }

        [HttpPost]
        public async Task<ActionResult> CreateTask(BoardTask task)
        {
            if (ModelState.IsValid)
            {
                var record = await this.taskManager.CreateTaskAsync(task);
                return JsonResponse(ApiResponse.Single(record));
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTask(BoardTask task)
        {
            if (ModelState.IsValid)
            {
                var record = await this.taskManager.UpdateTaskAsync(task);
                return JsonResponse(ApiResponse.Single(record));
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTask(Guid id)
        {
            if (ModelState.IsValid)
            {
                await this.taskManager.DeleteTaskAsync(id);
                return JsonResponse(ApiResponse.Success());
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }

        [HttpPost]
        public async Task<ActionResult> GetTaskComments(Guid taskId)
        {
            return JsonResponse(ApiResponse.List(await this.taskManager.GetTaskCommentsAsync(taskId)));
        }

        [HttpPost]
        public async Task<ActionResult> CreateTaskComment(TaskComment taskComment)
        {
            if (ModelState.IsValid)
            {
                taskComment.CommentedById = this.userManager.GetGuidUserId(User);
                var record = await this.taskManager.CreateTaskCommentAsync(taskComment);
                return JsonResponse(ApiResponse.Single(record));
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTaskComment(TaskComment taskComment)
        {
            if (ModelState.IsValid)
            {
                var record = await this.taskManager.UpdateTaskCommentAsync(taskComment);
                return JsonResponse(ApiResponse.Single(record));
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTaskComment(Guid id)
        {
            if (ModelState.IsValid)
            {
                await this.taskManager.DeleteTaskCommentAsync(id);
                return JsonResponse(ApiResponse.Success());
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }
    }
}