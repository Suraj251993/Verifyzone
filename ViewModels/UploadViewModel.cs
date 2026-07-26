using Microsoft.AspNetCore.Http;

namespace OrgCheck.ViewModels
{
    public class UploadViewModel
    {
        public IFormFile file { get; set; }
        public string user { get; set; }
    }
}
