using System;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) :
IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole,
IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>(options) // config the entities to use int as id
{
   // IdentityDbContext has a DbSet for Users
   public DbSet<UserLike> Likes { get; set; }
   public DbSet<Message> Messages { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      // one to many user <- roles
      modelBuilder.Entity<User>()
         .HasMany(u => u.UserRoles) // user can have many UserRoles
         .WithOne(ur => ur.User) // Each UserRole only linked to one User
         .HasForeignKey(ur => ur.UserId) 
         .IsRequired();

      // one to many role <- users
      modelBuilder.Entity<Role>()
         .HasMany(r => r.UserRoles) // role has many UserRoles
         .WithOne(ur => ur.Role) // each UserRole linked to one Role
         .HasForeignKey(ur => ur.RoleId)
         .IsRequired();

      modelBuilder.Entity<User>()
         .HasIndex(u => u.UserName)
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
