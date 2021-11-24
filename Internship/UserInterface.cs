using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Internship
{
    public class UserInterface
    {
        private List<Book> books = new();
        private List<Person> people = new();
        private string bookFile = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/" + "books.json";
        private string peopleFile = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/" + "people.json";
        private string booksJson;
        private string peopleJson;

        public void Run()
        {
            while (true)
            {
                if (!File.Exists(bookFile))
                {
                    File.Create(bookFile);
                }

                booksJson = File.ReadAllText(bookFile);

                if (booksJson != "")
                {
                    books = JsonSerializer.Deserialize<List<Book>>(booksJson);
                }

                if (!File.Exists(peopleFile))
                {
                    File.Create(peopleFile);
                }

                peopleJson = File.ReadAllText(peopleFile);

                if (peopleJson != "")
                {
                    people = JsonSerializer.Deserialize<List<Person>>(peopleJson);
                }

                string command;

                Console.WriteLine("Hello, please choose one of the following commands:");
                Console.WriteLine("1: Add a new book");
                Console.WriteLine("2: Take a book from the library");
                Console.WriteLine("3: Return a book");
                Console.WriteLine("4: List the books");
                Console.WriteLine("5: Delete a book");
                Console.WriteLine("6: Exit program");
                Console.Write("Your command: ");

                command = Console.ReadLine();
                Console.WriteLine();

                switch(command)
                {
                    case "1":
                        AddBook();
                        break;

                    case "2":
                        TakeBook();
                        break;

                    case "3":
                        ReturnBook();
                        break;

                    case "4":
                        ListBooks();
                        break;

                    case "5":
                        DeleteBook();
                        break;

                    case "6":
                        return;

                    default:
                        Console.WriteLine("Invalid command.");
                        break;
                }
            }

        }

        private void AddBook()
        {
            string name, author, category, language, year, month, day, ISBN;

            Console.Write("Book name: ");
            name = Console.ReadLine();
            Console.Write("Author: ");
            author = Console.ReadLine();
            Console.Write("Category: ");
            category = Console.ReadLine();
            Console.Write("Language: ");
            language = Console.ReadLine();

            while (true)
            {
                Console.Write("Publication year: ");
                year = Console.ReadLine();
                Console.Write("Publication month: ");
                month = Console.ReadLine();
                Console.Write("Publication day: ");
                day = Console.ReadLine();

                try
                {
                    var date = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                }
                catch 
                {
                    Console.WriteLine("Enter valid date.");
                    Console.WriteLine();
                    continue;
                }
                break;
            }

            Console.Write("ISBN: ");
            ISBN = Console.ReadLine();
            Console.WriteLine();

            var book = new Book
            {
                Name = name,
                Author = author,
                Category = category,
                Language = language,
                PublicationDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day)),
                ISBN = ISBN
            };

            books.Add(book);
            File.WriteAllText(bookFile, JsonSerializer.Serialize(books));
        }

        private void TakeBook()
        {
            string name, book, days;

            Console.Write("You are: ");
            name = Console.ReadLine();

            bool personIsNew = people.TrueForAll(person => person.Name != name);
            if (personIsNew)
            {
                people.Add(new Person
                {
                    Name = name,
                    TakenBooks = new List<Book>(),
                    DaysTaken = new List<int>(),
                    DateTaken = new List<DateTime>()
                });
            }

            var currentPerson = GetCurrentPerson(name, people);

            if (currentPerson.TakenBooks.Count > 2)
            {
                Console.WriteLine("You have already taken 3 books. Return a book if you want to take another.");
                Console.WriteLine();
                return;
            }
            Console.WriteLine("Select the number of the book you want to take:");

            for (int i = 0; i < books.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {books[i].Name}");
            }
            Console.WriteLine();
            Console.Write("Book to take: ");
            book = Console.ReadLine();

            if (!int.TryParse(book, out _))
            {
                Console.WriteLine("Command must be a valid integer");
                Console.WriteLine();
                return;
            }

            var bookToTake = books[int.Parse(book) - 1];

            if (CheckIfTaken(bookToTake, people))
            {
                Console.WriteLine("Book is already taken. Please choose another book.");
                Console.WriteLine();
                return;
            }

            Console.Write("How long do you want to take it (days): ");
            days = Console.ReadLine();
            Console.WriteLine();

            if (!int.TryParse(days, out int intDays))
            {
                Console.WriteLine("Enter an integer");
                Console.WriteLine();
                return;
            }
            else if (intDays > 60)
            {
                Console.WriteLine("You can't take a book more than 2 months");
                Console.WriteLine();
                return;
            }
            else if (intDays < 0)
            {
                Console.WriteLine("Must be a positive number");
                Console.WriteLine();
                return;
            }

            currentPerson.DateTaken.Add(DateTime.Now);
            currentPerson.DaysTaken.Add(intDays);
            currentPerson.TakenBooks.Add(bookToTake);

            File.WriteAllText(peopleFile, JsonSerializer.Serialize(people));
        }

        private void ReturnBook()
        {
            string name, book;

            Console.Write("You are: ");
            name = Console.ReadLine();
            bool personIsNew = people.TrueForAll(person => person.Name != name);
            if (personIsNew)
            {
                Console.WriteLine("You haven't taken anything");
                Console.WriteLine();
                return;
            }

            var currentPerson = GetCurrentPerson(name, people);

            Console.WriteLine("Select the number of the book you want to return: ");

            for (int i = 0; i < currentPerson.TakenBooks.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {currentPerson.TakenBooks[i].Name}");
            }

            Console.Write("Book you want to return: ");
            book = Console.ReadLine();

            if (!int.TryParse(book, out int bookIndex))
            {
                Console.WriteLine("Command must be a valid integer");
                Console.WriteLine();
                return;
            }
            else if ((bookIndex - 1) < 0 || (bookIndex - 1) > currentPerson.TakenBooks.Count)
            {
                Console.WriteLine("Integer is out of range");
                Console.WriteLine();
                return;
            }

            if (currentPerson.DateTaken[bookIndex - 1].AddDays(currentPerson.DaysTaken[bookIndex - 1]) < DateTime.Now)
            {
                Console.WriteLine("You are late. Next time try to return book on time.");
            }
            Console.WriteLine();

            currentPerson.DateTaken.RemoveAt(bookIndex - 1);
            currentPerson.DaysTaken.RemoveAt(bookIndex - 1);
            currentPerson.TakenBooks.RemoveAt(bookIndex - 1);

            File.WriteAllText(peopleFile, JsonSerializer.Serialize(people));
        }

        private void ListBooks()
        {
            string command;

            Console.WriteLine();
            Console.WriteLine("1: List all books");
            Console.WriteLine("2: Filter by name");
            Console.WriteLine("3: Filter by author");
            Console.WriteLine("4: Filter by category");
            Console.WriteLine("5: Filter by language");
            Console.WriteLine("6: Filter by ISBN");
            Console.WriteLine("7: Filter available and taken books");
            Console.Write("Your command: ");
            command = Console.ReadLine();
            Console.WriteLine();

            switch (command)
            {
                case "1":
                    books.ForEach(x => Console.WriteLine(x + "\n"));
                    break;

                case "2":
                    Console.Write("Name: ");
                    string name = Console.ReadLine();
                    Console.WriteLine();
                    var filteredBooksName = books.Where(book => book.Name == name).ToList();
                    filteredBooksName.ForEach(book => Console.WriteLine(book + "\n"));
                    break;

                case "3":
                    Console.Write("Author: ");
                    string author = Console.ReadLine();
                    Console.WriteLine();
                    var filteredBooksAuthor = books.Where(book => book.Author == author).ToList();
                    filteredBooksAuthor.ForEach(book => Console.WriteLine(book + "\n"));
                    break;

                case "4":
                    Console.Write("Category: ");
                    string category = Console.ReadLine();
                    Console.WriteLine();
                    var filteredBooksCategory = books.Where(book => book.Category == category).ToList();
                    filteredBooksCategory.ForEach(book => Console.WriteLine(book + "\n"));
                    break;

                case "5":
                    Console.Write("Language: ");
                    string language = Console.ReadLine();
                    Console.WriteLine();
                    var filteredBooksLanguage = books.Where(book => book.Language == language).ToList();
                    filteredBooksLanguage.ForEach(book => Console.WriteLine(book + "\n"));
                    break;

                case "6":
                    Console.Write("ISBN: ");
                    string ISBN = Console.ReadLine();
                    Console.WriteLine();
                    var filteredBooksISBN = books.Where(book => book.ISBN == ISBN).ToList();
                    filteredBooksISBN.ForEach(book => Console.WriteLine(book + "\n"));
                    break;

                case "7":
                    Console.Write("Available (1) or taken (2): ");
                    string availableTaken = Console.ReadLine();
                    Console.WriteLine();
                    switch (availableTaken)
                    {
                        case "1":
                            var filteredBooksAvailable = books.Where(book => !CheckIfTaken(book, people)).ToList();
                            filteredBooksAvailable.ForEach(book => Console.WriteLine(book + "\n"));
                            break;

                        case "2":
                            var filteredBooksTaken = books.Where(book => CheckIfTaken(book, people)).ToList();
                            filteredBooksTaken.ForEach(book => Console.WriteLine(book + "\n"));
                            break;

                        default:
                            Console.WriteLine("Invalid command");
                            break;
                    }
                    break;

                default:
                    Console.WriteLine("Invalid command");
                    Console.WriteLine();
                    break;
            }
            Console.WriteLine();
            
        }

        private void DeleteBook()
        {
            Console.WriteLine("Select the number of the book you want to delete:");
            for (int i = 0; i < books.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {books[i].Name}");
            }
            Console.WriteLine();
            Console.Write("Book to delete: ");
            string toDelete = Console.ReadLine();

            if (!int.TryParse(toDelete, out int intToDelete))
            {
                Console.WriteLine("Input should be an integer");
                Console.WriteLine();
            } 
            else if (intToDelete < 1 || intToDelete > books.Count)
            {
                Console.WriteLine("Select a valid number");
                Console.WriteLine();
            } 
            else
            {
                books.RemoveAt(intToDelete - 1);
                File.WriteAllText(bookFile, JsonSerializer.Serialize(books));
            }
        }

        public bool CheckIfTaken(Book book, List<Person> people)
        {
            bool bookIsTaken = false;
            people.ForEach(person => person.TakenBooks.ForEach(x =>
            {
                if (JsonSerializer.Serialize(x) == JsonSerializer.Serialize(book))
                {
                    bookIsTaken = true;
                }
            }));
            return bookIsTaken;
        }

        public Person GetCurrentPerson(string name, List<Person> people)
        {
            Person currentPerson = new();

            people.ForEach(person =>
            {
                if (person.Name == name)
                {
                    currentPerson = person;
                }
            });

            return currentPerson;
        }
    }
}
