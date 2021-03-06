﻿using System;
using System.IO;

namespace RestoreDatabase
{
  public class FileName : IComparable<FileName>
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
      var longDate = name.Split('_');
      string result;
      if (longDate[0].ToLower().StartsWith("masterdata"))
      {
        result = longDate[0];
      }
      else
      {
        result = $"{longDate[1]}_{longDate[2]}";
      }

      return result;
    }

    private DateTime CalculateDate(string name)
    {
      // Gestion_X_2_backup_2020_10_02_20_51_40_320.full
      // MASTERDATABASE_backup_2020_10_02_20_51_40_320.full
      var longDate = name.Split('_');
      int startIndex;
      if (longDate[0].ToLower().StartsWith("masterdata"))
      {
        startIndex = 2;
      }
      else
      {
        startIndex = 4;
      }

      DateTime result = new DateTime(int.Parse(longDate[startIndex]), int.Parse(longDate[startIndex + 1]), int.Parse(longDate[startIndex + 2]), int.Parse(longDate[startIndex + 3]), int.Parse(longDate[startIndex + 4]), int.Parse(longDate[startIndex + 5]));

      return result;
    }

    public int CompareTo(FileName otherFileName)
    {
      return DateOfFile.CompareTo(otherFileName.DateOfFile);
    }

    public override bool Equals(object objectToBeCompared)
    {
      return objectToBeCompared is FileName name &&
             LongName == name.LongName &&
             Extension == name.Extension &&
             DateOfFile == name.DateOfFile &&
             DatabaseName == name.DatabaseName &&
             IsDiffFile == name.IsDiffFile &&
             IsFullFile == name.IsFullFile;
    }

    public static bool operator ==(FileName left, FileName right)
    {
      return left.DateOfFile == right.DateOfFile;
    }

    public static bool operator !=(FileName left, FileName right)
    {
      return !(left.DateOfFile == right.DateOfFile);
    }

    public static bool operator <(FileName left, FileName right)
    {
      return ReferenceEquals(left.DateOfFile, null) ? !ReferenceEquals(right.DateOfFile, null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(FileName left, FileName right)
    {
      return left is null || left.CompareTo(right) <= 0;
    }

    public static bool operator >(FileName left, FileName right)
    {
      return left is object && left.CompareTo(right) > 0;
    }

    public static bool operator >=(FileName left, FileName right)
    {
      return left is null ? right is null : left.CompareTo(right) >= 0;
    }

    public override int GetHashCode()
    {
      return 1;
    }
  }
}
