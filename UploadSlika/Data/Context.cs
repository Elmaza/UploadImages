using Microsoft.EntityFrameworkCore;
using UploadImages.Api.Models;

namespace UploadImages.Api.Data
{
    public class Context : DbContext
    {

        public Context(DbContextOptions<Context> options) : base(options)
        {

        }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(Configuration.GetConnectionString("Context"));
        //}
        public DbSet<Image> Images { get; set; }

    }
}
