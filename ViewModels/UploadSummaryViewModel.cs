using System;
using System.Collections.Generic;

namespace OrgCheck.ViewModels
{
    public class UploadSummaryViewModel
    {
        public int totalrecords { get; set; }
        public int validrecords { get; set; }
        public int invalidrecords { get; set; }
        public List<ErrorLogViewModel> errors { get; set; }
        public int fileid { get; set; }
    }
    public class ErrorLogViewModel
    {
        public string errorcode { get; set; }
        public string errordescription { get; set; }
    }
}
