using DDD_program.MenuLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ConsoleHelperShortTests
{
    private TextWriter originalOut;
    private TextReader originalIn;

    [TestInitialize]
    public void Setup()
    {
        originalOut = Console.Out;
        originalIn = Console.In;
    }

    [TestCleanup]
    public void Cleanup()
    {
        Console.SetOut(originalOut);
        Console.SetIn(originalIn);
    }

    //NORMAL
    [TestMethod]
    public void GetIntegerInRange_ReturnsValid()
    {
        Console.SetIn(new StringReader("5\n"));
        int value = ConsoleHelper.GetIntegerInRange(1, 10, "Enter number");
        Assert.AreEqual(5, value);
    }

    //BOUNDARY
    [TestMethod]
    public void GetIntegerInRange_ReturnsMinBoundary()
    {
        Console.SetIn(new StringReader("1\n"));
        int value = ConsoleHelper.GetIntegerInRange(1, 10, "Enter");
        Assert.AreEqual(1, value);
    }

    //ERRONEOUS
    [TestMethod]
    public void GetIntegerInRange_InvalidInput_ShowsError()
    {
        var input = new StringReader("abc\n7\n");
        var output = new StringWriter();

        Console.SetIn(input);
        Console.SetOut(output);

        int value = ConsoleHelper.GetIntegerInRange(1, 10, "Enter");

        Assert.AreEqual(7, value);
        StringAssert.Contains(output.ToString(), "not a number");
    }
}
