using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using BoardMan.Web.Infrastructure.Utils;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Net;

namespace BoardMan.Web.Controllers
{
	public class TasksController : SiteControllerBase
	{
        private readonly ITaskManager taskManager;
        private readonly IBlobManager uploadService;
        private readonly long _fileSizeLimit = 2097152;
        private readonly string[] _permittedExtensions = { ".png", ".jpg", ".jpeg" };

		public TasksController(UserManager<AppUser> userManager, IConfiguration configuration, ILogger<TasksController> logger, IStringLocalizer<SharedResource> sharedLocalizer, ITaskManager taskManager, IBlobManager uploadService) : base(userManager, configuration, logger, sharedLocalizer)
		{
			this.taskManager = taskManager;
			this.uploadService = uploadService;
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

        [HttpPost]
        public async Task<ActionResult> GetTaskLabels(Guid taskId)
        {
            return JsonResponse(ApiResponse.List(await this.taskManager.GetTaskLabelsAsync(taskId)));
        }

        [HttpPost]
        public async Task<ActionResult> CreateTaskLabel(TaskLabel taskLabel)
        {
            if (ModelState.IsValid)
            {
                var record = await this.taskManager.CreateTaskLabelAsync(taskLabel);
                return JsonResponse(ApiResponse.Single(record));
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTaskLabel(TaskLabel taskLabel)
        {
            if (ModelState.IsValid)
            {
                var record = await this.taskManager.UpdateTaskLabelAsync(taskLabel);
                return JsonResponse(ApiResponse.Single(record));
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTaskLabel(Guid id)
        {
            if (ModelState.IsValid)
            {
                await this.taskManager.DeleteTaskLabelAsync(id);
                return JsonResponse(ApiResponse.Success());
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }

        [HttpPost]
        public async Task<ActionResult> GetTaskChecklists(Guid taskId)
        {
            return JsonResponse(ApiResponse.List(await this.taskManager.GetTaskChecklistsAsync(taskId)));
        }

        [HttpPost]
        public async Task<ActionResult> CreateTaskChecklist(TaskChecklist taskChecklist)
        {
            if (ModelState.IsValid)
            {
                taskChecklist.CreatedById = this.userManager.GetGuidUserId(User);
                var record = await this.taskManager.CreateTaskChecklistAsync(taskChecklist);
                return JsonResponse(ApiResponse.Single(record));
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTaskChecklist(TaskChecklist taskChecklist)
        {
            if (ModelState.IsValid)
            {
                var record = await this.taskManager.UpdateTaskChecklistAsync(taskChecklist);
                return JsonResponse(ApiResponse.Single(record));
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTaskChecklist(Guid id)
        {
            if (ModelState.IsValid)
            {
                await this.taskManager.DeleteTaskChecklistAsync(id);
                return JsonResponse(ApiResponse.Success());
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }


        [HttpPost]
        public async Task<ActionResult> GetTaskWatchers(Guid taskId)
        {
            return JsonResponse(ApiResponse.List(await this.taskManager.GetTaskWatchersAsync(taskId)));
        }

        [HttpPost]
        public async Task<ActionResult> CreateTaskWatcher(TaskWatcher taskWatcher)
        {
            if (ModelState.IsValid)
            {                
                var record = await this.taskManager.CreateTaskWatcherAsync(taskWatcher);
                return JsonResponse(ApiResponse.Single(record));
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTaskWatcher(Guid id)
        {
            if (ModelState.IsValid)
            {
                await this.taskManager.DeleteTaskWatcherAsync(id);
                return JsonResponse(ApiResponse.Success());
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }

		[HttpPost]
		public async Task<ActionResult> GetTaskAttachments(Guid taskId)
		{
			return JsonResponse(ApiResponse.List(await this.taskManager.GetTaskAttachmentsAsync(taskId)));
		}

		[HttpPost]
		public async Task<ActionResult> CreateTaskAttachment(TaskAttachment taskAttachment)
		{
			if (ModelState.IsValid)
			{
                var formFileContentStream = await FileHelpers.ProcessFormFile<TaskAttachment>(
                   taskAttachment.File, ModelState, _permittedExtensions,
                   _fileSizeLimit);

                // Perform a second check to catch ProcessFormFile method
                // violations. If any validation check fails, return to the
                // page.
                if (!ModelState.IsValid || formFileContentStream == null)
                {
                    return JsonResponse(ApiResponse.Error(ModelState.Errors()));
                }

                taskAttachment.TrustedFileName = WebUtility.HtmlEncode(taskAttachment.File.FileName);
                taskAttachment.FileUri = await this.uploadService.UploadAsync(taskAttachment.File.OpenReadStream(), taskAttachment.TrustedFileName, taskAttachment.File.ContentType);
                taskAttachment.UploadedById = this.userManager.GetGuidUserId(User);

                var record = await this.taskManager.CreateTaskAttachmentAsync(taskAttachment);
				return JsonResponse(ApiResponse.Single(record));
			}

			return JsonResponse(ApiResponse.Error(ModelState.Errors()));
		}

		[HttpPost]
		public async Task<ActionResult> DeleteTaskAttachment(Guid id)
		{
			if (ModelState.IsValid)
			{
                var taskAttachment = await this.taskManager.GetTaskAttachmentAsync(id);
                await this.uploadService.DeleteAsync(taskAttachment.TrustedFileName);
                await this.taskManager.DeleteTaskAttachmentAsync(id);
				return JsonResponse(ApiResponse.Success());
			}

			return JsonResponse(ApiResponse.Error(ModelState.Errors()));
		}
                
        public async Task<ActionResult> DownloadTaskAttachment(Guid id)
        {
            if (ModelState.IsValid)
            {
                var taskAttachment = await this.taskManager.GetTaskAttachmentAsync(id);                
                var downloadedBlob = await this.uploadService.DownloadAsync(taskAttachment.TrustedFileName);
                return File(downloadedBlob.Item1, downloadedBlob.Item2, downloadedBlob.Item3);
            }

            return JsonResponse(ApiResponse.Error(ModelState.Errors()));
        }
    }
}
