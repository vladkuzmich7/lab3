using SchoolRecordsSystem.Logic;
using SchoolRecordsSystem.Core;

namespace SchoolRecordsSystem.Interface
{
    public class CommandHandler
    {
        private readonly AcademicRecordManager _recordManager;

        public CommandHandler(AcademicRecordManager recordManager)
        {
            _recordManager = recordManager;
        }

        public void ProcessCommand(string command)
        {
            switch (command)
            {
                case "1":
                    AddNewPupil();
                    break;
                case "2":
                    EditExistingPupil();
                    break;
                case "3":
                    DisplayAllPupils();
                    break;
                case "4":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Неверная команда. Попробуйте снова.");
                    break;
            }
        }

        private void AddNewPupil()
        {
            Console.Write("Введите ФИО ученика: ");
            var name = Console.ReadLine();

            Console.Write("Введите оценку (0-100): ");
            if (!int.TryParse(Console.ReadLine(), out int score))
            {
                Console.WriteLine("Некорректная оценка.");
                return;
            }

            try
            {
                var quote = _recordManager.RegisterPupil(new PupilData
                {
                    FullName = name,
                    Score = score
                });

                Console.WriteLine("Ученик успешно добавлен.");
                Console.WriteLine($"Цитата дня: \"{quote.Text}\" — {quote.Author}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void EditExistingPupil()
        {
            Console.Write("Введите ID ученика: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Некорректный ID.");
                return;
            }

            Console.Write("Введите новое ФИО: ");
            var name = Console.ReadLine();

            Console.Write("Введите новую оценку (0-100): ");
            if (!int.TryParse(Console.ReadLine(), out int score))
            {
                Console.WriteLine("Некорректная оценка.");
                return;
            }

            try
            {
                _recordManager.ModifyPupilRecord(id, new PupilData
                {
                    FullName = name,
                    Score = score
                });

                Console.WriteLine("Данные ученика успешно обновлены.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void DisplayAllPupils()
        {
            var pupils = _recordManager.GetAllRecords();

            if (pupils.Count == 0)
            {
                Console.WriteLine("Нет данных об учениках.");
                return;
            }

            Console.WriteLine("Список учеников:");
            foreach (var pupil in pupils)
            {
                Console.WriteLine($"ID: {pupil.Id}, ФИО: {pupil.FullName}, Оценка: {pupil.Score}");
            }
        }
    }

    public class UserInterface
    {
        private readonly CommandHandler _commandHandler;

        public UserInterface(CommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        public void Run()
        {
            while (true)
            {
                DisplayMenu();
                var choice = Console.ReadLine();
                _commandHandler.ProcessCommand(choice);
            }
        }

        private void DisplayMenu()
        {
            Console.WriteLine("\nСистема учета успеваемости");
            Console.WriteLine("1. Добавить ученика");
            Console.WriteLine("2. Редактировать данные ученика");
            Console.WriteLine("3. Просмотреть всех учеников");
            Console.WriteLine("4. Выход");
            Console.Write("Выберите действие: ");
        }
    }
}