using System;
using ECommerce.Stock.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ECommerce.Stock.Infrastructure.Migrations;

[DbContext(typeof(StockDbContext))]
partial class StockDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("Relational:MaxIdentifierLength", 63)
            .HasAnnotation("ProductVersion", "8.0.6");

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("ECommerce.Stock.Infrastructure.Entities.InboxMessage", b =>
            {
                b.Property<Guid>("Id")
                    .HasColumnType("uuid");

                b.Property<string>("Consumer")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("character varying(200)");

                b.Property<Guid>("MessageId")
                    .HasColumnType("uuid");

                b.Property<DateTime?>("ProcessedAtUtc")
                    .HasColumnType("timestamp with time zone");

                b.Property<DateTime>("ReceivedAtUtc")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)");

                b.HasKey("Id");

                b.HasIndex("MessageId")
                    .IsUnique();

                b.ToTable("InboxMessages");
            });

        modelBuilder.Entity("ECommerce.Stock.Domain.StockItem", b =>
            {
                b.Property<int>("ProductId")
                    .HasColumnType("integer");

                b.Property<int>("AvailableQuantity")
                    .HasColumnType("integer");

                b.HasKey("ProductId");

                b.ToTable("StockItems");
            });

        modelBuilder.Entity("ECommerce.Stock.Domain.StockReservation", b =>
            {
                b.Property<Guid>("Id")
                    .HasColumnType("uuid");

                b.Property<Guid>("OrderId")
                    .HasColumnType("uuid");

                b.Property<int>("ProductId")
                    .HasColumnType("integer");

                b.Property<int>("Quantity")
                    .HasColumnType("integer");

                b.Property<DateTime>("ReservedAtUtc")
                    .HasColumnType("timestamp with time zone");

                b.HasKey("Id");

                b.ToTable("StockReservations");
            });
    }
}
