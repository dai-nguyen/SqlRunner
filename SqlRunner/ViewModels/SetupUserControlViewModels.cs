/** 
* This file is part of the SqlRunner project.
* Copyright (c) 2014 Dai Nguyen
* Author: Dai Nguyen
**/

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using SqlRunner.DataAccess;

namespace SqlRunner.ViewModels
{
    public class SetupUserControlViewModel : BindableBase
    {
        private string _server;
        public string Server 
        {
            get { return _server; }
            set
            {
                if (SetProperty(ref _server, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _database;
        public string Database 
        {
            get { return _database; }
            set
            {
                if (SetProperty(ref _database, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _user;
        public string User 
        {
            get { return _user; }
            set
            {
                if (SetProperty(ref _user, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _password;
        public string Password 
        {
            get { return _password; }
            set
            {
                if (SetProperty(ref _password, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


        private IAlertMessageService _alertService;

        public SetupUserControlViewModel(IAlertMessageService alertService)
        {
            _alertService = alertService;
            SaveCommand = new DelegateCommand(Save, CanSave);
            Load();
        }

        public DelegateCommand SaveCommand { get; private set; }
        
        public void Load()
        {
            _alertService.ShowMessage(true, "Loading...");

            Server = Properties.DbSettings.Default.Server;
            Database = Properties.DbSettings.Default.Database;
            User = Properties.DbSettings.Default.User;
            Password = Properties.DbSettings.Default.Password;

            _alertService.ShowMessage(false, "Ready");
        }

        public void Save()
        {
            _alertService.ShowMessage(true, "Saving...");

            // demo only. should encrypt data here
            Properties.DbSettings.Default.Server = Server;
            Properties.DbSettings.Default.Database = Database;
            Properties.DbSettings.Default.User = User;
            Properties.DbSettings.Default.Password = Password;
            Properties.DbSettings.Default.Save();

            _alertService.ShowMessage(false, "Saved");
        }

        public bool CanSave()
        {
            return !string.IsNullOrEmpty(Server)
                && !string.IsNullOrEmpty(Database)
                && !string.IsNullOrEmpty(User)
                && !string.IsNullOrEmpty(Password);
        }
    }
}
