using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using OrgCheck.DataAccess.Interfaces;
using OrgCheck.Middleware;
using OrgCheck.Models;
using OrgCheck.Services.Interfaces;
using OrgCheck.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgCheck.Services
{
    public class FileService : IFileService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ExecutionContext _executionContext;
        private readonly IMapper _mapper;
        public FileService(IServiceProvider serviceProvider, ExecutionContext executionContext, IMapper mapper)
        {
            _serviceProvider = serviceProvider;
            _executionContext = executionContext;
            _mapper = mapper;
        }
        public int AddFile(FileViewModel viewModel)
        {
            var record = _mapper.Map<File>(viewModel);
            record.Uploadedby = _executionContext.UserId;
            record.Uploadeddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            record.Uploadedstatus = 1;
            record.Status = 1;
            int id = _serviceProvider.GetRequiredService<IFileDA>().AddFile(record);
            return id;
        }
        public bool DeleteFile(int id)
        {
            _serviceProvider.GetRequiredService<IFileDA>().DeleteFile(id);
            return true;
        }
        public List<FileViewModel> GetUploadedFiles(int userId)
        {
            var listviews = new List<FileViewModel>();
            var lists = _serviceProvider.GetRequiredService<IFileDA>().GetUploadedFiles(userId);
            foreach (var item in lists)
            {
                var record = new FileViewModel()
                {
                    Id = item.Id,
                    Filename = item.Filename,
                    Filesize = item.Filesize,
                    ValidRecords = item.Validrecords,
                    InvalidRecords = item.Invalidrecords,
                    TotalRecords = item.Totalrecords,
                    Uploadeddate = item.Uploadeddate.Value.ToString("dd/MM/yyyy")
                };
                if (item.Uploadedstatus == 1)
                    record.UploadedStatus = "Uploaded";
                else if (item.Uploadedstatus == 2)
                    record.UploadedStatus = "Approved";
                else if (item.Uploadedstatus == 3)
                    record.UploadedStatus = "Rejected";
                listviews.Add(record);
            }
            return listviews;
        }
        public bool ApproveFile(int fileId)
        {
            var record = new File()
            {
                Id = fileId,
                Uploadedstatus = 2
            };
            _serviceProvider.GetRequiredService<IFileDA>().UpdateFile(record);
            return true;
        }
        public bool RejectFile(int fileId)
        {
            var record = new File()
            {
                Id = fileId,
                Uploadedstatus = 3
            };
            _serviceProvider.GetRequiredService<IFileDA>().UpdateFile(record);
            return true;
        }
    }
}
