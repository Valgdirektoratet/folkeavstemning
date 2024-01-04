﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Resultat.Api.Database;

#nullable disable

namespace Resultat.Api.Migrations
{
    [DbContext(typeof(ResultatContext))]
    [Migration("20231114142450_Migration_ClusteringIndex")]
    partial class Migration_ClusteringIndex
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Resultat.Api.Database.KryptertStemme", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("data");

                    b.Property<string>("FolkeavstemningId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("folkeavstemning_id");

                    b.Property<string>("Signatur")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("signatur");

                    b.HasKey("Id")
                        .HasName("pk_krypterte_stemmer");

                    b.HasIndex("FolkeavstemningId")
                        .HasDatabaseName("ix_krypterte_stemmer_folkeavstemning_id");

                    NpgsqlIndexBuilderExtensions.IncludeProperties(b.HasIndex("FolkeavstemningId"), new[] { "Data", "Signatur" });

                    b.HasIndex("Id")
                        .HasDatabaseName("ix_krypterte_stemmer_id");

                    b.HasIndex("Data", "Signatur")
                        .IsUnique()
                        .HasDatabaseName("ix_krypterte_stemmer_data_signatur");

                    b.ToTable("krypterte_stemmer", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
