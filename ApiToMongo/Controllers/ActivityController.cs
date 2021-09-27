using ApiToMongo.Models;
using DataLibrary;
using DataLibrary.Entity;
using DataLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiToMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IDbContext _dbContex;
        public ActivityController(IDbContext dbContext)
        {
            _dbContex = dbContext;
        }

        [HttpGet("all")]
        [Authorize(Policy = "RequireLoggedIn")]
        public async Task<IActionResult> GetAllTweets()
        {
            var records = await _dbContex.LoadRecordsAsync<TweetModel>("Tweets");
            return Ok(records);
        }

        [HttpGet("[action]/{username}")]
        [Authorize(Policy = "RequireLoggedIn")]
        public async Task<IActionResult> GetTweets([FromRoute] String username)
        {
            var entity = (_dbContex.LoadContext("Tweets") as TweetEntity);
            var filter = entity.GetUserTweets<TweetModel>(username);

            var record = await _dbContex.LoadRecordByFilterAsync("Tweets", filter);

            return Ok(record);
        }

        [Authorize]
        [HttpPost("{username}/add")]
        public async Task<IActionResult> CreateTweet([FromBody] TweetViewModel formdata,[FromRoute] String username)
        {
            if(ModelState.IsValid)
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if(identity !=null)
                {
                    var fullUsername = identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                    var model = new TweetModel()
                    {
                        Tweet = formdata.Tweet,
                        Tag = formdata.Tag,
                        CreateDateTime = DateTime.Now,
                        Username = fullUsername.Value,
                        LoginId = username
                    };

                    await _dbContex.InsertRecordAsync("Tweets", model);

                    return Created("Tweet created successfully", null);
                }

                return BadRequest("No valid token");
                
            }

            return BadRequest("One or more validation error");
        }

        [HttpPut("{username}/update/{id}")]
        public async Task<IActionResult> UpdateTweet([FromBody] TweetModel formdata)
        {
            if(ModelState.IsValid)
            {
                await _dbContex.UpsertRecordAsync<TweetModel>("Tweets", formdata.Id, formdata);

                return Ok("Tweet updated successfully");
            }
            return BadRequest("One or more validation error");

        }

        [HttpDelete("{username}/delete/{id}")]
        public async Task<IActionResult> DeleteTweet([FromRoute] String username,String id)
        {
            var result = await _dbContex.LoadRecordByIdAsync<TweetModel>("Tweets", new Guid(id));
            if(result !=null)
            {
                await  _dbContex.DeleteRecordAsync<TweetModel>("Tweets", new Guid(id));
                return Ok("Tweet deleted successfully");
            }
            return BadRequest("Tweet does not exist");

        }

        [Authorize]
        [HttpPut("{username}/like/{id}")]
        public async Task<IActionResult> LikeDislikeTweet([FromRoute] String username,String id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var fullUsername = identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                TweetLikeModel formdata = new TweetLikeModel
                {
                    TweetId = new Guid(id),
                    LoginId = username,
                    CreateDateTime = DateTime.Now,
                    Username =fullUsername.Value,
                    Like = true
                    };

                if (ModelState.IsValid)
                {
                    await _dbContex.UpsertRecordAsync<TweetLikeModel>("TweetsLike", formdata.Id, formdata);

                    return Ok("Tweet updated successfully");
                }
            }
                
            return BadRequest("One or more validation error");

        }




    }
}
