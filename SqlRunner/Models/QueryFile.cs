/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using System;

namespace SqlRunner.Models
{
    public class QueryFile
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Params { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public QueryFile()
        {
            DateCreated = DateTime.Now;
            DateModified = DateCreated;
        }
    }

    public class SearchQueryFile
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
