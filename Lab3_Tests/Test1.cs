using System.Net;
using SchoolRecordsSystem.Core;
using SchoolRecordsSystem.Storage;
using SchoolRecordsSystem.Logic;
using System.Text.Json;

namespace SchoolRecordsSystem.Tests
{
    [TestClass]
    public class AcademicRecordManagerTests
    {
        private AcademicRecordManager _recordManager;
        private JsonPupilStorage _storage;
        private Quote _quoteService;
        private PupilCreator _creator;
        private readonly string _testFilePath = "test_pupils.json";

        [TestInitialize]
        public void Setup()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }

            _storage = new JsonPupilStorage(_testFilePath);
            _quoteService = new Quote(new HttpClient());
            _creator = new PupilCreator();
            _recordManager = new AcademicRecordManager(_storage, _quoteService, _creator);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [TestMethod]
        public async Task RegisterPupil_ValidData_AddsPupilAndReturnsQuote()
        {
            // Arrange
            var pupilData = new PupilData { FullName = "Иванов Иван", Score = 85 };

            // Act
            var quote = _recordManager.RegisterPupil(pupilData);
            var pupils = _storage.GetAll();

            // Assert
            Assert.AreEqual(1, pupils.Count, "Должен быть добавлен один ученик");
            Assert.AreEqual("Иванов Иван", pupils[0].FullName, "ФИО ученика должно совпадать");
            Assert.AreEqual(85, pupils[0].Score, "Оценка ученика должна совпадать");
            Assert.IsFalse(string.IsNullOrEmpty(quote.Text), "Текст цитаты не должен быть пустым");
            Assert.IsFalse(string.IsNullOrEmpty(quote.Author), "Автор цитаты не должен быть пустым");
        }

        [TestMethod]
        public async Task GetRandomQuote_SuccessfulResponse_ReturnsQuote()
        {
            // Act
            var quote = _quoteService.GetRandomQuote();

            // Assert
            Assert.IsNotNull(quote, "Цитата не должна быть null");
            Assert.IsFalse(string.IsNullOrEmpty(quote.Text), "Текст цитаты не должен быть пустым");
            Assert.IsFalse(string.IsNullOrEmpty(quote.Author), "Автор цитаты не должен быть пустым");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterPupil_InvalidScore_ThrowsArgumentException()
        {
            var pupilData = new PupilData { FullName = "Иванов Иван", Score = -1 };
            _recordManager.RegisterPupil(pupilData);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterPupil_EmptyName_ThrowsArgumentException()
        {
            var pupilData = new PupilData { FullName = "", Score = 85 };
            _recordManager.RegisterPupil(pupilData);
        }

        [TestMethod]
        public void ModifyPupilRecord_ValidData_UpdatesPupil()
        {
            // Arrange
            var initialData = new PupilData { FullName = "Петров Петр", Score = 75 };
            _recordManager.RegisterPupil(initialData);
            var pupilId = _storage.GetAll()[0].Id;
            var updatedData = new PupilData { FullName = "Петров Петр Иванович", Score = 90 };

            // Act
            _recordManager.ModifyPupilRecord(pupilId, updatedData);
            var updatedPupil = _storage.GetById(pupilId);

            // Assert
            Assert.AreEqual("Петров Петр Иванович", updatedPupil.FullName);
            Assert.AreEqual(90, updatedPupil.Score);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ModifyPupilRecord_InvalidId_ThrowsException()
        {
            var pupilData = new PupilData { FullName = "Несуществующий", Score = 50 };
            _recordManager.ModifyPupilRecord(999, pupilData);
        }

        [TestMethod]
        public void GetAllRecords_EmptyStorage_ReturnsEmptyList()
        {
            // Act
            var records = _recordManager.GetAllRecords();

            // Assert
            Assert.AreEqual(0, records.Count);
        }
    }

    [TestClass]
    public class JsonPupilStorageTests
    {
        private readonly string _testFilePath = "test_storage.json";

        [TestInitialize]
        public void Setup()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [TestMethod]
        public void AddPupil_NewPupil_SavesToFile()
        {
            // Arrange
            var storage = new JsonPupilStorage(_testFilePath);
            var pupil = new Pupil { FullName = "Сидоров Сидор", Score = 80 };

            // Act
            storage.Add(pupil);
            var savedData = File.ReadAllText(_testFilePath);
            var pupils = JsonSerializer.Deserialize<List<Pupil>>(savedData);

            // Assert
            Assert.AreEqual(1, pupils.Count);
            Assert.AreEqual("Сидоров Сидор", pupils[0].FullName);
        }

        [TestMethod]
        public void UpdatePupil_ExistingPupil_UpdatesData()
        {
            // Arrange
            var storage = new JsonPupilStorage(_testFilePath);
            var pupil = new Pupil { Id = 1, FullName = "Original", Score = 50 };
            storage.Add(pupil);

            var updatedPupil = new Pupil { Id = 1, FullName = "Updated", Score = 75 };

            // Act
            storage.Update(updatedPupil);
            var retrieved = storage.GetById(1);

            // Assert
            Assert.AreEqual("Updated", retrieved.FullName);
            Assert.AreEqual(75, retrieved.Score);
        }
    }
}