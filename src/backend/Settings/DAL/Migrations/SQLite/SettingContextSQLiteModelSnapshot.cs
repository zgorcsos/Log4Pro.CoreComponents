﻿// <auto-generated />
using System;
using Log4Pro.CoreComponents.Settings.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Log4Pro.CoreComponents.Settings.DAL.Migrations.SQLite
{
    [DbContext(typeof(SettingContextSQLite))]
    partial class SettingContextSQLiteModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.15");

            modelBuilder.Entity("Log4Pro.CoreComponents.Settings.DAL.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("InstanceOrUserKey")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<string>("ModuleKey")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<bool>("UserLevelSettings")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("InstanceOrUserKey");

                    b.HasIndex("Key");

                    b.HasIndex("ModuleKey");

                    b.HasIndex("UserLevelSettings");

                    b.HasIndex("Version");

                    b.HasIndex("ModuleKey", "InstanceOrUserKey", "Key")
                        .IsUnique();

                    b.ToTable("Settings", "settings");
                });

            modelBuilder.Entity("Log4Pro.CoreComponents.Settings.DAL.SettingHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Changer")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("InstanceOrUserKey")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("ModuleKey")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("OperationType")
                        .HasColumnType("INTEGER")
                        .HasColumnName("OperationType");

                    b.Property<string>("SettingKey")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

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

            modelBuilder.Entity("Log4Pro.CoreComponents.Settings.DAL.CreateSetting", b =>
                {
                    b.HasBaseType("Log4Pro.CoreComponents.Settings.DAL.SettingHistory");

                    b.Property<string>("To")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("TEXT")
                        .HasColumnName("To");

                    b.ToTable("SettingHistories", "settings");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("Log4Pro.CoreComponents.Settings.DAL.DeleteSetting", b =>
                {
                    b.HasBaseType("Log4Pro.CoreComponents.Settings.DAL.SettingHistory");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("From")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("TEXT")
                        .HasColumnName("From");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.ToTable("SettingHistories", "settings");

                    b.HasDiscriminator().HasValue(2);
                });

            modelBuilder.Entity("Log4Pro.CoreComponents.Settings.DAL.ModifySetting", b =>
                {
                    b.HasBaseType("Log4Pro.CoreComponents.Settings.DAL.SettingHistory");

                    b.Property<string>("From")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("TEXT")
                        .HasColumnName("From");

                    b.Property<string>("To")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("TEXT")
                        .HasColumnName("To");

                    b.ToTable("SettingHistories", "settings");

                    b.HasDiscriminator().HasValue(1);
                });
#pragma warning restore 612, 618
        }
    }
}
