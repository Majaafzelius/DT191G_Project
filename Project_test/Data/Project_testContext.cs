using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project_test.Models;

namespace Project_test.Data
{
    public class Project_testContext : DbContext
    {
        public Project_testContext (DbContextOptions<Project_testContext> options)
            : base(options)
        {
        }

        public DbSet<Project_test.Models.Category> Category { get; set; } = default!;
        public DbSet<Project_test.Models.Product> Product { get; set; } = default!;
    }
}
