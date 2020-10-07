using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestoreDatabase;


namespace UnitTestRestoreDatabase
{
  [TestClass]
  public class UnitTestListOfFiles
  {
    [TestMethod]
    public void TestMethod1()
    {
      //GESTAMI_J_4_backup_2020_10_06_20_00_07_127.diff
      string source = "GESTAMI_J_4_backup_2020_10_06_20_00_07_127.diff";
      string expected = "J_4";
      var result1 = new FileName(source);
      var result = result1.DatabaseName;
      Assert.AreEqual(expected, result);
    }
  }
}
