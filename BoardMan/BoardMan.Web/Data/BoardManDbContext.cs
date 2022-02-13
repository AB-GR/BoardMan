using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Data;

public class BoardManDbContext : IdentityDbContext<AppUser, Role, Guid>
{
    public BoardManDbContext()
	{

	}

    public BoardManDbContext(DbContextOptions<BoardManDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DbSubscription> Subscriptions { get; set; }

    public virtual DbSet<DbPlan> Plans { get; set; }

    public virtual DbSet<DbPlanDiscount> PlanDiscounts { get; set; }

    public virtual DbSet<DbPaymentTrasaction> PaymentTransactions { get; set; }

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

        builder.Entity<Role>(b =>
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
        builder.Entity<DbPlan>().Property(p => p.Cost).HasPrecision(19, 4);
        builder.Entity<DbPlanDiscount>().Property(p => p.DiscountPercent).HasPrecision(5, 2);
        builder.Entity<DbPaymentTrasaction>().Property(p => p.CostBeforeDiscount).HasPrecision(19, 4);
        builder.Entity<DbPaymentTrasaction>().Property(p => p.DiscountApplied).HasPrecision(19, 4);
        builder.Entity<DbPaymentTrasaction>().Property(p => p.FinalCost).HasPrecision(19, 4);

        // Set cascade deletion to do nothing (consequently deletion fails if there's a dependent record)
        // because we should delete the dependent stuff explicitly to avoid unexpected deletion.
        foreach (var relationship in builder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }

        builder.Entity<DbPlan>().HasData(new DbPlan { Id = Guid.NewGuid(), Name = "Standard", Cost = 99, Currency = "USD", Description = "This is the standard monthly plan", PlanType = PlanType.Monthly, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow, ExpireAt = DateTime.UtcNow.AddYears(1) });
        builder.Entity<DbPlan>().HasData(new DbPlan { Id = Guid.NewGuid(), Name = "Standard", Cost = 948, Currency = "USD", Description = "This is the standard annual plan", PlanType = PlanType.Annual, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow, ExpireAt = DateTime.UtcNow.AddYears(1) });
        builder.Entity<DbPlan>().HasData(new DbPlan { Id = Guid.NewGuid(), Name = "Premium", Cost = 299, Currency = "USD", Description = "This is the premium monthly plan", PlanType = PlanType.Monthly, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow, ExpireAt = DateTime.UtcNow.AddYears(1) });
        builder.Entity<DbPlan>().HasData(new DbPlan { Id = Guid.NewGuid(), Name = "Premium", Cost = 3000, Currency = "USD", Description = "This is the premium annual plan", PlanType = PlanType.Annual, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow, ExpireAt = DateTime.UtcNow.AddYears(1) });
    }
}

public class AppUser : IdentityUser<Guid>
{
	public string FirstName { get; set; }

	public string LastName { get; set; }
}

public class Role : IdentityRole<Guid> { }