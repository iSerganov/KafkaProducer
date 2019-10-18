using Confluent.Kafka;
using KafkaProducer.Dtos;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaProducer.Services
{
    public class EventProcessService: BackgroundService
    {
        private readonly ConsumerConfig consumerConfig;
        private readonly ProducerConfig producerConfig;
        private readonly ConsumerWrapper consumerHelper;
        public EventProcessService(ConsumerConfig consumerConfig, ProducerConfig producerConfig, ConsumerWrapper cw)
        {
            this.producerConfig = producerConfig;
            this.consumerConfig = consumerConfig;
            this.consumerHelper = cw;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Event processing Service Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                string eventRequest = consumerHelper.ReadMessage("eventsqueue");

                //Deserilaize 
                EventDto eventToProcess = JsonConvert.DeserializeObject<EventDto>(eventRequest);

                //TODO:: Process Event
                Console.WriteLine($"Info: EventHandler => Processing the event for {eventToProcess.Name}");
                eventToProcess.Proprity = 3;

                //Write to ReadyToPublish Queue

                var producerWrapper = new ProducerWrapper(producerConfig, "publishedevents");
                await producerWrapper.WriteMessage(JsonConvert.SerializeObject(eventToProcess));
            }
        }

        public async Task Process (CancellationToken stoppingToken)
        {
            await ExecuteAsync(stoppingToken);
        }
    }
}
