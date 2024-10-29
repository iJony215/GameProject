using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static string usersFilePath = "users.txt"; 
    static string leaderboardFilePath = "leaderboard.txt"; 

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("Меню:");
            Console.WriteLine("1. Войти");
            Console.WriteLine("2. Зарегистрироваться");
            Console.WriteLine("3. Таблица результатов");
            Console.WriteLine("4. Выход");
            Console.Write("Выберите пункт: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    if (AuthorizeUser())
                    {
                        StartGame();
                    }
                    break;
                case "2":
                    RegisterUser();
                    break;
                case "3":
                    ShowLeaderboard();
                    break;
                case "4":
                    Console.WriteLine("Выход из игры.");
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    break;
            }
        }
    }

    
    static bool AuthorizeUser()
    {
        Console.Write("Введите логин: ");
        string login = Console.ReadLine();
        Console.Write("Введите пароль: ");
        string password = Console.ReadLine();

        if (!File.Exists(usersFilePath))
        {
            Console.WriteLine("Файл с пользователями не найден. Пожалуйста, зарегистрируйтесь.");
            return false;
        }

        var users = File.ReadAllLines(usersFilePath)
                        .Select(line => line.Split(','))
                        .ToDictionary(parts => parts[0], parts => parts[1]);

        if (users.ContainsKey(login) && users[login] == password)
        {
            Console.WriteLine("Авторизация успешна!");
            return true;
        }

        Console.WriteLine("Неверный логин или пароль.");
        return false;
    }

    
    static void RegisterUser()
    {
        Console.Write("Введите новый логин: ");
        string login = Console.ReadLine();
        Console.Write("Введите новый пароль: ");
        string password = Console.ReadLine();

        using (StreamWriter writer = new StreamWriter(usersFilePath, true))
        {
            writer.WriteLine($"{login},{password}");
        }

        Console.WriteLine("Регистрация успешна. Теперь вы можете войти.");
    }

 
    static void StartGame()
    {
        Console.Write("Введите ваше имя для таблицы лидеров: ");
        string playerName = Console.ReadLine();
        string secretNumber = GenerateUniqueNumber(); 
        int attempts = 0;

        Console.WriteLine("Игра началась! Попробуй угадать 4-значное число с уникальными цифрами.");

        while (true)
        {
            attempts++;
            Console.Write("Введите 4-значное число: ");
            string guess = Console.ReadLine();

            if (guess.Length != 4 || !int.TryParse(guess, out _))
            {
                Console.WriteLine("Нужно ввести ровно 4 цифры.");
                continue;
            }

            bool correctGuess = true;

            for (int i = 0; i < 4; i++)
            {
                if (guess[i] == secretNumber[i])
                {
                    Console.WriteLine($"Цифра {guess[i]} правильная и стоит на правильной позиции.");
                }
                else if (secretNumber.Contains(guess[i]))
                {
                    Console.WriteLine($"Цифра {guess[i]} правильная, но стоит на неправильной позиции.");
                    correctGuess = false;
                }
                else
                {
                    correctGuess = false;
                }
            }

            if (correctGuess)
            {
                Console.WriteLine($"Ты отгадал число {secretNumber} за {attempts} ходов.");
                SaveResult(playerName, attempts); 
                break;
            }
        }
    }


    static void SaveResult(string playerName, int attempts)
    {
        using (StreamWriter writer = new StreamWriter(leaderboardFilePath, true))
        {
            writer.WriteLine($"{playerName},{attempts}");
        }
    }

    
    static void ShowLeaderboard()
    {
        if (!File.Exists(leaderboardFilePath))
        {
            Console.WriteLine("Таблица лидеров пуста.");
            return;
        }

        var leaderboard = File.ReadAllLines(leaderboardFilePath)
            .Select(line => line.Split(','))
            .Select(parts => new { Name = parts[0], Attempts = int.Parse(parts[1]) })
            .OrderBy(result => result.Attempts)
            .ToList();

        if (leaderboard.Count == 0)
        {
            Console.WriteLine("Таблица лидеров пуста.");
        }
        else
        {
            Console.WriteLine("Таблица лидеров:");
            foreach (var entry in leaderboard)
            {
                Console.WriteLine($"{entry.Name}: {entry.Attempts} попыток");
            }
        }
    }


    static string GenerateUniqueNumber()
    {
        Random random = new Random();
        string digits = "0123456789";
        string secretNumber = "";

        while (secretNumber.Length < 4)
        {
            char digit = digits[random.Next(digits.Length)];
            if (!secretNumber.Contains(digit))
            {
                secretNumber += digit;
            }
        }

        return secretNumber;
    }
}
