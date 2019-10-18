using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaProducer.Dtos;
using KafkaProducer.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KafkaProducer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly ProducerConfig pconfig;
        private readonly ConsumerConfig cconfig;
        private readonly ConsumerWrapper consumer;
        public EventController(ProducerConfig pconfig, ConsumerConfig cconfig, ConsumerWrapper cw)
        {
            this.pconfig = pconfig;
            this.cconfig = cconfig;
            this.consumer = cw;
        }
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody]EventDto value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                //Serialize 
                string serializedEvent = JsonConvert.SerializeObject(value);

                var producer = new ProducerWrapper(this.pconfig, "eventsqueue");
                await producer.WriteMessage(serializedEvent);

                return Created("TransactionId", "Your event is in progress");
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }
        [HttpGet]
        [Route("api/[controller]/GetFromEventQueue")]
        public ActionResult<EventDto> GetFromEventQueue()
        {
            try
            {
                var msg = consumer.ReadMessage("eventsqueue");
                EventDto res = JsonConvert.DeserializeObject<EventDto>(msg);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet]
        [Route("api/[controller]/GetFromPublishedQueue")]
        public ActionResult<EventDto> GetFromPublishedQueue()
        {
            try
            {
                var msg = consumer.ReadMessage("publishedevents");
                EventDto res = JsonConvert.DeserializeObject<EventDto>(msg);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet]
        [Route("api/[controller]/Process")]
        public async Task<ActionResult> Process()
        {
            var service = new EventProcessService(this.cconfig, this.pconfig, consumer);
            await service.Process(System.Threading.CancellationToken.None);

            return Ok("Message in eventsqueue has been processed and transferred to published events");
        }
    }
}
