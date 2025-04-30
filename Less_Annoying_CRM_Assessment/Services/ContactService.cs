using Less_Annoying_CRM_Assessment.Entities;
using Less_Annoying_CRM_Assessment.Interfaces;

namespace Less_Annoying_CRM_Assessment.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;

        public ContactService(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<LACRMContact> HandleCallAsync(TelephonyEvent callEvent)
        {
            if (callEvent == null)
                throw new ArgumentNullException(nameof(callEvent));

            try
            {
                var contact = await _contactRepository.GetContactByPhoneNumberAsync(callEvent.CallersTelephoneNumber)
                             ?? await _contactRepository.CreateContactAsync(
                                    callEvent.CallersName ?? "Unknown Caller",
                                    callEvent.CallersTelephoneNumber);

                var note = $"Call started at {callEvent.CallStart:yyyy-MM-dd HH:mm:ss} from {callEvent.CallersName} ({callEvent.CallersTelephoneNumber})";

                await _contactRepository.AddNoteToContactAsync(contact.ContactId, note);

                return contact;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

