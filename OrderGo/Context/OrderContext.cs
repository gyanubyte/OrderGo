using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderGo.Models;
using System.Collections.Generic;

namespace OrderGo
{
    public class OrderContext:DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}
