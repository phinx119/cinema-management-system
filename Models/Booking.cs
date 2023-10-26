using System;
using System.Collections.Generic;

namespace PRN_ASG3.Models
{
    public partial class Booking
    {
        public int BookingId { get; set; }
        public int ShowId { get; set; }
        public string? Name { get; set; }
        public string? SeatStatus { get; set; }
        public decimal? Amount { get; set; }
    }

}
