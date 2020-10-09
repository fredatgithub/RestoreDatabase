using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestoreDatabase;

namespace UnitTestRestoreDatabase
{
  [TestClass]
  public class UnitTestListOfFiles
  {
    [TestMethod]
    public void TestMethod_file_name()
    {
      //GESTION_J_4_backup_2020_10_06_20_00_07_127.diff
      string source = "GESTION_J_4_backup_2020_10_06_20_00_07_127.diff";
      string expected = "J_4";
      var result1 = new FileName(source);
      var result = result1.DatabaseName;
      Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void TestMethod_Compare_date_later_than()
    {
      //GESTION_J_4_backup_2020_10_06_20_00_07_127.diff
      string source1 = "GESTION_J_4_backup_2020_10_06_20_00_07_127.diff";
      var source10 = new FileName(source1);
      string source2 = "GESTION_J_4_backup_2020_10_07_20_00_07_127.diff";
      var source20 = new FileName(source2);
      int expected = -1;
      var result = source10.CompareTo(source20);
      Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void TestMethod_Compare_date_similar()
    {
      //GESTION_J_4_backup_2020_10_06_20_00_07_127.diff
      string source1 = "GESTION_J_4_backup_2020_10_06_20_00_07_127.diff";
      var source10 = new FileName(source1);
      string source2 = "GESTION_J_4_backup_2020_10_06_20_00_07_127.diff";
      var source20 = new FileName(source2);
      int expected = 0;
      var result = source10.CompareTo(source20);
      Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void TestMethod_Compare_date_earlier_than()
    {
      //GESTION_J_4_backup_2020_10_06_20_00_07_127.diff
      string source1 = "GESTION_J_4_backup_2020_10_06_20_00_07_127.diff";
      var source10 = new FileName(source1);
      string source2 = "GESTION_J_4_backup_2020_10_05_20_00_07_127.diff";
      var source20 = new FileName(source2);
      int expected = 1;
      var result = source10.CompareTo(source20);
      Assert.AreEqual(expected, result);
    }
  }
}
