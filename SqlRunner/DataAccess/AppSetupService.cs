/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/


using SqlRunner.Models;
using System;

namespace SqlRunner.DataAccess
{
    public class AppSetupService : DataService<AppSetup>, IDisposable
    {
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
