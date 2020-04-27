using System;

namespace RestoreDatabase
{
  class FileName
  {
    public string Name { get; set; }
    public string Extension { get; set; }
    public DateTime DateOfFile { get; set; }

    public FileName()
    {
      Name = string.Empty;
      Extension = string.Empty;
    }

    public FileName(string name, string extension, DateTime date)
    {
      Name = name;
      Extension = extension;
      DateOfFile = date;
    }



  }
}
