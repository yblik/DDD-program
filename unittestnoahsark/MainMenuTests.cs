using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD_program.Menus;

namespace UnitTest
{
    internal class MainMenuTests
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

        [TestMethod]
        public void CreateMenu_StudentRole_AddsCorrectMenuItems()
        {
            // Arrange
            var mainMenu = new MainMenu(1, "student123");
            var outputWriter = new StringWriter();
            Console.SetOut(outputWriter);

            // Act
            mainMenu.CreateMenu();

            string menuText = mainMenu.ToString();

            // Assert - Student should see specific menu items
            StringAssert.Contains(menuText, "Choose action");

            Assert.Inconclusive("Need to refactor to expose menu items or use reflection");
        }

        [TestMethod]
        public void CreateMenu_StudentRole_CallsFeel()
        {
            // Arrange
            var mainMenu = new MainMenu(1, "student123");
            var outputWriter = new StringWriter();
            Console.SetOut(outputWriter);

            // Act
            mainMenu.CreateMenu();

            // Assert - PopUpFeel() should have been called
            // We can check if feeling prompt appears
            string output = outputWriter.ToString();

            // Since PopUpFeel() calls SetFeelingMenuItem.Select()
            // which writes to console, we should see something
            // However, this depends on SetFeelingMenuItem implementation

            Assert.Inconclusive("Need to mock SetFeelingMenuItem to verify call");
        }

        [TestMethod]
        public void CreateMenu_StudentRole_CallsCalendar()
        {
            // Arrange
            var mainMenu = new MainMenu(1, "student123");
            var outputWriter = new StringWriter();
            Console.SetOut(outputWriter);

            // Act
            mainMenu.CreateMenu();

            // Assert - DrawCalendar() should output calendar lines
            string output = outputWriter.ToString();

            // DrawCalendar() writes "---------------------------------------------------"
            Assert.IsTrue(output.Contains("---"));
            Assert.IsTrue(output.Contains("Upcoming meetings") || output.Contains("MeetingHUD"));
        }

        [TestMethod]
        public void CreateMenu_SupervisorRole_AddsViewStudentsDashboard()
        {
            var mainMenu = new MainMenu(2, "supervisor456");
            var outputWriter = new StringWriter();
            Console.SetOut(outputWriter);

            mainMenu.CreateMenu();

            string menuText = mainMenu.ToString();


            Assert.Inconclusive("Need to verify ViewStudentsDashboardMenuItem is added");
        }

        [TestMethod]
        public void CreateMenu_SupervisorRole_DoesNotCallPopUpFeel()
        {
            var mainMenu = new MainMenu(2, "supervisor456");
            var outputWriter = new StringWriter();
            Console.SetOut(outputWriter);

            mainMenu.CreateMenu();


            Assert.Inconclusive("Need to verify SetFeelingMenuItem not instantiated");
        }

        [TestMethod]
        public void CreateMenu_SeniorTutorRole_AddsAdminMenuItems()
        {
            var mainMenu = new MainMenu(3, "tutor789");
            var outputWriter = new StringWriter();
            Console.SetOut(outputWriter);

            mainMenu.CreateMenu();


            Assert.Inconclusive("Need to verify admin menu items are added");
        }

    }
}
