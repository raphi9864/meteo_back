using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

    public DbSet<TodoItem> Todos { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration des noms de tables explicites pour Oracle
        modelBuilder.Entity<User>().ToTable("USERS");
        modelBuilder.Entity<TodoItem>().ToTable("TODOITEMS");
        modelBuilder.Entity<UserProfile>().ToTable("USERPROFILES");

        // Configuration des noms de colonnes pour Oracle (majuscules)
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasColumnName("EMAIL");
            entity.Property(e => e.PasswordHash).HasColumnName("PASSWORDHASH");
            entity.Property(e => e.FirstName).HasColumnName("FIRSTNAME");
            entity.Property(e => e.LastName).HasColumnName("LASTNAME");
            entity.Property(e => e.CreatedAt).HasColumnName("CREATEDAT");
            entity.Property(e => e.IsActive).HasColumnName("ISACTIVE").HasConversion<int>();
        });

        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Title).HasColumnName("TITLE");
            entity.Property(e => e.IsDone).HasColumnName("ISDONE").HasConversion<int>();
            entity.Property(e => e.UserId).HasColumnName("USERID");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FirstName).HasColumnName("FIRSTNAME");
            entity.Property(e => e.LastName).HasColumnName("LASTNAME");
            entity.Property(e => e.Email).HasColumnName("EMAIL");
            entity.Property(e => e.BirthDate).HasColumnName("BIRTHDATE");
        });

        // Configuration des relations
        modelBuilder.Entity<TodoItem>()
            .HasOne(t => t.User)
            .WithMany(u => u.TodoItems)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index unique sur l'email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
