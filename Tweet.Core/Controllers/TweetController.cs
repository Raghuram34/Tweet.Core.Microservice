﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet.Core.Attributes;
using Tweet.Core.Models;
using Tweet.Core.Services.Abstractions;

namespace Tweet.Core.Controllers
{
    [Authorize]
    [CustomExceptionAttibute]
    [Route("api/[controller]")]
    [ApiController]
    public class TweetController : ControllerBase
    {
        private readonly ITweetService _tweetService;
        private readonly ILogger<TweetController> _logger;
        public TweetController(ITweetService tweetService, ILogger<TweetController> logger)
        {
            _tweetService = tweetService;
            _logger = logger;
        }

        [HttpPost("add")]
        public async Task AddTweet(TweetModel tweetModel)
        {
            _logger.LogInformation("Started: Add New Tweet "+ tweetModel.ToString());
            await _tweetService.AddTweet(tweetModel);
            _logger.LogInformation("Ended: Add New Tweet");
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<List<TweetModel>> GetAllTweets()
        {
            _logger.LogInformation("Started: Get All Tweets");
            var result = await _tweetService.GetAllTweets();
            _logger.LogInformation("Ended: All Tweets");
            return result;
        }

        [HttpGet("{id}")]
        public async Task<List<TweetModel>> GetTweetById(string id)
        {
            _logger.LogInformation("Started: Get Tweet By Id " + id);
            var result = await _tweetService.GetTweetById(id);
            _logger.LogInformation("Ended: Get Tweet By Id " + id);
            return result;
        }

        [HttpPatch("{userId}/like")]
        public async Task<ActionResult> LikeTweet([FromBody]string tweetId,[FromRoute] string userId)
        {
            _logger.LogInformation("Started: Like/Unlike Tweet. Id: " + tweetId);
            await _tweetService.LikeTweet(tweetId, userId);
            _logger.LogInformation("Ended: Like/Unlike Tweet. Id: " + tweetId);
            return Ok("Like/Unlike Tweet Action Done");
        }

        [HttpDelete("{userId}/delete/{tweetId}")]
        public async Task<ActionResult> DeleteTweet([FromRoute] string tweetId, [FromRoute] string userId)
        {
            _logger.LogInformation($"Started: Delete Tweet. Id: {tweetId}");
            var result = await _tweetService.DeleteTweet(tweetId, userId);
            _logger.LogInformation($"Ended: Delete Tweet. Id: {tweetId}");
            return Ok(result);
        }

        [HttpPatch("{tweetId}/reply")]
        public async Task ReplyTweet([FromRoute] string tweetId, [FromBody] TweetReply reply)
        {
            _logger.LogInformation($"Started: Reply to Tweet {tweetId}" );
            await _tweetService.ReplyTweet(tweetId, reply);
            _logger.LogInformation($"Ended: Reply to Tweet {tweetId}");
        }
    }
}
