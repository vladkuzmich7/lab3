using SchoolRecordsSystem.Interface;
using SchoolRecordsSystem.Logic;
using SchoolRecordsSystem.Storage;

var httpClient = new HttpClient();
var storage = new JsonPupilStorage("pupils_data.json");
var quoteService = new Quote(httpClient);
var creator = new PupilCreator();

var manager = new AcademicRecordManager(storage, quoteService, creator);
var commandHandler = new CommandHandler(manager);
var ui = new UserInterface(commandHandler);

try
{
    ui.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Критическая ошибка: {ex.Message}");
}