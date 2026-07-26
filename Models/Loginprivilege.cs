using System;
using System.Collections.Generic;

#nullable disable

namespace OrgCheck.Models
{
    public partial class Loginprivilege
    {
        public int Id { get; set; }
        public int Loginid { get; set; }
        public bool Exempverification { get; set; }
        public bool Emplverification { get; set; }
        public bool Studentverification { get; set; }
        public int Status { get; set; }

        public virtual Login Login { get; set; }
    }
}
