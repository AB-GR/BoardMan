using AutoMapper;
using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils;
using BoardMan.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Managers
{
	public interface ITaskManager
	{
		Task<List<BoardTask>> GetTasksAsync(Guid listId);

		Task<BoardTask> CreateTaskAsync(BoardTask task);

		Task<BoardTask> UpdateTaskAsync(BoardTask task);

		Task DeleteTaskAsync(Guid listId);

		Task<List<TaskComment>> GetTaskCommentsAsync(Guid taskId);

		Task<TaskComment> CreateTaskCommentAsync(TaskComment taskComment);

		Task<TaskComment> UpdateTaskCommentAsync(TaskComment taskComment);
		
		Task DeleteTaskCommentAsync(Guid id);

		Task<List<TaskLabel>> GetTaskLabelsAsync(Guid taskId);

		Task<TaskLabel> CreateTaskLabelAsync(TaskLabel taskLabel);

		Task<TaskLabel> UpdateTaskLabelAsync(TaskLabel taskLabel);

		Task DeleteTaskLabelAsync(Guid id);

		Task<List<TaskChecklist>> GetTaskChecklistsAsync(Guid taskId);

		Task<TaskChecklist> CreateTaskChecklistAsync(TaskChecklist taskChecklist);

		Task<TaskChecklist> UpdateTaskChecklistAsync(TaskChecklist taskChecklist);

		Task DeleteTaskChecklistAsync(Guid id);


		Task<List<TaskWatcher>> GetTaskWatchersAsync(Guid taskId);

		Task<TaskWatcher> CreateTaskWatcherAsync(TaskWatcher taskWatcher);

		Task DeleteTaskWatcherAsync(Guid id);

		Task<List<TaskAttachment>> GetTaskAttachmentsAsync(Guid taskId);

		Task<TaskAttachment> GetTaskAttachmentAsync(Guid taskAttachmentId);

		Task<TaskAttachment> CreateTaskAttachmentAsync(TaskAttachment taskAttachment);

		Task DeleteTaskAttachmentAsync(Guid id);
	}

	public class TaskManager : ITaskManager
	{
		private readonly BoardManDbContext dbContext;
		private readonly IMapper mapper;

		public TaskManager(BoardManDbContext boardManDbContext, IMapper mapper)
		{
			this.dbContext = boardManDbContext;
			this.mapper = mapper;
		}

		public async Task<List<BoardTask>> GetTasksAsync(Guid listId)
		{
			var dbTasks = await this.dbContext.Tasks.Where(x => x.ListId == listId && x.DeletedAt == null).ToListAsync().ConfigureAwait(false);
			return this.mapper.Map<List<BoardTask>>(dbTasks);
		}

		public async Task<BoardTask> CreateTaskAsync(BoardTask task)
		{
			var dbTask = this.mapper.Map<DbTask>(task);
			this.dbContext.Tasks.Add(dbTask);
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
			return this.mapper.Map<BoardTask>(dbTask);
		}

		public async Task<BoardTask> UpdateTaskAsync(BoardTask task)
		{
			var dbTask = await this.dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == task.Id && x.DeletedAt == null).ConfigureAwait(false);
			if (dbTask == null)
			{
				throw new EntityNotFoundException($"Task with Id {task.Id} not found");
			}

			this.mapper.Map(task, dbTask);
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
			return this.mapper.Map<BoardTask>(dbTask);
		}

		public async Task DeleteTaskAsync(Guid taskId)
		{
			var dbTask = await this.dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == taskId && x.DeletedAt == null).ConfigureAwait(false);
			if (dbTask == null)
			{
				throw new EntityNotFoundException($"Task with Id {taskId} not found");
			}

			dbTask.DeletedAt = DateTime.UtcNow;
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
		}
		

		public async Task<List<TaskComment>> GetTaskCommentsAsync(Guid taskId)
		{
			var dbTaskComments = await this.dbContext.TaskComments.Where(x => x.TaskId == taskId && x.DeletedAt == null).Include(x => x.CommentedBy).ToListAsync().ConfigureAwait(false);
			return this.mapper.Map<List<TaskComment>>(dbTaskComments);
		}

		public async Task<TaskComment> CreateTaskCommentAsync(TaskComment taskComment)
		{
			var dbTaskComment = this.mapper.Map<DbTaskComment>(taskComment);
			this.dbContext.TaskComments.Add(dbTaskComment);
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
			return this.mapper.Map<TaskComment>(dbTaskComment);
		}

		public async Task<TaskComment> UpdateTaskCommentAsync(TaskComment taskComment)
		{
			var dbTaskComment = await this.dbContext.TaskComments.Include(x => x.CommentedBy).FirstOrDefaultAsync(x => x.Id == taskComment.Id && x.DeletedAt == null).ConfigureAwait(false);
			if (dbTaskComment == null)
			{
				throw new EntityNotFoundException($"TaskComment with Id {taskComment.Id} not found");
			}

			this.mapper.Map(taskComment, dbTaskComment);
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
			return this.mapper.Map<TaskComment>(dbTaskComment);
		}

		public async Task DeleteTaskCommentAsync(Guid id)
		{
			var dbTaskComment = await this.dbContext.TaskComments.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null).ConfigureAwait(false);
			if (dbTaskComment == null)
			{
				throw new EntityNotFoundException($"TaskComment with Id {id} not found");
			}

			dbTaskComment.DeletedAt = DateTime.UtcNow;
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
		}


		public async Task<List<TaskLabel>> GetTaskLabelsAsync(Guid taskId)
		{
			var dbTaskLabels = await this.dbContext.TaskLabels.Where(x => x.TaskId == taskId && x.DeletedAt == null).ToListAsync().ConfigureAwait(false);
			return this.mapper.Map<List<TaskLabel>>(dbTaskLabels);
		}

		public async Task<TaskLabel> CreateTaskLabelAsync(TaskLabel taskLabel)
		{
			var dbTaskLabel = this.mapper.Map<DbTaskLabel>(taskLabel);
			this.dbContext.TaskLabels.Add(dbTaskLabel);
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
			return this.mapper.Map<TaskLabel>(dbTaskLabel);
		}

		public async Task<TaskLabel> UpdateTaskLabelAsync(TaskLabel taskLabel)
		{
			var dbTaskLabel = await this.dbContext.TaskLabels.FirstOrDefaultAsync(x => x.Id == taskLabel.Id && x.DeletedAt == null).ConfigureAwait(false);
			if (dbTaskLabel == null)
			{
				throw new EntityNotFoundException($"TaskLabel with Id {taskLabel.Id} not found");
			}

			this.mapper.Map(taskLabel, dbTaskLabel);
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
			return this.mapper.Map<TaskLabel>(dbTaskLabel);
		}

		public async Task DeleteTaskLabelAsync(Guid id)
		{
			var dbTaskLabel = await this.dbContext.TaskLabels.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null).ConfigureAwait(false);
			if (dbTaskLabel == null)
			{
				throw new EntityNotFoundException($"TaskLabel with Id {id} not found");
			}

			dbTaskLabel.DeletedAt = DateTime.UtcNow;
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
		}
		

		public async Task<List<TaskChecklist>> GetTaskChecklistsAsync(Guid taskId)
		{
			var dbTaskChecklist = await this.dbContext.TaskChecklists.Where(x => x.TaskId == taskId && x.DeletedAt == null).ToListAsync().ConfigureAwait(false);
			return this.mapper.Map<List<TaskChecklist>>(dbTaskChecklist);
		}

		public async Task<TaskChecklist> CreateTaskChecklistAsync(TaskChecklist taskChecklist)
		{
			var dbTaskChecklist = this.mapper.Map<DbTaskChecklist>(taskChecklist);
			this.dbContext.TaskChecklists.Add(dbTaskChecklist);
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
			return this.mapper.Map<TaskChecklist>(dbTaskChecklist);
		}

		public async Task<TaskChecklist> UpdateTaskChecklistAsync(TaskChecklist taskChecklist)
		{
			var dbTaskChecklist = await this.dbContext.TaskChecklists.FirstOrDefaultAsync(x => x.Id == taskChecklist.Id && x.DeletedAt == null).ConfigureAwait(false);
			if (dbTaskChecklist == null)
			{
				throw new EntityNotFoundException($"TaskChecklist with Id {taskChecklist.Id} not found");
			}

			this.mapper.Map(taskChecklist, dbTaskChecklist);
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
			return this.mapper.Map<TaskChecklist>(dbTaskChecklist);
		}

		public async Task DeleteTaskChecklistAsync(Guid id)
		{
			var dbTaskChecklist = await this.dbContext.TaskChecklists.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null).ConfigureAwait(false);
			if (dbTaskChecklist == null)
			{
				throw new EntityNotFoundException($"TaskChecklist with Id {id} not found");
			}

			dbTaskChecklist.DeletedAt = DateTime.UtcNow;
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
		}


		public async Task<List<TaskWatcher>> GetTaskWatchersAsync(Guid taskId)
		{
			var dbTaskWatcher = await this.dbContext.TaskWatchers.Where(x => x.TaskId == taskId && x.DeletedAt == null).ToListAsync().ConfigureAwait(false);
			return this.mapper.Map<List<TaskWatcher>>(dbTaskWatcher);
		}

		public async Task<TaskWatcher> CreateTaskWatcherAsync(TaskWatcher taskWatcher)
		{
			var dbTaskWatcher = this.mapper.Map<DbTaskWatcher>(taskWatcher);
			this.dbContext.TaskWatchers.Add(dbTaskWatcher);
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
			return this.mapper.Map<TaskWatcher>(dbTaskWatcher);
		}

		public async Task DeleteTaskWatcherAsync(Guid id)
		{
			var dbTaskWatcher = await this.dbContext.TaskWatchers.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null).ConfigureAwait(false);
			if (dbTaskWatcher == null)
			{
				throw new EntityNotFoundException($"TaskWatcher with Id {id} not found");
			}

			dbTaskWatcher.DeletedAt = DateTime.UtcNow;
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
		}


		public async Task<List<TaskAttachment>> GetTaskAttachmentsAsync(Guid taskId)
		{
			var dbTaskAttachments = await this.dbContext.TaskAttachments.Include(x => x.UploadedBy).Where(x => x.TaskId == taskId && x.DeletedAt == null).ToListAsync().ConfigureAwait(false);
			return this.mapper.Map<List<TaskAttachment>>(dbTaskAttachments);
		}

		public async Task<TaskAttachment> GetTaskAttachmentAsync(Guid taskAttachmentId)
		{
			var dbTaskAttachment = await this.dbContext.TaskAttachments.FirstOrDefaultAsync(x => x.Id == taskAttachmentId && x.DeletedAt == null).ConfigureAwait(false);
			if (dbTaskAttachment == null)
			{
				throw new EntityNotFoundException($"TaskAttachment with Id {taskAttachmentId} not found");
			}

			return this.mapper.Map<TaskAttachment>(dbTaskAttachment);
		}

		public async Task<TaskAttachment> CreateTaskAttachmentAsync(TaskAttachment taskAttachment)
		{
			var dbTaskAttachment = this.mapper.Map<DbTaskAttachment>(taskAttachment);
			this.dbContext.TaskAttachments.Add(dbTaskAttachment);
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
			return this.mapper.Map<TaskAttachment>(dbTaskAttachment);
		}

		public async Task DeleteTaskAttachmentAsync(Guid id)
		{
			var dbTaskAttachment = await this.dbContext.TaskAttachments.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null).ConfigureAwait(false);
			if (dbTaskAttachment == null)
			{
				throw new EntityNotFoundException($"TaskAttachment with Id {id} not found");
			}

			dbTaskAttachment.DeletedAt = DateTime.UtcNow;
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
		}
    }
}
