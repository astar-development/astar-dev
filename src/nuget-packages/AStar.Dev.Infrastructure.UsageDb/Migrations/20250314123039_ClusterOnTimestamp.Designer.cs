﻿// <auto-generated />

#nullable disable

using AStar.Dev.Infrastructure.UsageDb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AStar.Dev.Api.Usage.Logger.Migrations
{
    [DbContext(typeof(ApiUsageContext))]
    [Migration("20250314123039_ClusterOnTimestamp")]
    partial class ClusterOnTimestamp
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("usage")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AStar.Dev.Api.Usage.Sdk.ApiUsageEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ApiEndpoint")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("ApiName")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<long>("ElapsedMilliseconds")
                        .HasColumnType("bigint");

                    b.Property<int>("StatusCode")
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ApiName");

                    b.HasIndex(new[] { "Timestamp" }, "UpdatedDate_IX");

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex(new[] { "Timestamp" }, "UpdatedDate_IX"));

                    b.ToTable("ApiUsageEvent", "usage");
                });
#pragma warning restore 612, 618
        }
    }
}
