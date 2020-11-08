﻿// <auto-generated />
using System;
using DotnetExercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DotnetExercises.Migrations
{
    [DbContext(typeof(AppContext))]
    [Migration("20201108041253_InitSchema")]
    partial class InitSchema
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9");

            modelBuilder.Entity("Bookstore.Book", b =>
                {
                    b.Property<int?>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Author")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreationDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastEdited")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT");

                    b.Property<string>("Publisher")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("BookId");

                    b.ToTable("Books");
                });
#pragma warning restore 612, 618
        }
    }
}
