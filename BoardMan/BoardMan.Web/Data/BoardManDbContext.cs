using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Data;

public class BoardManDbContext : IdentityDbContext<AppUser, Role, Guid>
{
    public BoardManDbContext(DbContextOptions<BoardManDbContext> options)
        : base(options)
    {
    }

    public DbSet<DbSubscription> Subscriptions { get; set; }

    public DbSet<DbPlan> Plans { get; set; }

    public DbSet<DbPlanDiscount> PlanDiscounts { get; set; }

    public DbSet<DbPaymentTrasaction> PaymentTransactions { get; set; }

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
    }
}

public class AppUser : IdentityUser<Guid>
{
	public string FirstName { get; set; }

	public string LastName { get; set; }
}

public class Role : IdentityRole<Guid> { }