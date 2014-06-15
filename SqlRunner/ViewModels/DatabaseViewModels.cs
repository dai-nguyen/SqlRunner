/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using GalaSoft.MvvmLight;

namespace SqlRunner.ViewModels
{
    public class DatabaseView : ViewModelBase
    {
        private string server;
        public string Server 
        {
            get { return server; }
            set
            {
                if (server != value)
                {
                    server = value;
                    RaisePropertyChanged("Server");
                }
            }
        }

        private string database;
        public string Database 
        {
            get { return database; }
            set
            {
                if (database != value)
                {
                    database = value;
                    RaisePropertyChanged("Database");
                }
            }
        }

        private string user;
        public string User 
        {
            get { return user; }
            set
            {
                if (user != value)
                {
                    user = value;
                    RaisePropertyChanged("User");
                }
            }
        }

        private string password;
        public string Password 
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    RaisePropertyChanged("Password");
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
                if (columnName == "Server")
                    return (string.IsNullOrEmpty(server) || string.IsNullOrWhiteSpace(server)) ? "Required" : null;

                if (columnName == "Database")
                    return (string.IsNullOrEmpty(database) || string.IsNullOrWhiteSpace(database)) ? "Required" : null;

                if (columnName == "User")
                    return (string.IsNullOrEmpty(user) || string.IsNullOrWhiteSpace(user)) ? "Required" : null;

                if (columnName == "Password")
                    return (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password)) ? "Required" : null;

                return null;
            }
        }

        public void Load()
        {
            Server = Properties.DbSettings.Default.Server;
            Database = Properties.DbSettings.Default.Database;
            User = Properties.DbSettings.Default.User;
            Password = Properties.DbSettings.Default.Password;
        }

        public void Save()
        {
            Properties.DbSettings.Default.Server = Server;
            Properties.DbSettings.Default.Database = Database;
            Properties.DbSettings.Default.User = User;
            Properties.DbSettings.Default.Password = Password;
            Properties.DbSettings.Default.Save();
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Server)
                && !string.IsNullOrEmpty(Database)
                && !string.IsNullOrEmpty(User)
                && !string.IsNullOrEmpty(Password);
        }        
    }
}
