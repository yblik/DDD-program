using Microsoft.VisualStudio.TestTools.UnitTesting;
using DDD_program;

[TestClass]
public class RoleLogTests
{
    //NORMAL
    // Ensures choosing option 1 correctly sets the Student role and RoleID
    [TestMethod]
    public void SetProfile_Student_SetsCorrectRole()
    {
        var log = new RoleLog();
        log.SetProfile(1);

        Assert.AreEqual(RoleLog.Role.student, log.SelectedProfile);
        Assert.AreEqual(1, log.RoleID);
    }

    //NORMAL
    // Ensures choosing option 2 correctly sets the Supervisor rol
    [TestMethod]
    public void SetProfile_Supervisor_SetsCorrectRole()
    {
        var log = new RoleLog();
        log.SetProfile(2);

        Assert.AreEqual(RoleLog.Role.Supervisor, log.SelectedProfile);
        Assert.AreEqual(2, log.RoleID);
    }

    //BOUNDARY/ERRONEOUS 
    // Any invalid selection should fall back to SeniorTutor
    [TestMethod]
    public void SetProfile_InvalidDefault_SetsSeniorTutor()
    {
        var log = new RoleLog();
        log.SetProfile(99);   // invalid input

        Assert.AreEqual(RoleLog.Role.SeniorTutor, log.SelectedProfile);
        Assert.AreEqual(3, log.RoleID);
    }
}
