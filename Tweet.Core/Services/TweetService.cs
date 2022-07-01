using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet.Core.Exceptions;
using Tweet.Core.Models;
using Tweet.Core.Services.Abstractions;

namespace Tweet.Core.Services
{
    public class TweetService : ITweetService
    {
        private readonly IMongoCollection<TweetModel> _tweetsCollection;
        private readonly IConfiguration _configuration;

        public TweetService(IConfiguration configuration, IOptions<MongoDBSettings> mongoDBSettings)
        {
            _configuration = configuration;
            var mongoClient = new MongoClient(mongoDBSettings.Value.ConnectionURI);

            var mongoDatabase = mongoClient.GetDatabase(
                mongoDBSettings.Value.DatabaseName);

            _tweetsCollection = mongoDatabase.GetCollection<TweetModel>(
                mongoDBSettings.Value.CollectionName);
        }

        public async Task AddTweet(TweetModel tweet)
        {
            tweet.Date = DateTime.Now;
            await _tweetsCollection.InsertOneAsync(tweet);
        }

        public async Task<bool> DeleteTweet(string id, string userId)
        {
            TweetModel tweetModel = _tweetsCollection.Find(tweet => tweet.Id == id && tweet.User.Id == userId).FirstOrDefault();

            // tweetModel is Null then throw error
            if(tweetModel == null)
            {
                throw new CustomException("Either Tweet Not Existed or User Not authorized to delete this tweet");
            }

            await _tweetsCollection.DeleteOneAsync(tweet => tweet.Id == id);
            return true;
        }

        public async Task<List<TweetModel>> GetAllTweets()
        {
            return await _tweetsCollection.Find(tweet => true).SortByDescending(tweet => tweet.Date).ToListAsync();
        }

        public async Task<List<TweetModel>> GetTweetById(string id)
        {
            var tweets = await _tweetsCollection.Find(tweet => tweet.User.Id == id).SortByDescending(tweet => tweet.Date).ToListAsync();
            return tweets;
        }

        public async Task LikeTweet(string tweetId, string userId)
        {
            TweetModel updatedTweet = await _tweetsCollection.Find(tweet => tweet.Id == tweetId).FirstOrDefaultAsync();
            if(updatedTweet == null)
            {
                throw new CustomException("Tweet Not Found");
            }
            else if(updatedTweet.Likes == null) {
                updatedTweet.Likes = new List<string>() { userId };
            }
            else if(updatedTweet.Likes.Contains(userId))
            {
                updatedTweet.Likes.Remove(userId);
            }
            else
            {
                updatedTweet.Likes.Add(userId);
            }
            await _tweetsCollection.ReplaceOneAsync(tweet => tweet.Id == tweetId, updatedTweet);
        }

        public async Task ReplyTweet(string tweetId, TweetReply reply)
        {
            reply.Date = DateTime.Now;
            TweetModel updatedTweet = await _tweetsCollection.Find(tweet => tweet.Id == tweetId).FirstOrDefaultAsync();
            if (updatedTweet == null)
            {
                throw new CustomException("Tweet Not Found");
            }
            else if (updatedTweet.Replies == null)
            {
                updatedTweet.Replies = new List<TweetReply>() { reply };
            }
            else
            {
                updatedTweet.Replies.Add(reply);
            }
            await _tweetsCollection.ReplaceOneAsync(tweet => tweet.Id == tweetId, updatedTweet);
        }
    }
}
