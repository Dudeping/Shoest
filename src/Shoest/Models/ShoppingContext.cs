using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Shopping.Models
{
    public class ShoppingContext:DbContext
    {
        public ShoppingContext() : base("name=connstr")
        {

        }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Member> Members { get; set; }

        public DbSet<OrderHeader> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetailItems { get; set; }
    }
}
