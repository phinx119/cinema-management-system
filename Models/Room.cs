using System;
using System.Collections.Generic;

namespace PRN_ASG3.Models
{
    public partial class Room
    {
        public int RoomId { get; set; }
        public string? Name { get; set; }
        public int? NumberRows { get; set; }
        public int? NumberCols { get; set; }
    }
}
