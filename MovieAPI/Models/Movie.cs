using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieAPI.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string Duration { get; set; }
        public int ReleaseYear { get; set; }
        
        //Annotation Ignore is for CSV Helper and will ignore these properties when mapping to csv.
        [Ignore]
        public int averageWatchDurationS { get; set; }
        [Ignore]
        public int watches { get; set; }
    }
}
