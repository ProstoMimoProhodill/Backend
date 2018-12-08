﻿// <auto-generated />
using GraphLabs.Backend.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GraphLabs.Backend.Api.Migrations
{
    [DbContext(typeof(GraphLabsContext))]
    partial class GraphLabsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity("GraphLabs.Backend.Domain.TaskModule", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<string>("Version");

                    b.HasKey("Id");

                    b.ToTable("TaskModules");
                });

            modelBuilder.Entity("GraphLabs.Backend.Domain.TaskVariant", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<long>("TaskModuleId");

                    b.Property<string>("VariantData");

                    b.HasKey("Id");

                    b.HasIndex("TaskModuleId");

                    b.ToTable("TaskVariants");
                });

            modelBuilder.Entity("GraphLabs.Backend.Domain.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");
                });

            modelBuilder.Entity("GraphLabs.Backend.Domain.Student", b =>
                {
                    b.HasBaseType("GraphLabs.Backend.Domain.User");

                    b.Property<string>("Group");

                    b.HasIndex("Group");

                    b.HasDiscriminator().HasValue("Student");
                });

            modelBuilder.Entity("GraphLabs.Backend.Domain.Teacher", b =>
                {
                    b.HasBaseType("GraphLabs.Backend.Domain.User");

                    b.HasDiscriminator().HasValue("Teacher");
                });

            modelBuilder.Entity("GraphLabs.Backend.Domain.TaskVariant", b =>
                {
                    b.HasOne("GraphLabs.Backend.Domain.TaskModule", "TaskModule")
                        .WithMany("Variants")
                        .HasForeignKey("TaskModuleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
