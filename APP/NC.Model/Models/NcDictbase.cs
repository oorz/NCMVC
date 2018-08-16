using System;
using System.Collections.Generic;

namespace NC.Model.Models
{
    public partial class NcDictbase
    {
        public int Id { get; set; }
        public string KeyType { get; set; }
        public string KeyCode { get; set; }
        public string KeyValue { get; set; }
        public string CodeValue { get; set; }
        public int? SortId { get; set; }
        public int Ostatus { get; set; }
    }
}
