using System;
using System.Collections.Generic;
using System.Linq;
using OrgCheck.Models;
using OrgCheck.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace OrgCheck.DataAccess
{
    public class FileDA : IFileDA
    {
        public PostgresContext orgCheckContext;
        public FileDA(PostgresContext _orgCheckContext)
        {
            orgCheckContext = _orgCheckContext;
        }
        public int AddFile(File file)
        {
            orgCheckContext.Files.Add(file);
            orgCheckContext.SaveChanges();
            return file.Id;
        }
        public void UpdateFile(File file)
        {
            var existingEntity = orgCheckContext.Files.FirstOrDefault(_ => _.Id == file.Id);
            existingEntity.Uploadedstatus = file.Uploadedstatus;
            orgCheckContext.SaveChanges();
        }
        public void DeleteFile(int id)
        {
            var existingEntity = orgCheckContext.Files.FirstOrDefault(_ => _.Id == id);
            existingEntity.Status = 0;
            orgCheckContext.SaveChanges();
        }
        public List<File> GetUploadedFiles(int userId)
        {
            return orgCheckContext.Files.AsNoTracking().Where(_ => _.Uploadedby == userId && _.Status == 1).OrderByDescending(_ => _.Id)
                .Take(10).ToList();
        }
    }
}
