﻿using Filer.DAL.DbModels;
namespace Filer.Services.DTOs
{
    public class MainDTO
    {
        public IEnumerable<DAL.DbModels.File> files { get; set; }
        public IEnumerable<Folder> folders { get; set; }
        public string? SrcParam { get; set; }
        public long? folderId { get; set; } 
    }
}
