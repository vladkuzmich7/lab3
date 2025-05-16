using SchoolRecordsSystem.Core;
using SchoolRecordsSystem.Storage;
using System.Text.Json;

namespace SchoolRecordsSystem.Logic
{
    public interface IInspirationProvider
    {
        InspirationalQuote GetRandomQuote();
    }

    public class PupilCreator
    {
        public Pupil Create(string fullName, int score) => new Pupil
        {
            FullName = fullName,
            Score = score
        };
    }

    public class AcademicRecordManager
    {
        private readonly IPupilDataStore _dataStore;
        private readonly IInspirationProvider _inspirationProvider;
        private readonly PupilCreator _creator;

        public AcademicRecordManager(
            IPupilDataStore dataStore,
            IInspirationProvider inspirationProvider,
            PupilCreator creator)
        {
            _dataStore = dataStore;
            _inspirationProvider = inspirationProvider;
            _creator = creator;
        }

        public InspirationalQuote RegisterPupil(PupilData data)
        {
            ValidatePupilData(data);
            var pupil = _creator.Create(data.FullName, data.Score);
            _dataStore.Add(pupil);
            return _inspirationProvider.GetRandomQuote();
        }

        public void ModifyPupilRecord(int id, PupilData data)
        {
            ValidatePupilData(data);
            var pupil = _dataStore.GetById(id) ?? throw new Exception("Ученик не найден");
            pupil.FullName = data.FullName;
            pupil.Score = data.Score;
            _dataStore.Update(pupil);
        }

        public List<Pupil> GetAllRecords() => _dataStore.GetAll();

        private void ValidatePupilData(PupilData data)
        {
            if (string.IsNullOrWhiteSpace(data.FullName))
                throw new ArgumentException("Имя не может быть пустым");

            if (data.Score < 0 || data.Score > 100)
                throw new ArgumentException("Оценка должна быть от 0 до 100");
        }
    }
}