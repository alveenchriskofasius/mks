﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Context.Table;

public partial class MKSContext : DbContext
{
    public MKSContext(DbContextOptions<MKSContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleClaim> RoleClaims { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserClaim> UserClaims { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Role__3214EC27435702FB");

            entity.ToTable("Role");

            entity.Property(e => e.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<RoleClaim>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__RoleClai__3214EC279D89C9EF");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__User__3214EC076267E296");

            entity.ToTable("User");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FullName).IsRequired();
            entity.Property(e => e.KTP)
                .IsRequired()
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber).IsRequired();
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<UserClaim>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__UserClai__3214EC2701BE389D");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("UserRole");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}