/** 
* This file is part of the SqlRunner project.
* Copyright (c) 2014 Dai Nguyen
* Author: Dai Nguyen
**/

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Newtonsoft.Json;
using SqlRunner.DataAccess;
using SqlRunner.Models;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlRunner.ViewModels
{
    public class RunUserControlViewModel : BindableBase
    {
        private readonly QueryFileService _service;
        private readonly IAlertMessageService _alertService;
        private QueryFileBindable _queryFile;

        public RunUserControlViewModel(IAlertMessageService alertService)
        {
            _service = new QueryFileService();
            _alertService = alertService;
            Params = new ObservableCollection<SqlParamView>();
            SaveCommand = DelegateCommand.FromAsyncHandler(SaveSync, CanSave);
            DeleteCommand = DelegateCommand.FromAsyncHandler(DeleteAsync, CanDelete);
        }

        public QueryFileBindable QueryFile
        {
            get { return _queryFile; }
            private set { SetProperty(ref _queryFile, value); }
        }

        public ObservableCollection<SqlParamView> Params { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }

        public void LoadFromDropFile(string content)
        {            
            QueryFile.Id = 0;
            QueryFile.Name = "";
            QueryFile.Content = content;
            Params.Clear();

            MatchCollection matches = Regex.Matches(content, @"(?<!\w)@\w+");

            foreach (Match match in matches)
            {
                if (!Params.Any(t => t.Key == match.Value))
                    this.Params.Add(new SqlParamView(match.Value, ""));
            }
        }

        //public async void LoadFromSearchResult(SearchQueryFile result)
        //{
        //    using (QueryFileService service = new QueryFileService())
        //    {
        //        var found = await service.GetAsync(result.Id);

        //        if (found != null)
        //        {
        //            this.Id = found.Id;
        //            this.Name = found.Name;
        //            this.Content = found.Content;
        //            this.Params.Clear();

        //            var prams = JsonConvert.DeserializeObject<ObservableCollection<SqlParamView>>(found.Params);

        //            if (prams != null)
        //            {
        //                foreach (var p in prams)
        //                {
        //                    this.Params.Add(p);
        //                }
        //            }
        //        }
        //    }
        //}

        public bool CanSave()
        {
            // New
            if (Id == 0)
            {
                using (QueryFileService service = new QueryFileService())
                {
                    bool exist = service.All()
                        .Any(t => t.Name == this.Name);

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
         
        public async Task SaveSync()
        {
            try
            {
                if (QueryFile.Id == 0)
                    await _service.CreateAsync(QueryFile.ToEntity());
                else
                    await _service.UpdateAsync(QueryFile.ToEntity());
            }
            catch (Exception)
            {

            }
        }

        public async Task DeleteAsync()
        {
            try
            {
                await _service.DeleteAsync(QueryFile.Id);
            }
            catch (Exception)
            {

            }
        }

        private bool CanDelete()
        {
            return QueryFile.Id != 0;
        }
    }

    public class SqlParamView : BindableBase
    {
        private string key;
        public string Key
        {
            get { return key; }
            set { SetProperty(ref key, value); }            
        }

        private string val;
        public string Value
        {
            get { return val; }
            set { SetProperty(ref val, value); }            
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
