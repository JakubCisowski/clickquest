﻿// <auto-generated />
using ClickQuest.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ClickQuest.Migrations
{
    [DbContext(typeof(UserContext))]
    [Migration("20201130130553_AddDbKey")]
    partial class AddDbKey
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("ClickQuest.Heroes.Hero", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("ClickDamage")
                        .HasColumnType("int");

                    b.Property<double>("CritChance")
                        .HasColumnType("float");

                    b.Property<int>("Experience")
                        .HasColumnType("int");

                    b.Property<int>("Gold")
                        .HasColumnType("int");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PoisonDamage")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Heroes");
                });

            modelBuilder.Entity("ClickQuest.Items.Artifact", b =>
                {
                    b.Property<int>("DbKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("Rarity")
                        .HasColumnType("int");

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("DbKey");

                    b.ToTable("Artifacts");
                });

            modelBuilder.Entity("ClickQuest.Items.Material", b =>
                {
                    b.Property<int>("DbKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("Rarity")
                        .HasColumnType("int");

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("DbKey");

                    b.ToTable("Materials");
                });

            modelBuilder.Entity("ClickQuest.Items.Recipe", b =>
                {
                    b.Property<int>("DbKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("Rarity")
                        .HasColumnType("int");

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("DbKey");

                    b.ToTable("Recipes");
                });
#pragma warning restore 612, 618
        }
    }
}