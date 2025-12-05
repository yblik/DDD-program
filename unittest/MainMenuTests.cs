using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD_program.MenuLogic;
using DDD_program.Menus;

    [TestClass]
    public  class MainMenuTests
    {
        private TextWriter _originalOut;
        private TextReader _originalIn;

        [TestInitialize]
        public void TestInitialize()
        {
            _originalOut = Console.Out;
            _originalIn = Console.In;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Console.SetOut(_originalOut);
            Console.SetIn(_originalIn);
        }

        [TestMethod]
        public void Constructor_SetsRoleAndUsername()
        {
            // Arrange & Act
            var mainMenu = new MainMenu(2, "supervisor123");

            // Assert
            Assert.AreEqual(2, mainMenu.Role);
            Assert.AreEqual("supervisor123", mainMenu.Username);
        }

        [TestMethod]
        public void Constructor_WithStudentRole_SetsProperties()
        {
            // Arrange & Act
            var mainMenu = new MainMenu(1, "student456");

            // Assert
            Assert.AreEqual(1, mainMenu.Role);
            Assert.AreEqual("student456", mainMenu.Username);
        }

        [TestMethod]
        public void Constructor_WithSeniorTutorRole_SetsProperties()
        {
            // Arrange & Act
            var mainMenu = new MainMenu(3, "tutor789");

            // Assert
            Assert.AreEqual(3, mainMenu.Role);
            Assert.AreEqual("tutor789", mainMenu.Username);
        }

    }
