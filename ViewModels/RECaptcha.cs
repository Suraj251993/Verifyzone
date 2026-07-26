using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgCheck.ViewModels
{
    public class RECaptcha
    {
        public string Key = "<RECaptcha Site Key>";

        public string Secret = "<RECaptcha Secret Key>";
        public string Response { get; set; }
    }
}
