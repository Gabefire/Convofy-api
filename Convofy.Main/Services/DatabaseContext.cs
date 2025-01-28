using Microsoft.EntityFrameworkCore;
using Convofy.Models.User;
using Convofy.Main.Models.Forum;
using Convofy.Main.Models.Post;

namespace Convofy.Main.Services;
public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; set; }
    public required DbSet<Forum> Forums { get; set; }
    public required DbSet<UserForumFollows> UserForumFollows { get; set; }
    public required DbSet<Post> Posts { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.UpdatedAt)
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnUpdate();

        modelBuilder.Entity<Forum>()
            .Property(f => f.UpdatedAt)
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnUpdate();

        modelBuilder.Entity<UserForumFollows>()
            .Property(uf => uf.UpdatedAt)
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnUpdate();

        modelBuilder.Entity<Post>()
            .Property(p => p.UpdatedAt)
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnUpdate();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is User && e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            ((User)entry.Entity).UpdatedAt = DateTime.UtcNow;
        }

        var forumEntries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is Forum && e.State == EntityState.Modified);

        foreach (var entry in forumEntries)
        {
            ((Forum)entry.Entity).UpdatedAt = DateTime.UtcNow;
        }

        var userForumFollowsEntries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is UserForumFollows && e.State == EntityState.Modified);

        foreach (var entry in userForumFollowsEntries)
        {
            ((UserForumFollows)entry.Entity).UpdatedAt = DateTime.UtcNow;
        }

        var postEntries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is Post && e.State == EntityState.Modified);

        foreach (var entry in postEntries)
        {
            ((Post)entry.Entity).UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}