using System;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
   public DbSet<User> Users { get; set; }
   public DbSet<UserLike> Likes { get; set; }
   public DbSet<Message> Messages { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<User>()
         .HasIndex(u => u.Username)
         .IsUnique();

      modelBuilder.Entity<UserLike>()
         .HasKey(k => new { k.UserId, k.LikedUserId }); // configure the composite key value 

      modelBuilder.Entity<UserLike>()
         .HasOne(s => s.User) // source user
         .WithMany(l => l.LikedUsers) // liked users
         .HasForeignKey(s => s.UserId) // use source user as foreign key
         .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<UserLike>()
         .HasOne(s => s.LikedUser) // source user
         .WithMany(l => l.LikedBy) // liked users
         .HasForeignKey(s => s.LikedUserId) // use source user as foreign key
         .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<Message>()
         .HasOne(x => x.Recipient)
         .WithMany(x => x.MessagesReceived)
         .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<Message>()
         .HasOne(x => x.Sender)
         .WithMany(x => x.MessagesSent)
         .OnDelete(DeleteBehavior.Restrict);
   }

}
