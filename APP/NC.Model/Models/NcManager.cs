using System;
using System.Collections.Generic;

namespace NC.Model.Models
{
    public partial class NcManager
    {
        public int Id { get; set; }
        public int? RoleId { get; set; }
        public int? RoleType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Avatar { get; set; }
        public string RealName { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public byte? IsAudit { get; set; }
        public int? IsLock { get; set; }
        public DateTime? AddTime { get; set; }
        public long? DeptId { get; set; }

        public NcManagerRole Role { get; set; }
    }
}
