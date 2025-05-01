using Microsoft.EntityFrameworkCore;

namespace StudentAPI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<StudentSection> StudentSections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure StudentSection as a many-to-many join table
            modelBuilder.Entity<StudentSection>()
                .HasKey(ss => new { ss.StudentId, ss.SectionId });

            modelBuilder.Entity<StudentSection>()
                .HasOne(ss => ss.Student)
                .WithMany(s => s.StudentSections)
                .HasForeignKey(ss => ss.StudentId);

            modelBuilder.Entity<StudentSection>()
                .HasOne(ss => ss.Section)
                .WithMany(s => s.StudentSections)
                .HasForeignKey(ss => ss.SectionId);

            // Configure foreign key relationship between Section and Subject
            modelBuilder.Entity<Section>()
                .HasOne(s => s.Subject)
                .WithMany(s => s.Sections)
                .HasForeignKey(s => s.SubjectId);
        }
    }
}