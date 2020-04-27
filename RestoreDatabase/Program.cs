using System;
using System.IO;

namespace RestoreDatabase
{
  class Program
  {
    static void Main()
    {
      Action<string> display = Console.WriteLine;
      // check if several full backup files
      // if so delete oldest
      //check if diff files older than full
      // if so delete them
      // generate restore SQL script file
      string initialDirectory = @"E:\Partage\DepotTMA\ApplicationName_C_26\";
      string pattern = "ApplicationName_*.*";
      bool hasSeveralFull = false;
      var files = Directory.GetFiles(initialDirectory, pattern);
      foreach (var fileName in files)
      {
        display(GetDateFromFileName(fileName).ToLongDateString());
      }

      display("Press any key to exit:");
      Console.ReadKey();

    }

    public static DateTime GetDateFromFileName(string fileName)
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