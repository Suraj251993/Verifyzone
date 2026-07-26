using System;
using System.Collections.Generic;
using OrgCheck.Models;

namespace OrgCheck.DataAccess.Interfaces
{
    public interface IFileDA
    {
        int AddFile(File file);
        void UpdateFile(File file);
        void DeleteFile(int id);
        List<File> GetUploadedFiles(int userId);
    }
}
