using System;
using System.Collections.Generic;

namespace PRN_ASG3.Models
{
    public partial class Film
    {
        public int FilmId { get; set; }
        public int GenreId { get; set; }
        public string Title { get; set; } = null!;
        public int Year { get; set; }
        public string CountryCode { get; set; } = null!;
        public string? FilmUrl { get; set; }
    }
}
