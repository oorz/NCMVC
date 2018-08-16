using System;
using System.Collections.Generic;

namespace NC.Model.Models
{
    public partial class NcManagerDept
    {
        public long DeptId { get; set; }
        public long? ParentId { get; set; }
        public string ClassList { get; set; }
        public int? ClassLayer { get; set; }
        public int? SortId { get; set; }
        public string DeptName { get; set; }
        public string DeptDesc { get; set; }
        public long? CreatedId { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedId { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public int Ostatus { get; set; }
        public int? IsAudit { get; set; }
    }
}
