using System;
using System.Collections.Generic;

namespace NC.Model.Models
{
    public partial class NcManagerRole
    {
        public NcManagerRole()
        {
            NcManager = new HashSet<NcManager>();
            NcManagerRoleValue = new HashSet<NcManagerRoleValue>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; }
        public byte? RoleType { get; set; }
        public byte? IsSys { get; set; }

        public ICollection<NcManager> NcManager { get; set; }
        public ICollection<NcManagerRoleValue> NcManagerRoleValue { get; set; }
    }
}
