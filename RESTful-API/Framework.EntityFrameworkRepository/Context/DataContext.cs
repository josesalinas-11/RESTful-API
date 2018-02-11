using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Framework.Data.Entity.Contract;
using Framework.EntityFrameworkRepository.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Framework.EntityFrameworkRepository.Context
{
    public abstract class DataContext : DbContext
    {
        protected DataContext(DbContextOptions<DataContext> options) : base(options) =>
#if DEBUG
            this.ConfigureLogging(l => Debug.WriteLine(l));
#endif

        protected DataContext() =>
#if DEBUG
            this.ConfigureLogging(l => Debug.WriteLine(l));
#endif


        public Func<DateTime> TimestampProvider { get; set; } = () => DateTime.UtcNow;

        public override int SaveChanges()
        {
            TrackChanges();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            TrackChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void TrackChanges()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
            {
                if (!(entry.Entity is ISoftDelete auditable)) continue;
                switch (entry.State)
                {
                    case EntityState.Added:
                        break;
                    case EntityState.Deleted:
                        auditable.DeleteAt = TimestampProvider();
                        entry.State = EntityState.Modified;
                        break;
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Modified:
                        break;
                }
            }
        }
    }
}