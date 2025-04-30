using Less_Annoying_CRM_Assessment.Models;
using Less_Annoying_CRM_Assessment.Repositories;
using System.Collections.Concurrent;
using System.Net;

namespace Less_Annoying_CRM_Assessment.Logging
{
    public class MemoryRequestLogger : IRequestLogger
    {
        private readonly ConcurrentBag<ApiRequestLogViewModel> _logs = new();

        public void LogRequest(string endpoint, HttpStatusCode statusCode)
        {
            _logs.Add(new ApiRequestLogViewModel
            {
                Timestamp = DateTime.UtcNow,
                Endpoint = endpoint,
                StatusCode = (int)statusCode
            });
        }

        public IEnumerable<ApiRequestLogViewModel> GetLogs() => _logs.OrderByDescending(l => l.Timestamp);
    }
}

