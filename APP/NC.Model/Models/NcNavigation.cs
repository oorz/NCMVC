using System;
using System.Collections.Generic;

namespace NC.Model.Models
{
    public partial class NcNavigation
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int? ChannelId { get; set; }
        public string NavType { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string IconUrl { get; set; }
        public string LinkUrl { get; set; }
        public int? SortId { get; set; }
        public byte? IsLock { get; set; }
        public string Remark { get; set; }
        public string ActionType { get; set; }
        public byte? IsSys { get; set; }
    }
}
