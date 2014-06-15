﻿/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using SqlRunner.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace SqlRunner.DataAccess
{
    public class QueryFileService : DataService<QueryFile>, IDisposable
    {
        public QueryFileService()
            : base()
        { }

        public override async Task<QueryFile> CreateAsync(QueryFile entity)
        {
            entity.DateCreated = DateTime.Now;
            entity.DateModified = entity.DateCreated;
            return await base.CreateAsync(entity);
        }                

        public override async Task<QueryFile> UpdateAsync(QueryFile entity)
        {
            entity.DateModified = DateTime.Now;
            return await base.UpdateAsync(entity);
        }

        public async Task<List<SearchQueryFile>> LookupAsync(string search, CancellationToken token)
        {
            string keywords = "";

            if (search.Split(' ').Count(t => !string.IsNullOrEmpty(t) && !string.IsNullOrWhiteSpace(t)) >= 2)
                keywords = search.Replace(" ", " NEAR ");
            else
                keywords = "*" + search + "*";

            string sql = @"
select  q.Id, 
        q.Name, 
        q.DateCreated, 
        q.DateModified 
from    SearchQueryFile s 
        join QueryFile q on s.Id = q.Id 
where   s.Name MATCH @keywords
";
            var sparam = new System.Data.SQLite.SQLiteParameter("@keywords", keywords);
            return await Context.Database.SqlQuery<SearchQueryFile>(sql, sparam).ToListAsync(token);
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
