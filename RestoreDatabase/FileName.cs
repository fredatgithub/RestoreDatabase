using System;
using System.IO;

namespace RestoreDatabase
{
  public class FileName
  {
    public string LongName { get; set; }
    public string Extension { get; set; }
    public DateTime DateOfFile { get; set; }
    public string DatabaseName { get; set; }
    public bool IsDiffFile { get; set; }
    public bool IsFullFile { get; set; }

    public FileName()
    {
      LongName = string.Empty;
      Extension = string.Empty;
      IsDiffFile = false;
      IsFullFile = false;
    }

    public FileName(string name, string extension, DateTime date, string databaseName = "")
    {
      LongName = name;
      Extension = extension;
      DateOfFile = date;
      DatabaseName = DatabaseName;
      IsDiffFile = false;
      IsFullFile = false;
    }

    public FileName(string name)
    {
      LongName = name;
      if (name.EndsWith("diff"))
      {
        Extension = "diff";
        IsDiffFile = true;
        IsFullFile = false;
      }
      else if (name.EndsWith("full"))
      {
        Extension = "full";
        IsDiffFile = false;
        IsFullFile = true;
      }
      else
      {
        Extension = string.Empty;
        IsDiffFile = false;
        IsFullFile = false;
      }

      DateOfFile = CalculateDate(Path.GetFileName(name));
      DatabaseName = CalculateDatabaseName(Path.GetFileName(name));
    }

    private string CalculateDatabaseName(string name)
    {
      string result = string.Empty;
      var longDate = name.Split('_');
      result = $"{longDate[1]}_{longDate[2]}";
      return result;
    }

    private DateTime CalculateDate(string name)
    {
      // Gestion_X_2_backup_2020_10_02_20_51_40_320.full
      var longDate = name.Split('_');
      DateTime result = new DateTime(int.Parse(longDate[4]), int.Parse(longDate[5]), int.Parse(longDate[6]));
      return result;
    }
  }
}
