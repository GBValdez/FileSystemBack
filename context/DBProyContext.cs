using back.models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using project.roles;
using project.users;

namespace project.Models;

public partial class DBProyContext : IdentityDbContext<userEntity, rolEntity, string>
{
    IConfiguration _configuration;
    public DBProyContext(DbContextOptions<DBProyContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<Files> Files { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));

}
