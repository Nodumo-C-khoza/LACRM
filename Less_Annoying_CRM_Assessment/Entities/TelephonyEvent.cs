using System.ComponentModel.DataAnnotations;

namespace Less_Annoying_CRM_Assessment.Entities
{
    public class TelephonyEvent
    {
        public string EventName { get; set; }

        public DateTime CallStart { get; set; }

        public string CallId { get; set; }

        public string CallersName { get; set; }

        public string CallersTelephoneNumber { get; set; }
    }

}

