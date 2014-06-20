/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using SqlRunner.DataAccess;
using SqlRunner.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlRunner.ViewModel
{
    public class SqlFileViewModel : ViewModelBase, IDataErrorInfo
    {
        public ObservableCollection<SqlParamView> Params { get; set; }

        private Int64 id;
        public Int64 Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    RaisePropertyChanged("Id");
                    RaisePropertyChanged("IsNameEnabled");
                }
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public bool IsNameEnabled 
        { 
            get 
            { 
                return Id != 0 ? false : true; 
            }
            
        }

        private string content;
        public string Content
        {
            get { return content; }
            set
            {
                if (content != value)
                {
                    content = value;
                    RaisePropertyChanged("Content");
                }
            }
        }

        private DateTime dateCreated;
        public DateTime DateCreated
        {
            get { return dateCreated; }
            set
            {
                if (dateCreated != value)
                {
                    dateCreated = value;
                    RaisePropertyChanged("DateCreated");
                }
            }
        }

        private DateTime dateModified;
        public DateTime DateModified
        {
            get { return dateModified; }
            set
            {
                if (dateModified != value)
                {
                    dateModified = value;
                    RaisePropertyChanged("DateModified");
                }
            }
        }

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get 
            {
                if (columnName == "Name")
                {                    
                    if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                        return "Required";

                    using (QueryFileService service = new QueryFileService())
                    {
                        bool exist = service.All()
                            .Any(t => t.Name == this.Name);

                        return exist ? "Not Available" : null;
                    }
                }

                if (columnName == "Content")
                    return string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content) ? "Required" : null;

                return null;
            }
        }

        public RelayCommand SaveCommand { get; private set; }

        public SqlFileViewModel()
        {
            Id = 0;
            Params = new ObservableCollection<SqlParamView>();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;            
        }

        public SqlFileViewModel(QueryFile entity)
        {
            this.Id = entity.Id;
            this.Name = entity.Name;
            this.Content = entity.Content;
            this.Params = JsonConvert.DeserializeObject<ObservableCollection<SqlParamView>>(entity.Params);
        }

        public void LoadFromDropFile(string content)
        {
            this.Id = 0;
            this.Name = "";
            this.Content = content;
            this.Params.Clear();

            MatchCollection matches = Regex.Matches(content, @"(?<!\w)@\w+");

            foreach (Match match in matches)
            {
                if (!Params.Any(t => t.Key == match.Value))
                    this.Params.Add(new SqlParamView(match.Value, ""));
            }
        }

        public async void LoadFromSearchResult(SearchQueryFile result)
        {
            using (QueryFileService service = new QueryFileService())
            {
                var found = await service.GetAsync(result.Id);

                if (found != null)
                {
                    this.Id = found.Id;
                    this.Name = found.Name;
                    this.Content = found.Content;
                    this.Params.Clear();

                    var prams = JsonConvert.DeserializeObject<ObservableCollection<SqlParamView>>(found.Params);

                    if (prams != null)
                    {
                        foreach (var p in prams)
                        {
                            this.Params.Add(p);
                        }
                    }
                }
            }
        }

        public async Task<bool> CanSaveAsync()
        {
            // New
            if (Id == 0)
            {
                using (QueryFileService service = new QueryFileService())
                {
                    bool exist = await service.All()
                        .AnyAsync(t => t.Name == this.Name);

                    return !exist
                        && !string.IsNullOrEmpty(Name)
                        && !string.IsNullOrEmpty(Content);
                }
            }
            else // Update
            {
                return !string.IsNullOrEmpty(Name)
                && !string.IsNullOrEmpty(Content);
            }            
        }

        public QueryFile ToEntity()
        {
            return new QueryFile
            {
                Id = this.id,
                Name = this.name,
                Content = this.content,
                DateCreated = this.dateCreated,
                DateModified = this.dateModified,
                Params = JsonConvert.SerializeObject(this.Params)
            };
        }        

        public async Task<bool> SaveSync()
        {
            using (QueryFileService service = new QueryFileService())
            {
                if (this.id == 0)
                    return await service.CreateAsync(this.ToEntity()) != null ? true : false;
                else
                    return await service.UpdateAsync(this.ToEntity()) != null ? true : false;
            }
        }

        public async Task<bool> DeleteAsync()
        {
            if (this.Id != 0)
            {
                using (QueryFileService service = new QueryFileService())
                {
                    if (await service.DeleteAsync(Id))
                    {
                        Id = 0;
                        Name = "";
                        Content = "";
                        Params.Clear();
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class SqlParamView : ViewModelBase, IDataErrorInfo
    {
        private string key;
        public string Key
        {
            get { return key; }
            set
            {
                if (key != value)
                { 
                    key = value;
                    RaisePropertyChanged("Key");
                }
            }
        }

        private string val;
        public string Value
        {
            get { return val; }
            set
            {
                if (val != value)
                {
                    val = value;
                    RaisePropertyChanged("Value");
                }
            }
        }

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get 
            {
                if (columnName == "Key")
                    return (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key)) ? "Required" : null;

                if (columnName == "Value")
                    return (string.IsNullOrEmpty(val) || string.IsNullOrWhiteSpace(val)) ? "Required" : null;

                return null;
            }
        }
    
        public SqlParamView()
        {
            Key = "";
            Value = "";
        }

        public SqlParamView(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
