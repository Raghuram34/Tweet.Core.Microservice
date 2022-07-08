using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tweet.Core.Kafka
{
    public class KafkaConsumer: IHostedService
    {
        private readonly string topicName = "fse-tweet";
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                var config = new ConsumerConfig()
                {
                    GroupId = "tweet_consumer_group",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    BootstrapServers = "localhost:9092"
                };

                using (var builder = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    builder.Subscribe(topicName);
                    var cancelToken = new CancellationTokenSource();
                    try
                    {
                        while (true)
                        {
                            var consumer = builder.Consume(cancelToken.Token);
                            Console.WriteLine($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");
                        }
                    }
                    catch (Exception)
                    {
                        builder.Close();
                    }
                }
            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
