/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using SqlRunner.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SqlRunner.DataAccess
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
            Database.SetInitializer<DataContext>(null);
        }
        
        public DbSet<QueryFile> QueryFiles { get; set; }        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
