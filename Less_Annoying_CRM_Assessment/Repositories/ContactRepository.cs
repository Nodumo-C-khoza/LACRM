using Less_Annoying_CRM_Assessment.Entities;
using Less_Annoying_CRM_Assessment.Interfaces;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using Less_Annoying_CRM_Assessment.ExtensionMethods;
using Less_Annoying_CRM_Assessment.Models;

namespace Less_Annoying_CRM_Assessment.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IRequestLogger _requestLogger;

        public ContactRepository(HttpClient httpClient, IConfiguration config, IRequestLogger requestLogger, IOptions<LacrmSettings> lacrmOptions)
        {
            _httpClient = httpClient;
            _apiKey = lacrmOptions.Value.ApiKey ?? throw new ArgumentNullException(nameof(lacrmOptions.Value.ApiKey));
            _requestLogger = requestLogger;
            _httpClient.BaseAddress = new Uri("https://api.lessannoyingcrm.com/v2/");
        }
        public async Task<LACRMContact> GetContactByPhoneNumberAsync(string phoneNumber)
        {
            try
            {
                var request = new
                {
                    Function = "GetContacts",
                    Parameters = new
                    {
                        SearchTerms = phoneNumber,
                    }
                };

                var response = await SendRequestAsync(request, "GetContacts");

                if (response["Results"] == null || !response["Results"].Any())
                {
                    return null; 
                }

                var firstContact = response["Results"].FirstOrDefault();
                if (firstContact == null)
                {
                    return null;
                }

                return new LACRMContact
                {
                    ContactId = firstContact["ContactId"]?.ToString(),
                    Name = $"{firstContact["Name"]?["FirstName"]} {firstContact["Name"]?["LastName"]}".Trim(),
                    Phone = firstContact["Phone"]?[0]?["Text"]?.ToString(),
                    AssignedTo = firstContact["AssignedTo"]?.ToString(),
                    IsCompany = firstContact["IsCompany"]?.ToObject<bool>() ?? false
                };
            }
            catch (Exception ex)
            {
                return null; 
            }

        }

        public async Task<LACRMContact> CreateContactAsync(string fullName, string phoneNumber)
        {
            var users = await GetUsersAsync();
            if (users == null || users.Count == 0)
                throw new Exception("No users found in LACRM to assign the contact to.");

            var createRequest = new
            {
                Function = "CreateContact",
                Parameters = new
                {
                    Name = fullName,
                    Phone = new[] { new { Text = phoneNumber, Type = "Work" } }, 
                    AssignedTo = users[0].UserId,
                    IsCompany = false,
                    BackgroundInfo = "Created from phone call"
                }
            };

            var createResponse = await SendRequestAsync(createRequest, "CreateContact");
            var contactId = createResponse["ContactId"]?.ToString();

            if (string.IsNullOrEmpty(contactId))
                throw new Exception("Contact creation failed - no ContactId returned");

            return await GetFullContactDetailsAsync(contactId);
        }

        public async Task<List<LacrmUser>> GetUsersAsync()
        {
            var request = new
            {
                Function = "GetUsers",
                Parameters = new { }
            };

            var responseJson = await SendRequestAsync(request, "GetUsers");

            return responseJson.ToObject<List<LacrmUser>>();
        }
        private async Task<LACRMContact> GetFullContactDetailsAsync(string contactId)
        {
            var request = new
            {
                Function = "GetContact",
                Parameters = new { ContactId = contactId }
            };

            var response = await SendRequestAsync(request, "GetContact");

            return new LACRMContact
            {
                ContactId = contactId,
                Name = response["Name"]?.ToString() ?? string.Empty,
                Phone = ExtractPrimaryPhone(response["Phone"] as JArray),
                AssignedTo = response["AssignedTo"]?.ToString() ?? string.Empty,
                IsCompany = response["IsCompany"]?.Value<bool>() ?? false
            };
        }

        public async Task AddNoteToContactAsync(string contactId, string noteContent)
        {
            var request = new
            {
                Function = "CreateNote",
                Parameters = new
                {
                    ContactId = contactId,
                    Note = noteContent
                }
            };

            await SendRequestAsync(request, "CreateNote");
        }

        private async Task<JToken> SendRequestAsync(object requestBody, string endpoint)
        {
            var requestJson = JsonConvert.SerializeObject(requestBody);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "")
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };
            requestMessage.Headers.Add("Authorization", _apiKey);

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            _requestLogger.LogRequest(endpoint, response.StatusCode);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"LACRM API error: {content}");


            return JToken.Parse(content);
        }
        private string ExtractPrimaryPhone(JArray phones)
        {
            return phones?.FirstOrDefault()?["Text"]?.ToString() ?? string.Empty;
        }
    }
}

