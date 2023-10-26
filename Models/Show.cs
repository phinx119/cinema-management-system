using System;
using System.Collections.Generic;

namespace PRN_ASG3.Models
{
    public partial class Show
    {
        public int ShowId { get; set; }
        public int RoomId { get; set; }
        public int FilmId { get; set; }
        public DateTime? ShowDate { get; set; }
        public decimal? Price { get; set; }
        public bool? Status { get; set; }
        public int? Slot { get; set; }
    }

    public class MutipleJoinClass
    {
        public Show show { get; set; }
        public Room room { get; set; }
        public Film film { get; set; }
    }
}
