using MasterYourEnglish.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MasterYourEnglish.DAL.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Flashcard> Flashcards { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<FlashcardBundle> FlashcardBundles { get; set; }
        public DbSet<FlashcardBundleItem> FlashcardBundleItems { get; set; }
        public DbSet<SavedFlashcard> SavedFlashcards { get; set; }
        public DbSet<FlashcardBundleAttempt> FlashcardBundleAttempts { get; set; }
        public DbSet<FlashcardAttemptAnswer> FlashcardAttemptAnswers { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<TestQuestion> TestQuestions { get; set; }
        public DbSet<TestAttempt> TestAttempts { get; set; }
        public DbSet<TestAttemptAnswer> TestAttemptAnswers { get; set; }
        public DbSet<TestAttemptAnswerSelectedOption> TestAttemptAnswerSelectedOptions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FlashcardBundleItem>()
                .HasKey(fbi => new { fbi.FlashcardsBundleId, fbi.FlashcardId });

            modelBuilder.Entity<SavedFlashcard>()
                .HasKey(sf => new { sf.UserId, sf.FlashcardId });

            modelBuilder.Entity<TestQuestion>()
                .HasKey(tq => new { tq.TestId, tq.QuestionId });

            modelBuilder.Entity<TestAttemptAnswerSelectedOption>()
                .HasKey(taaso => new { taaso.AttemptAnswersId, taaso.SelectedOptionId });

            modelBuilder.Entity<FlashcardBundleItem>()
                .HasOne(fbi => fbi.FlashcardsBundle)
                .WithMany(b => b.FlashcardsBundleItems)
                .HasForeignKey(fbi => fbi.FlashcardsBundleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FlashcardBundleItem>()
                .HasOne(fbi => fbi.Flashcard)
                .WithMany(fbi => fbi.FlashcardsBundleItems)
                .HasForeignKey(fbi => fbi.FlashcardId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<TestQuestion>()
                .HasOne(tq => tq.Test)
                .WithMany(t => t.TestQuestions)
                .HasForeignKey(tq => tq.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestQuestion>()
                .HasOne(tq => tq.Question)
                .WithMany(q => q.TestQuestions)
                .HasForeignKey(tq => tq.QuestionId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<SavedFlashcard>()
                .HasOne(sf => sf.User)
                .WithMany(u => u.SavedFlashcards)
                .HasForeignKey(sf => sf.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SavedFlashcard>()
                .HasOne(sf => sf.Flashcard)
                .WithMany(f => f.SavedByUsers)
                .HasForeignKey(sf => sf.FlashcardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Flashcard>()
                .HasOne(f => f.CreatedByUser)
                .WithMany(u => u.Flashcards)
                .HasForeignKey(f => f.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<FlashcardBundle>()
                .HasOne(b => b.CreatedByUser)
                .WithMany(u => u.FlashcardsBundles)
                .HasForeignKey(b => b.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Test>()
                .HasOne(t => t.CreatedByUser)
                .WithMany(u => u.Tests)
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.CreatedByUser)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<TestAttempt>()
                .HasOne(ta => ta.Test)
                .WithMany(t => t.TestAttempts)
                .HasForeignKey(ta => ta.TestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TestAttempt>()
                .HasOne(ta => ta.User)
                .WithMany(u => u.TestAttempts)
                .HasForeignKey(ta => ta.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
