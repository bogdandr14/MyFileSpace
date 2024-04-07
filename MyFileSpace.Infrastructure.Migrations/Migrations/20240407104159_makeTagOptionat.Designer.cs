﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyFileSpace.Infrastructure.Persistence;

#nullable disable

namespace MyFileSpace.Infrastructure.Migrations.Migrations
{
    [DbContext(typeof(MyFileSpaceDbContext))]
    [Migration("20240407104159_makeTagOptionat")]
    partial class makeTagOptionat
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.AccessKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nchar(255)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.ToTable("AccessKey");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.DirectoryAccessKey", b =>
                {
                    b.Property<Guid>("DirectoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessKeyId")
                        .HasColumnType("int");

                    b.HasKey("DirectoryId", "AccessKeyId");

                    b.HasIndex("AccessKeyId")
                        .IsUnique();

                    b.HasIndex("DirectoryId")
                        .IsUnique();

                    b.ToTable("DirectoryAccessKey");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.FileAccessKey", b =>
                {
                    b.Property<Guid>("FileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessKeyId")
                        .HasColumnType("int");

                    b.HasKey("FileId", "AccessKeyId");

                    b.HasIndex("AccessKeyId")
                        .IsUnique();

                    b.HasIndex("FileId")
                        .IsUnique();

                    b.ToTable("FileAccessKey");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.FileLabel", b =>
                {
                    b.Property<int>("LabelId")
                        .HasColumnType("int");

                    b.Property<Guid>("FileId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LabelId", "FileId");

                    b.HasIndex("FileId");

                    b.ToTable("FileLabel");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.Label", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Label");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.StoredFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte>("AccessLevel")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasDefaultValue((byte)1);

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

                    b.Property<Guid>("DirectorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModifiedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("SizeInBytes")
                        .HasColumnType("bigint");

                    b.Property<bool>("State")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.HasKey("Id");

                    b.HasIndex("DirectorId");

                    b.HasIndex("OwnerId");

                    b.ToTable("StoredFile");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(321)
                        .HasColumnType("nvarchar(321)");

                    b.Property<DateTime>("LastPasswordChange")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<byte>("Role")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasDefaultValue((byte)0);

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("TagName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("TagName")
                        .IsUnique()
                        .HasFilter("[TagName] IS NOT NULL");

                    b.ToTable("User");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.UserDirectoryAccess", b =>
                {
                    b.Property<Guid>("DirectoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AllowedUserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("DirectoryId", "AllowedUserId");

                    b.HasIndex("AllowedUserId");

                    b.ToTable("UserDirectoryAccess");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.UserFileAccess", b =>
                {
                    b.Property<Guid>("FileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AllowedUserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("FileId", "AllowedUserId");

                    b.HasIndex("AllowedUserId");

                    b.ToTable("UserFileAccess");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.VirtualDirectory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte>("AccessLevel")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasDefaultValue((byte)1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ParentDirectoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("State")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("VirtualPath")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ParentDirectoryId");

                    b.ToTable("VirtualDirectory");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.DirectoryAccessKey", b =>
                {
                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.AccessKey", "AccessKey")
                        .WithOne("DirectoryAccess")
                        .HasForeignKey("MyFileSpace.Infrastructure.Persistence.Entities.DirectoryAccessKey", "AccessKeyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.VirtualDirectory", "AccessibleDirectory")
                        .WithOne("DirectoryAccessKey")
                        .HasForeignKey("MyFileSpace.Infrastructure.Persistence.Entities.DirectoryAccessKey", "DirectoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccessKey");

                    b.Navigation("AccessibleDirectory");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.FileAccessKey", b =>
                {
                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.AccessKey", "AccessKey")
                        .WithOne("FileAccess")
                        .HasForeignKey("MyFileSpace.Infrastructure.Persistence.Entities.FileAccessKey", "AccessKeyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.StoredFile", "AccessibleFile")
                        .WithOne("FileAccessKey")
                        .HasForeignKey("MyFileSpace.Infrastructure.Persistence.Entities.FileAccessKey", "FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccessKey");

                    b.Navigation("AccessibleFile");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.FileLabel", b =>
                {
                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.StoredFile", "File")
                        .WithMany("Labels")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.Label", "Label")
                        .WithMany("FilesWithLabel")
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");

                    b.Navigation("Label");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.Label", b =>
                {
                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.User", "Owner")
                        .WithMany("Labels")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.StoredFile", b =>
                {
                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.VirtualDirectory", "Directory")
                        .WithMany("FilesInDirectory")
                        .HasForeignKey("DirectorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.User", "Owner")
                        .WithMany("Files")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Directory");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.UserDirectoryAccess", b =>
                {
                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.User", "AllowedUser")
                        .WithMany("AllowedDirectories")
                        .HasForeignKey("AllowedUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.VirtualDirectory", "Directory")
                        .WithMany("AllowedUsers")
                        .HasForeignKey("DirectoryId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("AllowedUser");

                    b.Navigation("Directory");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.UserFileAccess", b =>
                {
                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.User", "AllowedUser")
                        .WithMany("AllowedFiles")
                        .HasForeignKey("AllowedUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.StoredFile", "File")
                        .WithMany("AllowedUsers")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AllowedUser");

                    b.Navigation("File");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.VirtualDirectory", b =>
                {
                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.User", "Owner")
                        .WithMany("Directories")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MyFileSpace.Infrastructure.Persistence.Entities.VirtualDirectory", "ParentDirectory")
                        .WithMany("ChildDirectories")
                        .HasForeignKey("ParentDirectoryId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Owner");

                    b.Navigation("ParentDirectory");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.AccessKey", b =>
                {
                    b.Navigation("DirectoryAccess");

                    b.Navigation("FileAccess");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.Label", b =>
                {
                    b.Navigation("FilesWithLabel");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.StoredFile", b =>
                {
                    b.Navigation("AllowedUsers");

                    b.Navigation("FileAccessKey");

                    b.Navigation("Labels");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.User", b =>
                {
                    b.Navigation("AllowedDirectories");

                    b.Navigation("AllowedFiles");

                    b.Navigation("Directories");

                    b.Navigation("Files");

                    b.Navigation("Labels");
                });

            modelBuilder.Entity("MyFileSpace.Infrastructure.Persistence.Entities.VirtualDirectory", b =>
                {
                    b.Navigation("AllowedUsers");

                    b.Navigation("ChildDirectories");

                    b.Navigation("DirectoryAccessKey");

                    b.Navigation("FilesInDirectory");
                });
#pragma warning restore 612, 618
        }
    }
}
