using System;
using System.Collections.Generic;

namespace NC.Model.Models
{
    public partial class NcDictcache
    {
        public long CacheId { get; set; }
        public long ParentId { get; set; }
        public string ClassList { get; set; }
        public int? ClassLayer { get; set; }
        public int? SortId { get; set; }
        public string Title { get; set; }
        public string Depend { get; set; }
        public string CacheKey { get; set; }
        public string CacheExp { get; set; }
        public string CacheDesc { get; set; }
        public string CreatedName { get; set; }
        public DateTime CreatedTime { get; set; }
        public string UpdatedName { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public int Ostatus { get; set; }
    }
}
