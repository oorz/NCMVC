using System;
using System.Collections.Generic;

namespace NC.Model.Models
{
    public partial class NcManagerRoleValue
    {
        public int Id { get; set; }
        public int? RoleId { get; set; }
        public string NavName { get; set; }
        public string ActionType { get; set; }

        public NcManagerRole Role { get; set; }
    }
}
