﻿using System;
using System.Collections.Generic;
using MSPR_bloc_4_orders.Models;
using Microsoft.EntityFrameworkCore;

namespace MSPR_bloc_4_orders.Data;

public partial class OrdersDbContext : DbContext
{
    public OrdersDbContext()
    {
    }

    public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Commande> Commandes { get; set; }

    public virtual DbSet<ProduitCommande> ProduitCommandes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Commande>(entity =>
        {
            entity.HasKey(e => e.IdCommande).HasName("PK__commande__385131BFF43B5808");

            entity.ToTable("commandes");

            entity.Property(e => e.IdCommande)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_commande");
            entity.Property(e => e.Createdate)
                .HasColumnType("datetime")
                .HasColumnName("createdate");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
        });

        modelBuilder.Entity<ProduitCommande>(entity =>
        {
            entity.HasKey(e => e.IdProduitcommande).HasName("PK__Produit___EB86D38CC4F584D1");

            entity.ToTable("Produit_commandes");

            entity.Property(e => e.IdProduitcommande).HasColumnName("id_produitcommande");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IdCommande).HasColumnName("id_commande");
            entity.Property(e => e.IdProduit).HasColumnName("id_produit");
            entity.Property(e => e.Nom)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nom");
            entity.Property(e => e.Prix)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("prix");
            entity.Property(e => e.Quantite).HasColumnName("quantite");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
