using OrgCheck.ViewModels;
using System;
using System.Collections.Generic;

namespace OrgCheck.Services.Interfaces
{
    public interface IFileService
    {
        int AddFile(FileViewModel viewModel);
        bool DeleteFile(int id);
        List<FileViewModel> GetUploadedFiles(int userId);
        bool ApproveFile(int fileId);
        bool RejectFile(int fileId);
    }
}
