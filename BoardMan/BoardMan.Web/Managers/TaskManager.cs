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
			var dbTasks = await this.dbContext.Tasks.Where(x => x.ListId == listId && x.DeletedAt == null).ToListAsync();
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
			var dbTask = await this.dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == task.Id);
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
			var dbTask = await this.dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == taskId);
			if (dbTask == null)
			{
				throw new EntityNotFoundException($"Task with Id {taskId} not found");
			}

			dbTask.DeletedAt = DateTime.UtcNow;
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
		}
		

		public async Task<List<TaskComment>> GetTaskCommentsAsync(Guid taskId)
		{
			var dbTaskComments = await this.dbContext.TaskComments.Where(x => x.TaskId == taskId && x.DeletedAt == null).Include(x => x.CommentedBy).ToListAsync();
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
			var dbTaskComment = await this.dbContext.TaskComments.Include(x => x.CommentedBy).FirstOrDefaultAsync(x => x.Id == taskComment.Id);
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
			var dbTaskComment = await this.dbContext.TaskComments.FirstOrDefaultAsync(x => x.Id == id);
			if (dbTaskComment == null)
			{
				throw new EntityNotFoundException($"TaskComment with Id {id} not found");
			}

			dbTaskComment.DeletedAt = DateTime.UtcNow;
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<List<TaskLabel>> GetTaskLabelsAsync(Guid taskId)
		{
			var dbTaskLabels = await this.dbContext.TaskLabels.Where(x => x.TaskId == taskId && x.DeletedAt == null).ToListAsync();
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
			var dbTaskLabel = await this.dbContext.TaskLabels.FirstOrDefaultAsync(x => x.Id == taskLabel.Id);
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
			var dbTaskLabel = await this.dbContext.TaskLabels.FirstOrDefaultAsync(x => x.Id == id);
			if (dbTaskLabel == null)
			{
				throw new EntityNotFoundException($"TaskLabel with Id {id} not found");
			}

			dbTaskLabel.DeletedAt = DateTime.UtcNow;
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
		}
	}
}
