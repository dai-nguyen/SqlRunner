/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using Microsoft.Practices.Prism.Mvvm;
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

    public class QueryFileBindable : BindableBase
    {
        private Int64 id;
        public Int64 Id 
        { 
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private string name;
        public string Name 
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string content;
        public string Content 
        {
            get { return content; }
            set { SetProperty(ref content, value); }
        }

        private string param;
        public string Param
        {
            get { return param; }
            set { SetProperty(ref param, value); }
        }

        private DateTime dateCreated;
        public DateTime DateCreated 
        {
            get { return dateCreated; }
            set { SetProperty(ref dateCreated, value); }
        }

        private DateTime dateModified;
        public DateTime DateModified 
        {
            get { return dateModified; }
            set { SetProperty(ref dateModified, value); }
        }

        public QueryFileBindable()
        {
            DateCreated = DateTime.Now;
            DateModified = DateCreated;
        }

        public QueryFileBindable(QueryFile model)
        {
            Id = model.Id;
            Name = model.Name;
            Content = model.Content;
            Param = model.Params;
            DateCreated = model.DateCreated;
            DateModified = model.DateModified;
        }

        public QueryFile ToEntity()
        {
            return new QueryFile
            {
                Id = this.Id,
                Name = this.Name,
                Content = this.Content,
                Params = this.Param,
                DateCreated = this.DateCreated,
                DateModified = this.DateModified
            };
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
