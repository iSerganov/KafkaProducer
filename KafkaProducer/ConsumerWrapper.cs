using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaProducer
{
    public class ConsumerWrapper
    {
        private ConsumerConfig _consumerConfig;
        private Dictionary<string, IConsumer<string, string>> _consumers;
        private static readonly Random rand = new Random();
        public ConsumerWrapper(ConsumerConfig config, List<string> topicNames)
        {
            this._consumers = new Dictionary<string, IConsumer<string, string>>();
            this._consumerConfig = config;
            foreach (var topicName in topicNames)
            {
                var c = new ConsumerBuilder<string, string>(config).Build();
                c.Subscribe(topicName);
                this._consumers.Add(topicName, c);
            }
        }
        public string ReadMessage(string topicName)
        {
            var consumer = this._consumers[topicName];
            var consumeResult = consumer.Consume();
            var result = consumeResult.Value;
            consumer.Commit();
            return result;
        }
    }
}
