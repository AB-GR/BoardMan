using System.Collections.Concurrent;

namespace BoardMan.Web.Infrastructure.Utils
{
	public static class ValueLock
	{
		private static readonly ConcurrentDictionary<string, TimedLock> locks = new();

		public static TimedLock Get(Guid id)
		{
			return locks.GetOrAdd(Key(id), s => new TimedLock());
		}

		public static TimedLock Get(string id)
		{
			return locks.GetOrAdd(id, s => new TimedLock());
		}

		static string Key(Guid id) => id.ToString("N");
	}

    public class TimedLock
    {
        private readonly SemaphoreSlim toLock;

        public TimedLock()
        {
            toLock = new SemaphoreSlim(1, 1);
        }

        public async Task<LockReleaser> Lock(int milliseconds = -1)
        {
            if (await toLock.WaitAsync(milliseconds))
            {
                return new LockReleaser(toLock);
            }

            throw new TimeoutException();
        }

        public struct LockReleaser : IDisposable
        {
            private readonly SemaphoreSlim toRelease;

            public LockReleaser(SemaphoreSlim toRelease)
            {
                this.toRelease = toRelease;
            }

            public void Dispose()
            {
                toRelease.Release();
            }
        }
    }
}
