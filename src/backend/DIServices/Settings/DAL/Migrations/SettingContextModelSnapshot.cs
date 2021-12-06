﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Log4Pro.DIServices.Settings.DAL;

namespace Log4Pro.DIServices.Settings.DAL.Migrations
{
    [DbContext(typeof(SettingContext))]
    partial class SettingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Log4Pro.AspNetServices.Settings.DAL.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DefaultValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InstanceOrUserKey")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("ModuleKey")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Options")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Version")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("InstanceOrUserKey");

                    b.HasIndex("Key");

                    b.HasIndex("ModuleKey");

                    b.HasIndex("Version");

                    b.HasIndex("ModuleKey", "InstanceOrUserKey", "Key")
                        .IsUnique()
                        .HasFilter("[ModuleKey] IS NOT NULL AND [InstanceOrUserKey] IS NOT NULL");

                    b.ToTable("Settings", "settings");
                });

            modelBuilder.Entity("Log4Pro.AspNetServices.Settings.DAL.SettingHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Changer")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("InstanceOrUserKey")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ModuleKey")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("OperationType")
                        .HasColumnType("int")
                        .HasColumnName("OperationType");

                    b.Property<string>("SettingKey")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("Version")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Changer");

                    b.HasIndex("InstanceOrUserKey");

                    b.HasIndex("ModuleKey");

                    b.HasIndex("SettingKey");

                    b.HasIndex("TimeStamp");

                    b.HasIndex("Version");

                    b.ToTable("SettingHistories", "settings");

                    b.HasDiscriminator<int>("OperationType");
                });

            modelBuilder.Entity("Log4Pro.AspNetServices.Settings.DAL.CreateSetting", b =>
                {
                    b.HasBaseType("Log4Pro.AspNetServices.Settings.DAL.SettingHistory");

                    b.Property<string>("To")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("To");

                    b.ToTable("SettingHistories", "settings");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("Log4Pro.AspNetServices.Settings.DAL.DeleteSetting", b =>
                {
                    b.HasBaseType("Log4Pro.AspNetServices.Settings.DAL.SettingHistory");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("From")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("From");

                    b.ToTable("SettingHistories", "settings");

                    b.HasDiscriminator().HasValue(2);
                });

            modelBuilder.Entity("Log4Pro.AspNetServices.Settings.DAL.ModifySetting", b =>
                {
                    b.HasBaseType("Log4Pro.AspNetServices.Settings.DAL.SettingHistory");

                    b.Property<string>("From")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("From");

                    b.Property<string>("To")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("To");

                    b.ToTable("SettingHistories", "settings");

                    b.HasDiscriminator().HasValue(1);
                });
#pragma warning restore 612, 618
        }
    }
}
