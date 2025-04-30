using Microsoft.AspNetCore.Mvc;
using Less_Annoying_CRM_Assessment.Entities;
using Less_Annoying_CRM_Assessment.ExtensionMethods;
using Less_Annoying_CRM_Assessment.Interfaces;
using Less_Annoying_CRM_Assessment.Models;
using Less_Annoying_CRM_Assessment.Repositories;
using System.Net;

namespace Less_Annoying_CRM_Assessment.Controllers
{
    [ApiController]
    [Route("api")]
    public class CallsController : Controller
    {
        private readonly IContactService _contactService;
        private readonly IRequestLogger _requestLogger;

        public CallsController(IContactService contactService, IRequestLogger requestLogger)
        {
            _contactService = contactService;
            _requestLogger = requestLogger;
        }

        [HttpPost("calls")]
        public async Task<IActionResult> PostCall([FromBody] TelephonyEvent callEvent)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var contact = await _contactService.HandleCallAsync(callEvent);

                return Ok(new
                {
                    contactId = contact.ContactId,
                    name = contact.Name,
                    assignedTo = contact.AssignedTo,
                    phone = contact.Phone,
                    isCompany = contact.IsCompany
                });
            }
            catch (CrmIntegrationException ex)
            {
                return StatusCode(502, new { error = ex.Message });
            }
        }

        [HttpGet("logs")]
        public IActionResult GetLogs()
        {
            var logs = _requestLogger.GetLogs();
            return Ok(logs);
        }


    }
}

