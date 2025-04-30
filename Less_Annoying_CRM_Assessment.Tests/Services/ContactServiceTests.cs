using Less_Annoying_CRM_Assessment.Entities;
using Less_Annoying_CRM_Assessment.Interfaces;
using Less_Annoying_CRM_Assessment.Services;
using Moq;
using Xunit;

namespace Less_Annoying_CRM_Assessment.Tests.Services
{
    public class ContactServiceTests
    {
        private readonly Mock<IContactRepository> _mockRepo;
        private readonly ContactService _service;
        private readonly TelephonyEvent _testCallEvent;

        public ContactServiceTests()
        {
            _mockRepo = new Mock<IContactRepository>();
            _service = new ContactService(_mockRepo.Object);

            _testCallEvent = new TelephonyEvent
            {
                CallersTelephoneNumber = "0670896722",
                CallersName = "Nodumo",
                CallStart = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task HandleCallAsync_WithNullEvent_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.HandleCallAsync(null));
        }

        [Fact]
        public async Task HandleCallAsync_WithExistingContact_AddsNoteAndReturnsContact()
        {
            // Arrange
            var existingContact = new LACRMContact { ContactId = "4025830486878918972098489188472" };
            _mockRepo.Setup(r => r.GetContactByPhoneNumberAsync(_testCallEvent.CallersTelephoneNumber))
                    .ReturnsAsync(existingContact);

            // Act
            var result = await _service.HandleCallAsync(_testCallEvent);

            // Assert
            _mockRepo.Verify(r => r.CreateContactAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockRepo.Verify(r => r.AddNoteToContactAsync(existingContact.ContactId, It.IsAny<string>()), Times.Once);
            Assert.Equal(existingContact, result);
        }

        [Fact]
        public async Task HandleCallAsync_WithNewContact_CreatesContactAndAddsNote()
        {
            // Arrange
            var newContact = new LACRMContact { ContactId = "new-contact-id" };
            _mockRepo.Setup(r => r.GetContactByPhoneNumberAsync(_testCallEvent.CallersTelephoneNumber))
                    .ReturnsAsync((LACRMContact)null);
            _mockRepo.Setup(r => r.CreateContactAsync(_testCallEvent.CallersName, _testCallEvent.CallersTelephoneNumber))
                    .ReturnsAsync(newContact);

            // Act
            var result = await _service.HandleCallAsync(_testCallEvent);

            // Assert
            _mockRepo.Verify(r => r.CreateContactAsync(_testCallEvent.CallersName, _testCallEvent.CallersTelephoneNumber), Times.Once);
            _mockRepo.Verify(r => r.AddNoteToContactAsync(newContact.ContactId, It.IsAny<string>()), Times.Once);
            Assert.Equal(newContact, result);
        }

        [Fact]
        public async Task HandleCallAsync_CreatesExpectedNoteContent()
        {
            // Arrange
            var testTime = new DateTime(2025, 04, 30, 14, 30, 0);
            var callEvent = new TelephonyEvent
            {
                CallersTelephoneNumber = "0670896722",
                CallersName = "Nodumo",
                CallStart = testTime
            };

            var newContact = new LACRMContact
            {
                ContactId = "4025830486878918972098489188472" 
            };

            _mockRepo.Setup(r => r.GetContactByPhoneNumberAsync(It.IsAny<string>()))
                     .ReturnsAsync((LACRMContact)null);
            _mockRepo.Setup(r => r.CreateContactAsync(It.IsAny<string>(), It.IsAny<string>()))
                     .ReturnsAsync(newContact); // Return valid contact

            string actualNote = null;
            _mockRepo.Setup(r => r.AddNoteToContactAsync(It.IsAny<string>(), It.IsAny<string>()))
                     .Callback<string, string>((id, note) => actualNote = note);

            // Act
            await _service.HandleCallAsync(callEvent);

            // Assert
            var expectedNote = $"Call started at {testTime:yyyy-MM-dd HH:mm:ss} from Nodumo (0670896722)";
            Assert.Equal(expectedNote, actualNote);
        }
    }
}
