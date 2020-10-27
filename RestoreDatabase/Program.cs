using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RestoreDatabase
{
  public class Program
  {
    public static void Main(string[] arguments)
    {
      Action<string> display = Console.WriteLine;
      display("RestoreDatabase Version 1.0");
      display(Environment.NewLine);
      display("Removing duplicate full and keeping the latest one");
      display(Environment.NewLine);
      display("Removing oldest diff files and keeping the latest one");
      display(Environment.NewLine);
      // check if several full backup files
      // if so delete oldest
      //check if diff files older than full
      // if so delete them
      // generate restore SQL script file
      string initialDirectory = string.Empty;
      string applicationName = "applicationName";
      string fullBackupFileName = string.Empty;
      string lastDiffBackupFileName = string.Empty;
      bool deletefiles = true;
      if (arguments.Length > 0 && !string.IsNullOrEmpty(arguments[0]))
      {
        initialDirectory = arguments[0];
      }
      else
      {
        if (Directory.Exists(Properties.Settings.Default.StartingDirectory))
        {
          initialDirectory = Properties.Settings.Default.StartingDirectory;
        }
        else if (Directory.Exists(@"E:\Partage\BackupProd\"))
        {
          initialDirectory = @"E:\Partage\BackupProd\";
        }
        else
        {
          initialDirectory = @"C:\";
        }
      }

      string pattern = "*.*";
      if (arguments.Length > 0 && !string.IsNullOrEmpty(arguments[1]))
      {
        pattern = arguments[1];
      }
      else
      {
        pattern = @"*.*";
      }

      if (!Directory.Exists(initialDirectory))
      {
        display($"Le répertoire {initialDirectory} n'a pas été trouvé.");
        return;
      }

      var files = Directory.GetFiles(initialDirectory, pattern);

      ListOfFileName listOfAllfiles = new ListOfFileName();
      //Gestion_J_4_backup_2020_10_06_20_00_07_127.diff
      foreach (var fileName in files)
      {
        FileName tmpFile = new FileName(fileName);
        listOfAllfiles.ListOfFiles.Add(tmpFile);
        if (tmpFile.IsDiffFile)
        {
          listOfAllfiles.ListOfDiff.Add(tmpFile);
        }
        else if (tmpFile.IsFullFile)
        {
          listOfAllfiles.ListOfFull.Add(tmpFile);
        }
      }

      // create sub-directories if not exist
      foreach (FileName fileName in listOfAllfiles.ListOfFiles)
      {
        string databaseName = fileName.DatabaseName;
        string directory = Path.GetDirectoryName(fileName.LongName) + $"\\{databaseName}";
        if (!Directory.Exists(directory))
        {
          Directory.CreateDirectory(directory);
        }
      }

      // move files to sub-directories: diff and full
      foreach (FileName fileName in listOfAllfiles.ListOfFiles)
      {
        //Gestion_A_3_backup_2020_10_02_20_51_40_320.full
        string databaseName = fileName.DatabaseName;
        string directory = Path.GetDirectoryName(fileName.LongName) + $"\\{databaseName}";
        string targetFileName = $"{directory}\\{Path.GetFileName(fileName.LongName)}";
        try
        {
          File.Move(fileName.LongName, targetFileName);
        }
        catch (Exception exception)
        {
          Console.WriteLine($"error while trying to move a file: {exception.Message}");
          //if file already exists on target directory, then delete it
          if (File.Exists(targetFileName))
          {
            try
            {
              if (deletefiles)
              {
                File.Delete(fileName.LongName);
              }
            }
            catch (Exception exception2)
            {
              Console.WriteLine($"Error while deleting the file: {fileName.LongName}");
              Console.WriteLine($"The exception is {exception2.Message}");
            }
          }
        }
      }

      // get all files now in a sub-directories
      //Gestion_J_4_backup_2020_10_06_20_00_07_127.diff
      files = Directory.GetFiles(initialDirectory, pattern, SearchOption.AllDirectories);
      listOfAllfiles = new ListOfFileName();
      foreach (var fileName in files)
      {
        FileName tmpFile = new FileName(fileName);
        listOfAllfiles.ListOfFiles.Add(tmpFile);
        if (tmpFile.IsDiffFile)
        {
          listOfAllfiles.ListOfDiff.Add(tmpFile);
        }
        else if (tmpFile.IsFullFile)
        {
          listOfAllfiles.ListOfFull.Add(tmpFile);
        }
      }

      // search for and delete duplicate full
      Dictionary<string, int> dicoNumberFull = new Dictionary<string, int>();
      Dictionary<string, List<string>> listOfDuplicateFull = new Dictionary<string, List<string>>();
      foreach (FileName fileName in listOfAllfiles.ListOfFull)
      {
        if (!dicoNumberFull.ContainsKey(fileName.DatabaseName))
        {
          dicoNumberFull.Add(fileName.DatabaseName, 1);
          listOfDuplicateFull.Add(fileName.DatabaseName, new List<string> { fileName.LongName });
        }
        else
        {
          dicoNumberFull[fileName.DatabaseName]++;
          listOfDuplicateFull[fileName.DatabaseName].Add(fileName.LongName);
        }
      }

      // delete duplicate full
      foreach (var item in listOfDuplicateFull)
      {
        if (item.Value.Count > 1)
        {
          var oldestFile = GetOldestFileName(item.Value);
          try
          {
            if (deletefiles)
            {
              File.Delete(oldestFile);
            }
          }
          catch (Exception exception)
          {
            Console.WriteLine($"Error while trying to delete duplicate full file: {exception.Message}");
          }
        }
      }

      // delete diff before full and all diff but lastest one
      Dictionary<string, int> dicoNumberDiff = new Dictionary<string, int>();
      Dictionary<string, List<string>> listOfDiffBeforeFull = new Dictionary<string, List<string>>();
      foreach (FileName fileName in listOfAllfiles.ListOfDiff)
      {
        if (!dicoNumberDiff.ContainsKey(fileName.DatabaseName))
        {
          dicoNumberDiff.Add(fileName.DatabaseName, 1);
          listOfDiffBeforeFull.Add(fileName.DatabaseName, new List<string> { fileName.LongName });
        }
        else
        {
          dicoNumberDiff[fileName.DatabaseName]++;
          listOfDiffBeforeFull[fileName.DatabaseName].Add(fileName.LongName);
        }
      }

      // delete oldest diff files and keep the latest diff file before full
      foreach (var item in listOfDiffBeforeFull)
      {
        if (item.Value.Count > 1)
        {

          var oldestFiles = GetOldestFileNames(item.Value);
          try
          {
            foreach (var file in oldestFiles)
            {
              if (deletefiles)
              {
                File.Delete(file);
              }
            }
          }
          catch (Exception exception)
          {
            Console.WriteLine($"Error while trying to delete oldest diff file: {exception.Message}");
          }
        }
      }

      /*
       USE [master]
ALTER DATABASE [serverName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
BACKUP LOG [serverName] TO  DISK = N'C:\MSSQL\Backup\serverName_LogBackup_date.bak' WITH NOFORMAT, NOINIT,  NAME = N'serverName_LogBackup_date', NOSKIP, NOREWIND, NOUNLOAD,  NORECOVERY ,  STATS = 5
RESTORE DATABASE [serverName] FROM  DISK = N'C:\Path\serverName\serverName_backup_date.full' WITH  FILE = 1,  MOVE N'GestION_data' TO N'C:\MSSQL\DATA\serverName.mdf',  MOVE N'GestION_log' TO N'E:\MSSQL\DATA\serverName.ldf',  NORECOVERY,  NOUNLOAD,  REPLACE,  STATS = 5
RESTORE DATABASE [serverName] FROM  DISK = N'C:\Path\serverName\serverName_backup_date.diff' WITH  FILE = 1,  NOUNLOAD,  STATS = 5
ALTER DATABASE [serverName] SET MULTI_USER
GO
       * */

      display("Files removed, latest full and diff kept");
      display(Environment.NewLine);
      display("Press any key to exit:");
      //Console.ReadKey(); // commented because put on a scheduled task
    }

    private bool IsFileLocked(FileInfo file)
    {
      try
      {
        using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
        {
          stream.Close();
        }
      }
      catch (IOException)
      {
        //the file is unavailable because it is:
        //still being written to
        //or being processed by another thread
        //or does not exist (has already been processed)
        return true;
      }

      //file is not locked
      return false;
    }

    private static List<string> GetOldestFileNames(List<string> value)
    {
      List<string> result = new List<string>();
      List<FileName> tmpFileList = new List<FileName>();
      foreach (var item in value)
      {
        FileName tmpFile = new FileName(item);
        tmpFileList.Add(tmpFile);
      }

      var newestDate = tmpFileList.Select(f => f.DateOfFile).Max();
      foreach (var item in value)
      {
        FileName tmpFileName = new FileName(item);
        if (tmpFileName.DateOfFile != newestDate)
        {
          result.Add(tmpFileName.LongName);
        }
      }

      return result;
    }

    private static string GetOldestFileName(List<string> value)
    {
      string result = string.Empty;
      List<FileName> tmpFileList = new List<FileName>();
      foreach (var item in value)
      {
        FileName tmpFile = new FileName(item);
        tmpFileList.Add(tmpFile);
      }

      var oldestDate = tmpFileList.Select(f => f.DateOfFile).Min();
      foreach (var item in value)
      {
        FileName tmpFileName = new FileName(item);
        if (tmpFileName.DateOfFile == oldestDate)
        {
          result = tmpFileName.LongName;
          break;
        }
      }

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
