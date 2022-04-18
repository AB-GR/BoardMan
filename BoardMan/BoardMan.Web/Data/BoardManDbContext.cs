using BoardMan.Web.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Data;

public class BoardManDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public BoardManDbContext()
	{
        TrackTimeStamps();
    }

    public BoardManDbContext(DbContextOptions<BoardManDbContext> options)
        : base(options)
    {
        TrackTimeStamps();
    }

    public Guid? LoggedInUserId { get; set; }

    public virtual DbSet<DbWorkspace> Workspaces { get; set; } = null!;

    public virtual DbSet<DbWorkspaceMember> WorkspaceMembers { get; set; } = null!;

    public virtual DbSet<DbBoard> Boards { get; set; } = null!;

    public virtual DbSet<DbBoardMember> BoardMembers { get; set; } = null!;

    public virtual DbSet<DbList> Lists { get; set; } = null!;

    public virtual DbSet<DbTask> Tasks { get; set; } = null!;

    public virtual DbSet<DbTaskComment> TaskComments { get; set; } = null!;

    public virtual DbSet<DbTaskChecklist> TaskChecklists { get; set; } = null!;

    public virtual DbSet<DbTaskAttachment> TaskAttachments { get; set; } = null!;

    public virtual DbSet<DbTaskWatcher> TaskWatchers { get; set; } = null!;

    public virtual DbSet<DbTaskLabel> TaskLabels { get; set; } = null!;

    public virtual DbSet<DbActivityTracking> ActivityTrackings { get; set; } = null!;

    public virtual DbSet<DbEmailInvite> EmailInvites { get; set; } = null!;

    public virtual DbSet<DbSubscription> Subscriptions { get; set; } = null!;

    public virtual DbSet<DbPlan> Plans { get; set; } = null!;

    public virtual DbSet<DbPlanDiscount> PlanDiscounts { get; set; } = null!;

    public virtual DbSet<DbPaymentTransaction> PaymentTransactions { get; set; } = null!;

    public virtual DbSet<DbBillingDetails> BillingDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<AppUser>(b =>
        {
            b.Property(u => u.Id).HasDefaultValueSql("newsequentialid()");
        });

        builder.Entity<AppRole>(b =>
        {
            b.Property(u => u.Id).HasDefaultValueSql("newsequentialid()");
        });

        // for using auto seeded Guid Ids
        foreach (var entity in builder.Model.GetEntityTypes().Where(t => t.ClrType.BaseType == typeof(DbEntity)))
        {
            foreach (var property in entity.ClrType.GetProperties()
                .Where(p => p.PropertyType == typeof(Guid) && p.Name == "Id"
                && p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute))))
            {
                builder.Entity(entity.ClrType).Property(property.Name).HasDefaultValueSql("newsequentialid()");
            }
        }

        builder.Entity<DbPlanDiscount>().HasIndex(pd => new { pd.Code, pd.PlanId }).IsUnique();
        builder.Entity<DbBoardMember>().HasIndex(pd => new { pd.MemberId, pd.RoleId }).IsUnique();
        builder.Entity<DbEmailInvite>().HasIndex(pd => new { pd.EmailAddress, pd.RoleId }).IsUnique();

        builder.Entity<DbPlan>().Property(p => p.Cost).HasPrecision(19, 4);
        builder.Entity<DbPlanDiscount>().Property(p => p.DiscountPercent).HasPrecision(5, 2);
        builder.Entity<DbPaymentTransaction>().Property(p => p.CostBeforeDiscount).HasPrecision(19, 4);
        builder.Entity<DbPaymentTransaction>().Property(p => p.DiscountApplied).HasPrecision(19, 4);
        builder.Entity<DbPaymentTransaction>().Property(p => p.FinalCost).HasPrecision(19, 4);

        // Set cascade deletion to do nothing (consequently deletion fails if there's a dependent record)
        // because we should delete the dependent stuff explicitly to avoid unexpected deletion.
        foreach (var relationship in builder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }

		//builder.Entity<DbPlan>().HasData(new DbPlan { Id = Guid.NewGuid(), Name = "Free", Cost = 0, Currency = "USD", Description = "This is the free plan", PlanType = PlanType.Annual, CreatedAt = DateTime.UtcNow, ExpireAt = DateTime.UtcNow.AddYears(1), BoardLimit = 1 });
		//builder.Entity<DbPlan>().HasData(new DbPlan { Id = Guid.NewGuid(), Name = "Standard (Monthly)", Cost = 99, Currency = "USD", Description = "This is the standard monthly plan", PlanType = PlanType.Monthly, CreatedAt = DateTime.UtcNow, ExpireAt = DateTime.UtcNow.AddYears(1), BoardLimit = 5 });
		//builder.Entity<DbPlan>().HasData(new DbPlan { Id = Guid.NewGuid(), Name = "Standard (Annual)", Cost = 948, Currency = "USD", Description = "This is the standard annual plan", PlanType = PlanType.Annual, CreatedAt = DateTime.UtcNow, ExpireAt = DateTime.UtcNow.AddYears(1), BoardLimit = 5 });
		//builder.Entity<DbPlan>().HasData(new DbPlan { Id = Guid.NewGuid(), Name = "Premium (Monthly)", Cost = 299, Currency = "USD", Description = "This is the premium monthly plan", PlanType = PlanType.Monthly, CreatedAt = DateTime.UtcNow, ExpireAt = DateTime.UtcNow.AddYears(1) });
		//builder.Entity<DbPlan>().HasData(new DbPlan { Id = Guid.NewGuid(), Name = "Premium (Annual)", Cost = 3000, Currency = "USD", Description = "This is the premium annual plan", PlanType = PlanType.Annual, CreatedAt = DateTime.UtcNow, ExpireAt = DateTime.UtcNow.AddYears(1) });

		//builder.Entity<AppRole>().HasData(new AppRole { Id = Guid.NewGuid(), Name = RoleNames.ApplicationSuperAdmin, NormalizedName = RoleNames.ApplicationSuperAdmin });
		//builder.Entity<AppRole>().HasData(new AppRole { Id = Guid.NewGuid(), Name = RoleNames.WorkspaceSuperAdmin, NormalizedName = RoleNames.WorkspaceSuperAdmin });
		//builder.Entity<AppRole>().HasData(new AppRole { Id = Guid.NewGuid(), Name = RoleNames.WorkspaceAdmin, NormalizedName = RoleNames.WorkspaceAdmin });
		//builder.Entity<AppRole>().HasData(new AppRole { Id = Guid.NewGuid(), Name = RoleNames.WorkspaceContributor, NormalizedName = RoleNames.WorkspaceContributor });
		//builder.Entity<AppRole>().HasData(new AppRole { Id = Guid.NewGuid(), Name = RoleNames.WorkspaceReader, NormalizedName = RoleNames.WorkspaceReader });
		//builder.Entity<AppRole>().HasData(new AppRole { Id = Guid.NewGuid(), Name = RoleNames.BoardSuperAdmin, NormalizedName = RoleNames.BoardSuperAdmin });
		//builder.Entity<AppRole>().HasData(new AppRole { Id = Guid.NewGuid(), Name = RoleNames.BoardAdmin, NormalizedName = RoleNames.BoardAdmin });
		//builder.Entity<AppRole>().HasData(new AppRole { Id = Guid.NewGuid(), Name = RoleNames.BoardContributor, NormalizedName = RoleNames.BoardContributor });
		//builder.Entity<AppRole>().HasData(new AppRole { Id = Guid.NewGuid(), Name = RoleNames.BoardReader, NormalizedName = RoleNames.BoardReader });

		//var userId = Guid.NewGuid();
        //builder.Entity<AppUser>().HasData(new AppUser { Id = userId, FirstName = "Admin", LastName = "User", UserName = "admin@boardman.com", NormalizedUserName = "admin@boardman.com", Email = "admin@boardman.com", NormalizedEmail = "admin@boardman.com", EmailConfirmed = true, PasswordHash = "", SecurityStamp = Guid.NewGuid().ToString() });
        //builder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid> { UserId = userId, RoleId = Guid.Parse("49e3045d-b948-4d0a-b596-455673bd989c") });

        builder.Entity<DbEmailInvite>().Navigation(e => e.AddedBy).AutoInclude();

        builder.Entity<DbBoardMember>().Navigation(e => e.AddedBy).AutoInclude();
        builder.Entity<DbBoardMember>().Navigation(e => e.Member).AutoInclude();
        builder.Entity<DbBoardMember>().Navigation(e => e.Role).AutoInclude();

        builder.Entity<DbWorkspaceMember>().Navigation(e => e.AddedBy).AutoInclude();
        builder.Entity<DbWorkspaceMember>().Navigation(e => e.Member).AutoInclude();
        builder.Entity<DbWorkspaceMember>().Navigation(e => e.Role).AutoInclude();
    }

    private void TrackTimeStamps()
	{
		ChangeTracker.StateChanged += UpdateTimestamps;
        ChangeTracker.Tracked += UpdateTimestamps;
    }

    private static void UpdateTimestamps(object sender, EntityEntryEventArgs e)
    {
        if (e.Entry.Entity is DbEntity entityWithTimestamps)
        {
            switch (e.Entry.State)
            {
                case EntityState.Modified:
                    entityWithTimestamps.ModifiedAt = DateTime.UtcNow;                    
                    break;
                case EntityState.Added:
                    entityWithTimestamps.CreatedAt = DateTime.UtcNow;                    
                    break;
            }
        }
    }
}

public class AppUser : IdentityUser<Guid>
{
	public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
}

public class AppRole : IdentityRole<Guid> { }