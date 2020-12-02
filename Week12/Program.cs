using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.AspNetCore.Mvc;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureServices(services =>
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });
            services.AddDbContext<ApplicationContext>();
        })
        .Configure(app =>
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        });
    })
    .Build()
    .Run();

public class DesignTimeDbContextFactory :
   IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<ApplicationContext>().Options);
}

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public virtual DbSet<RegistrationForm> RegistrationForms { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<AssignCareer> AssignCareers { get; set; }

    public virtual DbSet<HomeAddress> HomeAddresses { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Career> Careers { get; set; }

    public virtual DbSet<HighSchoolResult> HighSchoolResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseLazyLoadingProxies().UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=HSU");

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<RegistrationForm>()
            .Property(p => p.CreationDate)
            .ValueGeneratedOnAdd();

        builder.Entity<RegistrationForm>()
            .Property(p => p.RecentEditedTime)
            .ValueGeneratedOnAddOrUpdate();
    }
}

[ApiController]
[Route("api/v1/[controller]")]
class FormController : ControllerBase
{
    private ApplicationContext _context;

    public FormController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Insert(RegistrationForm form)
    {
        _context.RegistrationForms.Add(form);
        _context.SaveChanges();
        return Ok(form);
    }
}
