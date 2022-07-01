using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet.Core.Models;

namespace Tweet.Core.Services.Abstractions
{
    public interface ITweetService
    {
        Task AddTweet(TweetModel tweet);

        Task<List<TweetModel>> GetAllTweets();

        Task<List<TweetModel>> GetTweetById(string id);

        Task LikeTweet(string tweetId, string userId);

        Task ReplyTweet(string tweetId, TweetReply reply);

        Task<Boolean> DeleteTweet(string tweetId, string userId);
    }
}
