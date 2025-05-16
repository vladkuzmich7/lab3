using SchoolRecordsSystem.Core;
using System.Text.Json;

namespace SchoolRecordsSystem.Storage
{
    public interface IPupilDataStore
    {
        void Add(Pupil pupil);
        void Update(Pupil pupil);
        Pupil GetById(int id);
        List<Pupil> GetAll();
    }

    public class JsonPupilStorage : IPupilDataStore
    {
        private readonly string _storagePath;
        private List<Pupil> _pupils;
        private int _nextId;

        public JsonPupilStorage(string storagePath)
        {
            _storagePath = storagePath;
            LoadData();
        }

        private void LoadData()
        {
            if (File.Exists(_storagePath))
            {
                var json = File.ReadAllText(_storagePath);
                _pupils = JsonSerializer.Deserialize<List<Pupil>>(json) ?? new List<Pupil>();
                _nextId = _pupils.Count > 0 ? _pupils.Max(p => p.Id) + 1 : 1;
            }
            else
            {
                _pupils = new List<Pupil>();
                _nextId = 1;
            }
        }

        private void SaveData()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_pupils, options);
            File.WriteAllText(_storagePath, json);
        }

        public void Add(Pupil pupil)
        {
            pupil.Id = _nextId++;
            _pupils.Add(pupil);
            SaveData();
        }

        public void Update(Pupil pupil)
        {
            var existing = _pupils.FirstOrDefault(p => p.Id == pupil.Id);
            if (existing != null)
            {
                existing.FullName = pupil.FullName;
                existing.Score = pupil.Score;
                SaveData();
            }
        }

        public Pupil GetById(int id) => _pupils.FirstOrDefault(p => p.Id == id);

        public List<Pupil> GetAll() => _pupils;
    }
}