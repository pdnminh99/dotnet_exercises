#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DotnetExercises.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotnetExercises
{
    [ApiController]
    [Route("[controller]")]
    public abstract class GenericController<TCtx, TM> : ControllerBase
        where TCtx : DbContext where TM : class
    {
        private readonly TCtx _context;

        protected GenericController(TCtx context) => _context = context;

        public async Task<List<TM>> GetAll() => await _context.Set<TM>().ToListAsync();

        [HttpPost]
        public async Task<TM> Insert(TM entity)
        {
            entity = (await _context.Set<TM>().AddAsync(entity)).Entity;
            await _context.SaveChangesAsync();
            return entity;
        }

        [HttpPatch]
        public async Task<TM?> Update(TM entity)
        {
            EntityEntry<TM> entityEntry = _context.Set<TM>().Update(entity);
            await _context.SaveChangesAsync();
            return entityEntry.Entity;
        }

        [HttpDelete]
        [Route("{entryId}")]
        public async Task<TM?> Delete(int entryId)
        {
            TM entryToDelete = await _context.Set<TM>().FindAsync(entryId);
            if (entryToDelete == null) return null;
            EntityEntry<TM> entityEntry = _context.Set<TM>().Remove(entryToDelete);
            await _context.SaveChangesAsync();
            return entityEntry.Entity;
        }
    }

    public class StudentController : GenericController<dbfirstContext, Students>
    {
        public StudentController(dbfirstContext context) : base(context)
        {
        }
    }

    public class BookController : GenericController<AppContext, Book>
    {
        public BookController(AppContext context) : base(context)
        {
        }
    }

    public class Book
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? BookId { get; set; }

#nullable enable

        [Required(AllowEmptyStrings = false, ErrorMessage = "`Title` field must not empty or null.")]
        public string? Title { get; set; }

        public string? Author { get; set; }

        public string? Publisher { get; set; }

        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreationDate
        {
            get => _creationdate ?? DateTime.Now;
            set => _creationdate = value;
        }

        private DateTime? _creationdate;

        public DateTime? CreatedAt => CreationDate;

        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? LastEdited
        {
            get => _lastEditedDate ?? DateTime.Now;
            set => _lastEditedDate = value;
        }

        public DateTime? LastEditedAt => LastEdited;

        private DateTime? _lastEditedDate;
    }

    public class AppContext : DbContext
    {
        public virtual DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite("DataSource=codefirst.db");

#nullable disable
    }

    public static class Week8
    {
        public static void Start(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services =>
                    {
                        // Add code first context
                        services.AddDbContext<AppContext>();

                        // Add Db first context
                        services.AddDbContext<dbfirstContext>();

                        services.AddControllers()
                            .AddJsonOptions(options =>
                            {
                                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                                options.JsonSerializerOptions.IgnoreNullValues = true;
                            });
                    }).Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
                    });
                })
                .Build()
                .Run();
    }
}