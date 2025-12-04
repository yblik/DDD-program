using DDD_program.MenuLogic;

namespace UnitTest
{
    [TestClass]
    public class ConsoleHelperTests
    {
        //uses set in and outs to simulate console input and capture output

        //arrange is prep
        //act is doing
        //assert is checking

        //use input reader to simulate user input (sort of store)
        private TextWriter _originalOut;
        private TextReader _originalIn;

        [TestInitialize]
        public void TestInitialize()
        {
            // Store original console streams
            _originalOut = Console.Out;
            _originalIn = Console.In;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Restore original console streams
            Console.SetOut(_originalOut);
            Console.SetIn(_originalIn);
        }

        // Feature 1: GetIntegerInRange
        [TestMethod]
        public void GetIntegerInRange_Valid_ReturnsInt()
        {
            // Arrange
            var input = "5\n"; //user types 5 then enter
            var inputReader = new StringReader(input);
            Console.SetIn(inputReader);
            var outputWriter = new StringWriter();
            Console.SetOut(outputWriter);

            // Act
            var result = ConsoleHelper.GetIntegerInRange(1, 10, "Test message");

            // Assert
            Assert.AreEqual(5, result); //if it reaches 5
            StringAssert.Contains(outputWriter.ToString(), "Test message");
        }

        [TestMethod]
        public void GetIntegerInRange_MinBoundary_ReturnsMin()
        {
            // Arrange
            var input = "1\n";
            Console.SetIn(new StringReader(input));
            Console.SetOut(new StringWriter());

            // Act
            var result = ConsoleHelper.GetIntegerInRange(1, 10, "Test");

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void GetIntegerInRange_MaxBoundary_ReturnsMax()
        {
            // Arrange
            var input = "10\n";
            Console.SetIn(new StringReader(input));
            Console.SetOut(new StringWriter());

            // Act
            var result = ConsoleHelper.GetIntegerInRange(1, 10, "Test");

            // Assert
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void GetIntegerInRange_InvalidThenValidInput_ReturnsValid()
        {
            // Arrange
            var input = "abc\n15\n";
            Console.SetIn(new StringReader(input));
            var outputWriter = new StringWriter();
            Console.SetOut(outputWriter);

            // Act
            var result = ConsoleHelper.GetIntegerInRange(1, 10, "Test");

            // Assert
            Assert.AreEqual(15, result);
            StringAssert.Contains(outputWriter.ToString(), "is not a number");
        }

        [TestMethod]
        public void GetIntegerInRange_OutOfRange_ReturnsValid()
        {
            // Arrange
            var input = "0\n11\n5\n";
            Console.SetIn(new StringReader(input));
            var outputWriter = new StringWriter();
            Console.SetOut(outputWriter);

            // Act
            var result = ConsoleHelper.GetIntegerInRange(1, 10, "Test");

            // Assert
            Assert.AreEqual(5, result);
            StringAssert.Contains(outputWriter.ToString(), "is not between");
        }

        // Feature 2: GetTimeInRange
        [TestMethod]
        public void GetTimeInRange_ValidTime_ReturnsTimeSpan()
        {
            // Arrange
            var input = "13:30\n";
            Console.SetIn(new StringReader(input));
            Console.SetOut(new StringWriter());

            var min = new TimeSpan(12, 0, 0);
            var max = new TimeSpan(18, 0, 0);

            // Act
            var result = ConsoleHelper.GetTimeInRange(min, max, "Enter time");

            // Assert
            Assert.AreEqual(new TimeSpan(13, 30, 0), result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetTimeInRange_MinGreaterThanMax_ThrowsArgumentException()
        {
            // Arrange
            var min = new TimeSpan(18, 0, 0);
            var max = new TimeSpan(12, 0, 0);

            // Act & Assert (via ExpectedException)
            ConsoleHelper.GetTimeInRange(min, max, "Test");
        }

        [TestMethod]
        public void GetTimeInRange_InvalidTimeFormat_Valid()
        {
            // Arrange
            var input = "invalid\n13:30\n";
            Console.SetIn(new StringReader(input));
            var outputWriter = new StringWriter();
            Console.SetOut(outputWriter);

            var min = new TimeSpan(12, 0, 0);
            var max = new TimeSpan(18, 0, 0);

            // Act
            var result = ConsoleHelper.GetTimeInRange(min, max, "Test");

            // Assert
            Assert.AreEqual(new TimeSpan(13, 30, 0), result);
            StringAssert.Contains(outputWriter.ToString(), "is not a valid time format");
        }

        [TestMethod]
        public void GetTimeInRange_TimeOutOfRange_Valid()
        {
            // Arrange
            var input = "11:30\n19:00\n14:00\n";
            Console.SetIn(new StringReader(input));
            var outputWriter = new StringWriter();
            Console.SetOut(outputWriter);

            var min = new TimeSpan(12, 0, 0);
            var max = new TimeSpan(18, 0, 0);

            // Act
            var result = ConsoleHelper.GetTimeInRange(min, max, "Test");

            // Assert
            Assert.AreEqual(new TimeSpan(14, 0, 0), result);
            StringAssert.Contains(outputWriter.ToString(), "is not between");
        }

        // Feature 3: GetSelectionFromMenu
        [TestMethod]
        public void GetSelectionFromMenu_ValidSelection_ReturnsCorrectIndex()
        {
            // Arrange
            var items = new List<string> { "Option 1", "Option 2", "Option 3" };
            var input = "2\n";
            Console.SetIn(new StringReader(input));
            var outputWriter = new StringWriter();
            Console.SetOut(outputWriter);

            // Act
            var result = ConsoleHelper.GetSelectionFromMenu(items, "Choose option:");

            // Assert
            Assert.AreEqual(1, result); // Zero-based index
            var output = outputWriter.ToString();
            StringAssert.Contains(output, "Choose option:");
            StringAssert.Contains(output, "1. Option 1");
            StringAssert.Contains(output, "2. Option 2");
            StringAssert.Contains(output, "3. Option 3");
        }

        [TestMethod]
        public void GetSelectionFromMenu_InvalidThenValidSelection_Valid()
        {
            // Arrange
            var items = new List<string> { "A", "B", "C" };
            var input = "0\n4\n2\n"; // Invalid low, invalid high, valid
            Console.SetIn(new StringReader(input));
            var outputWriter = new StringWriter();
            Console.SetOut(outputWriter);

            // Act
            var result = ConsoleHelper.GetSelectionFromMenu(items, "Menu:");

            // Assert
            Assert.AreEqual(1, result); // Zero-based index for "B"
        }

    }
}
