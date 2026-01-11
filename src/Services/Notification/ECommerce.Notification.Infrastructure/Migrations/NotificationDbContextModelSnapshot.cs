using System;
using ECommerce.Notification.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ECommerce.Notification.Infrastructure.Migrations;

[DbContext(typeof(NotificationDbContext))]
partial class NotificationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("Relational:MaxIdentifierLength", 63)
            .HasAnnotation("ProductVersion", "8.0.6");

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("ECommerce.Notification.Infrastructure.Entities.InboxMessage", b =>
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

        modelBuilder.Entity("ECommerce.Notification.Domain.NotificationLog", b =>
            {
                b.Property<Guid>("Id")
                    .HasColumnType("uuid");

                b.Property<string>("Channel")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("Content")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<DateTime>("CreatedAtUtc")
                    .HasColumnType("timestamp with time zone");

                b.Property<Guid>("OrderId")
                    .HasColumnType("uuid");

                b.Property<string>("Recipient")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasColumnType("text");

                b.HasKey("Id");

                b.ToTable("NotificationLogs");
            });
    }
}
