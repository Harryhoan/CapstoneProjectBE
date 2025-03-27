using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public DbSet<ProjectPlatform> GamePlatforms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Project> Projects { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints here if needed
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserId)
                .IsUnique();

            modelBuilder.Entity<Collaborator>()
                .HasKey(c => new { c.UserId, c.ProjectId });

            modelBuilder.Entity<PledgeDetail>()
                .HasKey(c => new { c.PledgeId, c.PaymentId });

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Comments)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Domain.Entities.File>()
                .HasOne(f => f.User)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pledge>()
                .HasMany(p => p.PledgeDetails)
                .WithOne(pd => pd.Pledge)
                .HasForeignKey(pd => pd.PledgeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Project)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Token>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Monitor)
                .WithMany(u => u.MonitoredProjects)
                .HasForeignKey(p => p.MonitorId)
                .OnDelete(DeleteBehavior.Cascade); // New configuration

            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.Categories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FAQ>()
                .HasOne(g => g.Project)
                .WithMany(p => p.Question)
                .HasForeignKey(g => g.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FAQ>()
                .HasKey(g => new { g.ProjectId, g.Question });

            modelBuilder.Entity<ProjectCategory>()
                .HasKey(pc => new { pc.CategoryId, pc.ProjectId });

            modelBuilder.Entity<ProjectComment>()
                .HasKey(pc => new { pc.CommentId, pc.ProjectId });

            modelBuilder.Entity<ProjectPlatform>()
                .HasKey(pp => new { pp.PlatformId, pp.ProjectId });

            modelBuilder.Entity<Reward>()
                .HasOne(r => r.Project)
                .WithMany(p => p.Rewards)
                .HasForeignKey(r => r.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostComment>()
                .HasKey(pc => new { pc.CommentId, pc.PostId });
        }

    }
}
