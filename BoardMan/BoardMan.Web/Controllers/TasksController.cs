using BoardMan.Web.Auth;
using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using BoardMan.Web.Infrastructure.Utils;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Authorization;
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

		public TasksController(UserManager<DbAppUser> userManager, IAuthorizationService authorizationService, IConfiguration configuration, ILogger<TasksController> logger, IStringLocalizer<SharedResource> sharedLocalizer, ITaskManager taskManager, IBlobManager uploadService) : base(userManager, authorizationService, configuration, logger, sharedLocalizer)
		{
			this.taskManager = taskManager;
			this.uploadService = uploadService;
		}

		[HttpPost]
        public async Task<ActionResult> GetTasksByListId(Guid listId)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                return JsonResponse(ApiResponse.List(await this.taskManager.GetTasksAsync(listId)));
            }, new EntityResource { Id = listId, Type = EntityType.List }, Policies.BoardReaderPolicy);            
        }

        [HttpPost]
        public async Task<ActionResult> CreateTask(BoardTask task)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    var record = await this.taskManager.CreateTaskAsync(task);
                    return JsonResponse(ApiResponse.Single(record));
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = task.ListId, Type = EntityType.List }, Policies.BoardContributorPolicy);            
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTask(BoardTask task)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    var record = await this.taskManager.UpdateTaskAsync(task);
                    return JsonResponse(ApiResponse.Single(record));
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = task.Id, Type = EntityType.Task }, Policies.BoardContributorPolicy);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTask(Guid id)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    await this.taskManager.DeleteTaskAsync(id);
                    return JsonResponse(ApiResponse.Success());
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = id, Type = EntityType.Task }, Policies.BoardContributorPolicy);
        }

        [HttpPost]
        public async Task<ActionResult> GetTaskComments(Guid taskId)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                return JsonResponse(ApiResponse.List(await this.taskManager.GetTaskCommentsAsync(taskId)));
            }, new EntityResource { Id = taskId, Type = EntityType.Task }, Policies.BoardReaderPolicy);            
        }

        [HttpPost]
        public async Task<ActionResult> CreateTaskComment(TaskComment taskComment)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    taskComment.CommentedById = this.userManager.GetGuidUserId(User);
                    var record = await this.taskManager.CreateTaskCommentAsync(taskComment);
                    return JsonResponse(ApiResponse.Single(record));
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = taskComment.TaskId, Type = EntityType.Task }, Policies.BoardReaderPolicy);            
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTaskComment(TaskComment taskComment)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    var record = await this.taskManager.UpdateTaskCommentAsync(taskComment);
                    return JsonResponse(ApiResponse.Single(record));
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = taskComment.Id, Type = EntityType.TaskComment }, Policies.BoardReaderPolicy);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTaskComment(Guid id)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    await this.taskManager.DeleteTaskCommentAsync(id);
                    return JsonResponse(ApiResponse.Success());
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = id, Type = EntityType.TaskComment }, Policies.BoardReaderPolicy);
           
        }

        [HttpPost]
        public async Task<ActionResult> GetTaskLabels(Guid taskId)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                return JsonResponse(ApiResponse.List(await this.taskManager.GetTaskLabelsAsync(taskId)));
            }, new EntityResource { Id = taskId, Type = EntityType.Task }, Policies.BoardReaderPolicy);            
        }

        [HttpPost]
        public async Task<ActionResult> CreateTaskLabel(TaskLabel taskLabel)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    var record = await this.taskManager.CreateTaskLabelAsync(taskLabel);
                    return JsonResponse(ApiResponse.Single(record));
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = taskLabel.TaskId, Type = EntityType.Task }, Policies.BoardContributorPolicy);
            
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTaskLabel(TaskLabel taskLabel)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    var record = await this.taskManager.UpdateTaskLabelAsync(taskLabel);
                    return JsonResponse(ApiResponse.Single(record));
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = taskLabel.TaskId, Type = EntityType.Task }, Policies.BoardContributorPolicy);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTaskLabel(Guid id)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    await this.taskManager.DeleteTaskLabelAsync(id);
                    return JsonResponse(ApiResponse.Success());
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = id, Type = EntityType.Tasklabel }, Policies.BoardContributorPolicy);            
        }

        [HttpPost]
        public async Task<ActionResult> GetTaskChecklists(Guid taskId)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                return JsonResponse(ApiResponse.List(await this.taskManager.GetTaskChecklistsAsync(taskId)));
            }, new EntityResource { Id = taskId, Type = EntityType.Task }, Policies.BoardReaderPolicy);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTaskChecklist(TaskChecklist taskChecklist)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    taskChecklist.CreatedById = this.userManager.GetGuidUserId(User);
                    var record = await this.taskManager.CreateTaskChecklistAsync(taskChecklist);
                    return JsonResponse(ApiResponse.Single(record));
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = taskChecklist.TaskId, Type = EntityType.Task }, Policies.BoardContributorPolicy);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTaskChecklist(TaskChecklist taskChecklist)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    var record = await this.taskManager.UpdateTaskChecklistAsync(taskChecklist);
                    return JsonResponse(ApiResponse.Single(record));
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = taskChecklist.Id, Type = EntityType.TaskChecklist }, Policies.BoardContributorPolicy);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTaskChecklist(Guid id)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    await this.taskManager.DeleteTaskChecklistAsync(id);
                    return JsonResponse(ApiResponse.Success());
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = id, Type = EntityType.TaskChecklist }, Policies.BoardContributorPolicy);
        }

        [HttpPost]
        public async Task<ActionResult> GetTaskWatchers(Guid taskId)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                return JsonResponse(ApiResponse.List(await this.taskManager.GetTaskWatchersAsync(taskId)));
            }, new EntityResource { Id = taskId, Type = EntityType.Task }, Policies.BoardReaderPolicy);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTaskWatcher(TaskWatcher taskWatcher)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    var record = await this.taskManager.CreateTaskWatcherAsync(taskWatcher);
                    return JsonResponse(ApiResponse.Single(record));
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = taskWatcher.TaskId, Type = EntityType.Task }, Policies.BoardContributorPolicy);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTaskWatcher(Guid id)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    await this.taskManager.DeleteTaskWatcherAsync(id);
                    return JsonResponse(ApiResponse.Success());
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = id, Type = EntityType.TaskWatcher }, Policies.BoardContributorPolicy);
        }

		[HttpPost]
		public async Task<ActionResult> GetTaskAttachments(Guid taskId)
		{
            return await AuthorizedJsonResposeAsync(async () => {
                return JsonResponse(ApiResponse.List(await this.taskManager.GetTaskAttachmentsAsync(taskId)));
            }, new EntityResource { Id = taskId, Type = EntityType.Task }, Policies.BoardReaderPolicy);            
		}

		[HttpPost]
		public async Task<ActionResult> CreateTaskAttachment(TaskAttachment taskAttachment)
		{
            return await AuthorizedJsonResposeAsync(async () => {
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
            }, new EntityResource { Id = taskAttachment.TaskId, Type = EntityType.Task }, Policies.BoardContributorPolicy);
		}

		[HttpPost]
		public async Task<ActionResult> DeleteTaskAttachment(Guid id)
		{
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    var taskAttachment = await this.taskManager.GetTaskAttachmentAsync(id);
                    await this.uploadService.DeleteAsync(taskAttachment.TrustedFileName);
                    await this.taskManager.DeleteTaskAttachmentAsync(id);
                    return JsonResponse(ApiResponse.Success());
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = id, Type = EntityType.TaskAttachment, Action = UserAction.Delete }, Policies.BoardContributorPolicy);
		}
                
        public async Task<ActionResult> DownloadTaskAttachment(Guid id)
        {
            return await AuthorizedJsonResposeAsync(async () => {
                if (ModelState.IsValid)
                {
                    var taskAttachment = await this.taskManager.GetTaskAttachmentAsync(id);
                    var downloadedBlob = await this.uploadService.DownloadAsync(taskAttachment.TrustedFileName);
                    return File(downloadedBlob.Item1, downloadedBlob.Item2, downloadedBlob.Item3);
                }

                return JsonResponse(ApiResponse.Error(ModelState.Errors()));
            }, new EntityResource { Id = id, Type = EntityType.TaskAttachment }, Policies.BoardReaderPolicy);
        }
    }
}
