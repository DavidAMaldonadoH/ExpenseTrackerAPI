using ExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerAPI;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; set; }
    public required DbSet<Expense> Expenses { get; set; }
    public required DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.ToTable("User");

            user.Property(u => u.Id).HasColumnType("bigint");
            user.HasKey(u => u.Id);
            user.Property(u => u.Username).IsRequired().HasColumnType("varchar(64)");
            user.HasIndex(u => u.Username).IsUnique();
            user.Property(u => u.FirstName).IsRequired().HasColumnType("varchar(64)");
            user.Property(u => u.LastName).IsRequired().HasColumnType("varchar(64)");
            user.Property(u => u.Password).IsRequired().HasColumnType("varchar(255)");
        });

        modelBuilder.Entity<Category>(category =>
        {
            category.ToTable("Category");

            category.Property(c => c.Id).HasColumnType("int");
            category.HasKey(c => c.Id);
            category.Property(c => c.Name).IsRequired().HasColumnType("varchar(64)");
            category.HasIndex(c => c.Name).IsUnique();
            category.Property(c => c.Description).HasColumnType("varchar(255)");
        });

        modelBuilder.Entity<Expense>(expense =>
        {
            expense.ToTable("Expense");

            expense.Property(e => e.Id).HasColumnType("bigint");
            expense.HasKey(e => e.Id);
            expense.Property(e => e.Name).IsRequired().HasColumnType("varchar(128)");
            expense.Property(e => e.Amount).IsRequired().HasColumnType("decimal(10, 2)");
            expense.Property(e => e.CategoryId).IsRequired().HasColumnType("int");
        });

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.User)
            .WithMany(u => u.Expenses)
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("fk_user_expenses")
            .IsRequired();

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.Category)
            .WithMany(c => c.Expenses)
            .HasForeignKey(e => e.CategoryId)
            .HasConstraintName("fk_category_expenses")
            .IsRequired();
    }
}