/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using System;

namespace SqlRunner.Models
{
    public class FileLog
    {
        public Int64 Id { get; set; }
        public string FileName { get; set; }
        public string Params { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
