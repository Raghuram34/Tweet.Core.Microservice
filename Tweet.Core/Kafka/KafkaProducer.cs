using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet.Core.Models;

namespace Tweet.Core.Kafka
{
    public class KafkaProducer
    {
        private string _topicName = "fse-tweet";
        private IProducer<Null, string> _producer;
        private ProducerConfig _config;

        public KafkaProducer(ProducerConfig config)
        {
            _config = config;
            _producer = new ProducerBuilder<Null, string>(_config).Build();
        }

        public async Task TweetAdded(TweetModel tweet)
        {
            var dr = await _producer.ProduceAsync(_topicName, new Message<Null, string>()
            {
                Value = tweet.User.Email 
            });
            Console.WriteLine($"Tweet => Added  '{dr.Value}' to '{dr.TopicPartitionOffset}'");
            return;
        }
    }
}
