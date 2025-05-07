using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        { }

        public DbSet<ProjectCategory> ProjectCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Collaborator> Collaborators { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Domain.Entities.File> Files { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Pledge> Pledges { get; set; }
        public DbSet<PledgeDetail> PledgeDetails { get; set; }
        public DbSet<ProjectComment> ProjectComments { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ProjectPlatform> ProjectPlatforms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<VerifyCode> VerifyCodes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserId)
                .IsUnique();

            modelBuilder.Entity<Collaborator>()
                .HasKey(c => new { c.UserId, c.ProjectId });

            modelBuilder.Entity<PledgeDetail>()
                .HasKey(c => new { c.PledgeId, c.PaymentId });

            modelBuilder.Entity<FAQ>()
                .HasKey(g => new { g.ProjectId, g.Question });

            modelBuilder.Entity<ProjectCategory>()
                .HasKey(pc => new { pc.CategoryId, pc.ProjectId });

            modelBuilder.Entity<ProjectPlatform>()
                .HasKey(pc => new { pc.PlatformId, pc.ProjectId });

            modelBuilder.Entity<PostComment>()
                .HasKey(pc => new { pc.CommentId, pc.PostId });

            modelBuilder.Entity<ProjectComment>()
                .HasKey(pc => new { pc.CommentId, pc.ProjectId });

            modelBuilder.Entity<User>()
                .HasMany(u => u.Projects)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.MonitoredProjects)
                .WithOne(p => p.Monitor)
                .HasForeignKey(p => p.MonitorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Pledges)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Pledges)
                .WithOne(p => p.Project)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Rewards)
                .WithOne(r => r.Project)
                .HasForeignKey(r => r.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Token>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.Categories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Comments)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Collaborator>()
                .HasOne(c => c.User)
                .WithMany(u => u.Collaborators)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Collaborator>()
                .HasOne(c => c.Project)
                .WithMany(p => p.Collaborators)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PledgeDetail>()
                .HasOne(pd => pd.Pledge)
                .WithMany(p => p.PledgeDetails)
                .HasForeignKey(pd => pd.PledgeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProjectCategories)
                .HasForeignKey(pc => pc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectCategory>()
                .HasOne(pc => pc.Project)
                .WithMany(p => p.ProjectCategories)
                .HasForeignKey(pc => pc.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectPlatform>()
                .HasOne(pp => pp.Platform)
                .WithMany(p => p.ProjectPlatforms)
                .HasForeignKey(pp => pp.PlatformId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectPlatform>()
                .HasOne(pp => pp.Project)
                .WithMany(p => p.ProjectPlatforms)
                .HasForeignKey(pp => pp.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostComment>()
                .HasOne(pc => pc.Comment)
                .WithOne(c => c.PostComment)
                .HasForeignKey<PostComment>(pc => pc.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.PostComments)
                .WithOne(pc => pc.Post)
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectComment>()
                .HasOne(pc => pc.Comment)
                .WithOne(c => c.ProjectComment)
                .HasForeignKey<ProjectComment>(pc => pc.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectComment>()
                .HasOne(pc => pc.Project)
                .WithMany(p => p.ProjectComments)
                .HasForeignKey(pc => pc.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FAQ>()
                .HasOne(f => f.Project)
                .WithMany(p => p.Questions)
                .HasForeignKey(f => f.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Project)
                .WithMany(pr => pr.Posts)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Domain.Entities.File>()
                .HasOne(f => f.User)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
