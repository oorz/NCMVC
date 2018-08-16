using System;
using System.Collections.Generic;

namespace NC.Model.Models
{
    public partial class NcManagerLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string ActionType { get; set; }
        public string Remark { get; set; }
        public string UserIp { get; set; }
        public DateTime? AddTime { get; set; }
    }
}
