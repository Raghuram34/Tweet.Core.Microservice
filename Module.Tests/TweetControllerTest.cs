using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Tweet.Core.Controllers;
using Tweet.Core.Exceptions;
using Tweet.Core.Kafka;
using Tweet.Core.Models;
using Tweet.Core.Services.Abstractions;
using Xunit;

namespace Module.Tests
{
    public class TweetControllerTest
    {
        private readonly Mock<ITweetService> mockTweetService;
        private readonly Mock<KafkaProducer> mockKafkaProducer;

        private readonly Dictionary<string, string> inMemorySettings = new Dictionary<string, string>() {
            { "producer:bootstrapservers", "localhost:9092" }
        };

        public TweetControllerTest()
        {
            var producerConfig = new ProducerConfig();
            IConfiguration configuration = new ConfigurationBuilder()
                                            .AddInMemoryCollection(inMemorySettings)
                                            .Build();
            configuration.Bind("producer", producerConfig);

            mockTweetService = new Mock<ITweetService>();
            mockKafkaProducer = new Mock<KafkaProducer>(producerConfig);
        }

        [Fact]
        public async Task GetAllTweetModels_SimpleTestAsync()
        {
            // ARRANGE
            TweetController tweetController = new TweetController(mockTweetService.Object, new Mock<ILogger<TweetController>>().Object, mockKafkaProducer.Object);

            // SETUP
            mockTweetService
                .Setup(service => service.GetAllTweets())
                .ReturnsAsync(new List<TweetModel>());

            // ACT
            var result = await tweetController.GetAllTweets();

            Assert.IsType<List<TweetModel>>(result);
        }

        [Fact]
        public async Task GetAllTweetModels_ErrorTest()
        {
            // ARRANGE
            TweetController tweetController = new TweetController(mockTweetService.Object, new Mock<ILogger<TweetController>>().Object, mockKafkaProducer.Object);

            // SETUP
            mockTweetService
                .Setup(service => service.GetAllTweets())
                .Throws(new CustomException());

            // ACT
            await Assert.ThrowsAsync<CustomException>(() => tweetController.GetAllTweets());

        }

        [Fact]
        public async Task CreateTweetModel_ErrorTestAsync()
        {
            // ARRANGE
            TweetController tweetController = new TweetController(mockTweetService.Object, new Mock<ILogger<TweetController>>().Object, mockKafkaProducer.Object);

            // SETUP
            mockTweetService
                .Setup(service => service.AddTweet(It.IsAny<TweetModel>()))
                .Throws(new CustomException());

            // ACT
            await Assert.ThrowsAsync<CustomException>(() => tweetController.AddTweet(new Mock<TweetModel>().Object));
        }

        [Fact]
        public async Task CreateTweetModel_SimpleTestAsync()
        {
            // ARRANGE
            TweetController tweetController = new TweetController(mockTweetService.Object, new Mock<ILogger<TweetController>>().Object, mockKafkaProducer.Object);

            var tweet = new TweetModel();
            tweet.User = new Profile() { 
                Email = "same"
            };
            // SETUP
            mockTweetService
                .Setup(service => service.AddTweet(It.IsAny<TweetModel>()));

            // ACT
            var result = await tweetController.AddTweet(tweet) as ObjectResult;

            Assert.Equal(201, result.StatusCode);
        }

        [Fact]
        public async Task ReplyTweetModel_SimpleTestAsync()
        {
            // ARRANGE
            TweetController tweetController = new TweetController(mockTweetService.Object, new Mock<ILogger<TweetController>>().Object, mockKafkaProducer.Object);

            // SETUP
            mockTweetService
                .Setup(service => service.ReplyTweet(It.IsAny<string>(), It.IsAny<TweetReply>()))
                .Returns(Task.CompletedTask);

            var result = await tweetController.ReplyTweet(It.IsAny<string>(), It.IsAny<TweetReply>()) as ObjectResult;

            // ACT
            Assert.Equal(200, result.StatusCode);

        }

        [Fact]
        public async Task ReplyTweetModel_ErrorTestAsync()
        {
            // ARRANGE
            TweetController tweetController = new TweetController(mockTweetService.Object, new Mock<ILogger<TweetController>>().Object, mockKafkaProducer.Object);

            // SETUP
            mockTweetService
                .Setup(service => service.ReplyTweet(It.IsAny<string>(), It.IsAny<TweetReply>()))
                .Throws(new CustomException());

            // ACT
            await Assert.ThrowsAsync<CustomException>(() => tweetController.ReplyTweet(It.IsAny<string>(), It.IsAny<TweetReply>()));

        }

        [Fact]
        public async Task LikeTweetModel_SimpleTestAsync()
        {
            // ARRANGE
            TweetController tweetController = new TweetController(mockTweetService.Object, new Mock<ILogger<TweetController>>().Object, mockKafkaProducer.Object);

            // SETUP
            mockTweetService
                .Setup(service => service.LikeTweet(It.IsAny<string>(), It.IsAny<string>()));

            // ACT
            var result = await tweetController.LikeTweet(It.IsAny<string>(), It.IsAny<string>()) as ObjectResult;

            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task LikeTweetModel_ErrorTestAsync()
        {
            // ARRANGE
            TweetController tweetController = new TweetController(mockTweetService.Object, new Mock<ILogger<TweetController>>().Object, mockKafkaProducer.Object);

            // SETUP
            mockTweetService
                .Setup(service => service.LikeTweet(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new CustomException());

            // ACT
            await Assert.ThrowsAsync<CustomException>(() => tweetController.LikeTweet(It.IsAny<string>(), It.IsAny<string>()));

        }

        [Fact]
        public async Task DeleteTweetModel_SimpleTestAsync()
        {
            // ARRANGE
            TweetController tweetController = new TweetController(mockTweetService.Object, new Mock<ILogger<TweetController>>().Object, mockKafkaProducer.Object);

            // SETUP
            mockTweetService
                .Setup(service => service.DeleteTweet(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // ACT
            var result = await tweetController.DeleteTweet(It.IsAny<string>(), It.IsAny<string>()) as ObjectResult;

            Assert.Equal(200, result.StatusCode);
            Assert.Equal(true, result.Value);
        }

        [Fact]
        public async Task DeleteTweetModel_ErrorTestAsync()
        {
            // ARRANGE
            TweetController tweetController = new TweetController(mockTweetService.Object, new Mock<ILogger<TweetController>>().Object, mockKafkaProducer.Object);

            // SETUP
            mockTweetService
                .Setup(service => service.DeleteTweet(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new CustomException());

            // ACT
            await Assert.ThrowsAsync<CustomException>(() => tweetController.DeleteTweet(It.IsAny<string>(), It.IsAny<string>()));

        }
    }
}
