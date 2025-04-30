using Less_Annoying_CRM_Assessment.Entities;
using Less_Annoying_CRM_Assessment.Models;

namespace Less_Annoying_CRM_Assessment.Interfaces
{
    public interface IContactRepository
    {
        Task<LACRMContact> GetContactByPhoneNumberAsync(string phoneNumber);
        Task<LACRMContact> CreateContactAsync(string fullName, string phoneNumber);
        Task AddNoteToContactAsync(string contactId, string noteContent);
        Task<List<LacrmUser>> GetUsersAsync();
    }
}

