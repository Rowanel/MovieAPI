using System;
using System.Collections.Generic;
using System.Globalization;
using MovieAPI.Data;
using MovieAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MovieAPI.Controllers
{
	[ApiController]
	[Route("metadata")]
	public class MovieAPIController : ControllerBase
	{
        private readonly ILogger<MovieAPIController> _logger;

        public MovieAPIController(ILogger<MovieAPIController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id}")]
		public ActionResult<object> GetMovie(int id)
		{
			IEnumerable<object> MovieObj = DataAdapter.GetInstance().GetMovie_ByID(id);
			if (MovieObj == null)
				return NotFound();

			return Ok(MovieObj);
		}

		[HttpGet]
		[Route("moviestats")]
		public IEnumerable<object> GetMovieStats()
		{
			return DataAdapter.GetInstance().Fullstats();
		}

		[HttpPost]
		public IActionResult Post(Movie movie)
        {
			DataAdapter.GetInstance().PostMeta(movie);
			return Ok(movie);
        }
	}
}
