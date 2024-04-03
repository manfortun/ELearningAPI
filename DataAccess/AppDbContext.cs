using eLearningApi.Models;
using Microsoft.EntityFrameworkCore;

namespace eLearningApi.DataAccess;

public class AppDbContext : DbContext
{
    public DbSet<Content> Contents { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<EnrollmentModule> EnrollmentModules { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Course>()
            .HasOne(c => c.Author)
            .WithMany(u => u.Courses)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Enrollment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Module>()
            .HasOne(c => c.Author)
            .WithMany(u => u.Modules)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Subject>()
            .HasOne(c => c.Author)
            .WithMany(u => u.Subjects)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Content>()
            .HasOne(c => c.Author)
            .WithMany(u => u.Contents)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<EnrollmentModule>()
            .HasOne(c => c.Enrollment)
            .WithMany(u => u.EnrollmentModules)
            .HasForeignKey(c => c.EnrollmentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
