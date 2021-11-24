using Microsoft.VisualStudio.TestTools.UnitTesting;
using Internship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Internship.Tests
{
    [TestClass()]
    public class UserInterfaceTests
    {
        [TestMethod()]
        public void CheckIfTakenTest()
        {
            var userInterface = new UserInterface();
            var people = new List<Person>();
            people.Add(new Person
            {
                Name = "person",
                TakenBooks = new List<Book>(),
                DaysTaken = new List<int>(),
                DateTaken = new List<DateTime>()
            });
            people[0].TakenBooks.Add(new Book
            {
                Name = "book",
                Author = "writer",
                Category = "horror",
                Language = "english",
                PublicationDate = new DateTime(2000, 05, 12),
                ISBN = "123-456-789"
            });
            people[0].DateTaken.Add(DateTime.Now);
            people[0].DaysTaken.Add(30);

            var book = new Book
            {
                Name = "book",
                Author = "writer",
                Category = "horror",
                Language = "english",
                PublicationDate = new DateTime(2000, 05, 12),
                ISBN = "123-456-789"
            };

            Assert.AreEqual(true, userInterface.CheckIfTaken(book, people));
            Assert.AreEqual(false, userInterface.CheckIfTaken(new Book(), people));
        }

        [TestMethod()]
        public void GetCurrentPersonTest()
        {
            var userInterface = new UserInterface();
            var people = new List<Person>();
            people.Add(new Person
            {
                Name = "person",
                TakenBooks = new List<Book>(),
                DaysTaken = new List<int>(),
                DateTaken = new List<DateTime>()
            });
            Assert.AreEqual(true, JsonSerializer.Serialize(people[0]) == JsonSerializer.Serialize(userInterface.GetCurrentPerson("person", people)));
            Assert.AreEqual(false, JsonSerializer.Serialize(people[0]) == JsonSerializer.Serialize(userInterface.GetCurrentPerson("wrongperson", people)));
        }

        
    }
}