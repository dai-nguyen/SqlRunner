/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using SqlRunner.DataAccess;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;
using System.Threading;
using SqlRunner.Models;
using System.Collections.Generic;

namespace SqlRunner.ViewModels
{
    public class DatabaseView : NotifyPropertyChanged, IDataErrorInfo
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
                    OnPropertyChanged("Server");
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
                    OnPropertyChanged("Database");
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
                    OnPropertyChanged("User");
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
                    OnPropertyChanged("Password");
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

        public async Task<bool> LoadAsync()
        {
            using (AppSetupService service = new AppSetupService())
            {
                var srv = await service.All()
                    .FirstOrDefaultAsync(t => t.Name == "Server");

                Server = srv != null ? srv.Value : "";

                var db = await service.All()
                    .FirstOrDefaultAsync(t => t.Name == "Database");

                Database = db != null ? db.Value : "";

                var usr = await service.All()
                    .FirstOrDefaultAsync(t => t.Name == "User");

                User = usr != null ? usr.Value : "";

                var pass = await service.All()
                    .FirstOrDefaultAsync(t => t.Name == "Password");

                Password = pass != null ? pass.Value : "";

                return true;
            }
        }

        public async Task<bool> SaveAsync(CancellationToken token)
        {
            List<bool> results = new List<bool>();

            using (AppSetupService service = new AppSetupService())
            {
                var srv = await service.All()
                    .FirstOrDefaultAsync(t => t.Name == "Server");

                if (srv != null)
                {
                    srv.Value = server;
                    results.Add(await service.UpdateAsync(srv, token) != null ? true : false);
                }
                else
                    results.Add(await service.CreateAsync(new AppSetup { Name = "Server", Value = server }, token) != null ? true : false);

                var db = await service.All()
                    .FirstOrDefaultAsync(t => t.Name == "Database");

                if (db != null)
                {
                    db.Value = database;
                    results.Add(await service.UpdateAsync(db, token) != null ? true : false);
                }
                else
                    results.Add(await service.CreateAsync(new AppSetup { Name = "Database", Value = database }, token) != null ? true : false);
                
                var usr = await service.All()
                    .FirstOrDefaultAsync(t => t.Name == "User");

                if (usr != null)
                {
                    usr.Value = user;
                    results.Add(await service.UpdateAsync(usr, token) != null ? true : false);
                }
                else
                    results.Add(await service.CreateAsync(new AppSetup { Name = "User", Value = user }, token) != null ? true : false);

                var pass = await service.All()
                    .FirstOrDefaultAsync(t => t.Name == "Password");

                if (pass != null)
                {
                    pass.Value = password;
                    results.Add(await service.UpdateAsync(pass, token) != null ? true : false);
                }
                else
                    results.Add(await service.CreateAsync(new AppSetup { Name = "Password", Value = password }, token) != null ? true : false);

                return results.Count(t => t == true) == 4;
            }
        }        
    }
}
