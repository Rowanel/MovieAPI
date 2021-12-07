using CsvHelper;
using CsvHelper.Configuration;
using MovieAPI.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MovieAPI.Data
{
    public class DataAdapter
    {
        private static DataAdapter _DataAdapter = null;
        public static DataAdapter GetInstance()
        {
            if (_DataAdapter == null)
            {
                initialiseMovies();
                initialiseStats();
                _DataAdapter = new DataAdapter();
            }
            return _DataAdapter;
        }

        private static string metadataPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\data\\csvdata\\metadata.csv";
        private static string statsPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\data\\csvdata\\stats.csv";

        private static List<Movie> movies;
        private static List<Stats> moviestats;

        private static void initialiseMovies()
        { 
            
            if (new FileInfo(metadataPath).Exists == false)
                return;

            using (var csv = new CsvReader(File.OpenText(metadataPath), System.Globalization.CultureInfo.CurrentCulture))
            {
                var imovies = csv.GetRecords<Movie>();
                movies = imovies.ToList();
            };
        }

        private static void initialiseStats()
        {
            if (new FileInfo(statsPath).Exists == false)
                return;

            using (var csv = new CsvReader(File.OpenText(statsPath), System.Globalization.CultureInfo.CurrentCulture))
            {
                var istats = csv.GetRecords<Stats>();
                moviestats = istats.ToList();
            };
        }

        public IEnumerable<object> GetMovie_ByID(int id)
        {
            if (movies == null)
            {
                return null;
            }

            //Check if any records exist for movie before attempting to return an ienumerable list
            Movie findMovie = movies.Find(x => x.MovieId == id);
            if (findMovie == null)
                return null;

            var returnList = (from m in movies
                              where m.MovieId == id
                              orderby m.Id descending
                              group new { m } by new { m.MovieId, m.Language } into g
                              select new { MovieId = g.Key.MovieId, Title = g.FirstOrDefault().m.Title, Language = g.Key.Language, Duration = g.FirstOrDefault().m.Duration, ReleaseYear = g.FirstOrDefault().m.ReleaseYear });


            return returnList.ToList();
        }

        public IEnumerable<object> Fullstats()
        {
            if (movies == null)
            {
                return null;
            }

            //Group the Movie by it's corresponding stats as there could be thousands of stats per movie. Order by Watches DESC and then by Release Year desc
            var returnList = (from m in movies
                              join s in moviestats on m.MovieId equals s.movieId
                              group new {m, s} by new { m.MovieId} into g
                              select new { 
                                  movieid = g.Key.MovieId, 
                                  Title = g.FirstOrDefault().m.Title, 
                                  averageWatchDurationS = Math.Round(TimeSpan.FromMilliseconds(g.Average(a => a.s.watchDurationMs)).TotalSeconds, 0, MidpointRounding.AwayFromZero), 
                                  watches = g.Select(c => c.s).Distinct().Count(), 
                                  releaseyear = g.FirstOrDefault().m.ReleaseYear
                              }).OrderByDescending(x => x.watches).ThenByDescending(x => x.releaseyear);
            
            return returnList.ToList();
        }

        public bool PostMeta(Movie movie)
        {
            var config = new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture) { HasHeaderRecord = false};

            using (var stream = File.Open(metadataPath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer,config))
            {
                csv.NextRecord();
                //Increment ID by last max number in meta data file.
                movie.Id = movies.Max(m => m.Id) + 1;
                csv.WriteRecord(movie);
            }

            movies.Add(movie);

            return true;
        }
    }
}
