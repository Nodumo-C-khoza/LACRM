using Less_Annoying_CRM_Assessment.Entities;

namespace Less_Annoying_CRM_Assessment.Interfaces
{
    public interface IContactService
    {
        Task<LACRMContact> HandleCallAsync(TelephonyEvent callEvent);

    }
}

