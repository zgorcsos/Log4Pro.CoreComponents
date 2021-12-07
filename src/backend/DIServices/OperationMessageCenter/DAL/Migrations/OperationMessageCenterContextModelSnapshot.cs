﻿// <auto-generated />
using System;
using Log4Pro.CoreComponents.DIServices.OperationMessageCenter.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Log4Pro.CoreComponents.CoreComponents.DIServices.OperationMessageCenter.DAL.Migrations
{
    [DbContext(typeof(OperationMessageCenterContext))]
    partial class OperationMessageCenterContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Log4Pro.CoreComponents.DIServices.OperationMessageCenter.DAL.AdditionalMessageData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DataKey")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("DataValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OperationMessageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DataKey");

                    b.HasIndex("OperationMessageId");

                    b.ToTable("AdditionalMessageDatas", "omcenter");
                });

            modelBuilder.Entity("Log4Pro.CoreComponents.DIServices.OperationMessageCenter.DAL.OperationMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Handled")
                        .HasColumnType("bit");

                    b.Property<string>("HandledBy")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("HandledTimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("Instance")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MessageCategory")
                        .HasColumnType("int");

                    b.Property<string>("Module")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("OtherFilter")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Thread")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Handled");

                    b.HasIndex("HandledTimeStamp");

                    b.HasIndex("Instance");

                    b.HasIndex("MessageCategory");

                    b.HasIndex("Module");

                    b.HasIndex("OtherFilter");

                    b.HasIndex("Thread");

                    b.HasIndex("TimeStamp");

                    b.ToTable("OperationMessages", "omcenter");
                });

            modelBuilder.Entity("Log4Pro.CoreComponents.DIServices.OperationMessageCenter.DAL.AdditionalMessageData", b =>
                {
                    b.HasOne("Log4Pro.CoreComponents.DIServices.OperationMessageCenter.DAL.OperationMessage", "OperationMessage")
                        .WithMany("AdditionalMessageDatas")
                        .HasForeignKey("OperationMessageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("OperationMessage");
                });

            modelBuilder.Entity("Log4Pro.CoreComponents.DIServices.OperationMessageCenter.DAL.OperationMessage", b =>
                {
                    b.Navigation("AdditionalMessageDatas");
                });
#pragma warning restore 612, 618
        }
    }
}
