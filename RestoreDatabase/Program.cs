using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RestoreDatabase
{
  class Program
  {
    static void Main(string[] arguments)
    {
      Action<string> display = Console.WriteLine;
      // check if several full backup files
      // if so delete oldest
      //check if diff files older than full
      // if so delete them
      // generate restore SQL script file
      string initialDirectory = string.Empty;
      if (arguments.Length > 0 && !string.IsNullOrEmpty(arguments[0]))
      {
        initialDirectory = arguments[0];
      }
      else
      {
        initialDirectory = @"E:\Partage\DepotTMA\ApplicationName\";
      }

      string pattern = "*.*";
      if (arguments.Length > 0 && !string.IsNullOrEmpty(arguments[1]))
      {
        pattern = arguments[1];
      }
      else
      {
        pattern = @"ApplicationName_*.*";
      }

      if (!Directory.Exists(initialDirectory))
      {
        display($"Le répertoire {initialDirectory} n'a pas été trouvé.");
        return;
      }
      var files = Directory.GetFiles(initialDirectory, pattern);
      foreach (var fileName in files)
      {
        display($"{Path.GetExtension(fileName)} {GetDateFromFileName(fileName)}");
      }

      // delete oldest full file
      if (files.Where(f => f.Contains(".full")).Count() > 1)
      {
        string oldestFileName = GetOldestFileName(files, ".full");
        display($"oldest ful file is: {oldestFileName}");
        File.Delete(oldestFileName);
        files = Directory.GetFiles(initialDirectory, pattern);
      }

      var dateFromFull = GetDateFromFileName(files.Where(f => f.Contains(".full")).First());
      display($"date from full {dateFromFull}");
      // delete all diff files older than full
      var allDiffFiles = files.Where(f => f.Contains(".diff"));
      foreach (var item in allDiffFiles)
      {
        display($"{item}");
      }

      var oldDiffFilePriorToFull = GetListOfOlder(allDiffFiles, dateFromFull);
      display($"il y a {oldDiffFilePriorToFull.Count} DIFF file(s) prior to the full backup which was done on {dateFromFull} ");

      if (oldDiffFilePriorToFull.Count > 0)
      {
        foreach (var item in oldDiffFilePriorToFull)
        {
          display($"{item}");
          File.Delete(item);
        }
      }

      /*
       USE [master]
ALTER DATABASE [serverName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
BACKUP LOG [serverName] TO  DISK = N'C:\MSSQL\Backup\serverName_LogBackup_date.bak' WITH NOFORMAT, NOINIT,  NAME = N'serverName_LogBackup_date', NOSKIP, NOREWIND, NOUNLOAD,  NORECOVERY ,  STATS = 5
RESTORE DATABASE [serverName] FROM  DISK = N'C:\Path\serverName\serverName_backup_date.full' WITH  FILE = 1,  MOVE N'GestAMI_data' TO N'C:\MSSQL\DATA\serverName.mdf',  MOVE N'GestAMI_log' TO N'E:\MSSQL\DATA\serverName.ldf',  NORECOVERY,  NOUNLOAD,  REPLACE,  STATS = 5
RESTORE DATABASE [serverName] FROM  DISK = N'C:\Path\serverName\serverName_backup_date.diff' WITH  FILE = 1,  NOUNLOAD,  STATS = 5
ALTER DATABASE [serverName] SET MULTI_USER
GO
       * */

      display("Press any key to exit:");
      Console.ReadKey();

    }

    private static List<string> GetListOfOlder(IEnumerable<string> list, DateTime date)
    {
      List<string> result = new List<string>();
      foreach (var item in list)
      {
        if (GetDateFromFileName(item) < date)
        {
          result.Add(item);
        }
      }

      return result;
    }
    private static string GetOldestFileName(string[] files, string extension)
    {
      string result = string.Empty;
      var subList = files.Where(f => f.Contains(".full"));
      DateTime file1 = GetDateFromFileName(subList.ToArray()[0]);
      DateTime file2 = GetDateFromFileName(subList.ToArray()[1]);
      result = subList.ToList().Min(date => date).ToString();
      return result;
    }

    private static DateTime GetDateFromFileName(string fileName)
    {
      // ApplicationName_C_26_backup_2020_04_17_21_02_14_900.full
      DateTime result;
      var dateSplitted = fileName.Split('_');
      int annee = int.Parse(dateSplitted[4]);
      int mois = int.Parse(dateSplitted[5]);
      int jour = int.Parse(dateSplitted[6]);
      int heure = int.Parse(dateSplitted[7]);
      int minute = int.Parse(dateSplitted[8]);
      int seconde = int.Parse(dateSplitted[9]);

      result = new DateTime(annee, mois, jour, heure, minute, seconde);
      return result;
    }
  }
}