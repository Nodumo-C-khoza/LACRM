namespace Less_Annoying_CRM_Assessment.Models
{
    public class ApiRequestLogViewModel
    {
        public DateTime Timestamp { get; set; }
        public string Endpoint { get; set; }
        public int StatusCode { get; set; }
    }

    public class LacrmUser
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Timezone { get; set; }
        public string Email { get; set; }
    }

}

