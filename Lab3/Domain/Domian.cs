namespace SchoolRecordsSystem.Core
{
    public class Pupil
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Score { get; set; }
    }

    public class PupilData
    {
        public string FullName { get; init; }
        public int Score { get; init; }
    }

    public class InspirationalQuote
    {
        public string Text { get; set; }
        public string Author { get; set; }
    }
}