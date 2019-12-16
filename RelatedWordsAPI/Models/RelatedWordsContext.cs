using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RelatedWordsAPI.App;

namespace RelatedWordsAPI.Models
{

    public class RelatedWordsContext : DbContext
    {
        public RelatedWordsContext(DbContextOptions<RelatedWordsContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Sentence> Sentences { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<WordPage> WordPages { get; set; }
        public DbSet<WordSentence> WordSentences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Project>().ToTable("Project");
            modelBuilder.Entity<Page>().ToTable("Page")
                .Property(p => p.ProcessingStatus)
                .HasDefaultValue(PageProcessingStatus.NotStarted);
        }
    }




}
