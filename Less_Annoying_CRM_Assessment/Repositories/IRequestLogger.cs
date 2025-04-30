using Less_Annoying_CRM_Assessment.Models;
using System.Net;

namespace Less_Annoying_CRM_Assessment.Repositories
{
    public interface IRequestLogger
    {
        void LogRequest(string endpoint, HttpStatusCode statusCode);
        IEnumerable<ApiRequestLogViewModel> GetLogs();
    }
}

