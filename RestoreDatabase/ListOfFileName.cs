using System.Collections.Generic;

namespace RestoreDatabase
{
  class ListOfFileName
  {
    public List<FileName> ListOfFiles { get; set; }
    public List<FileName> ListOfFull { get; set; }
    public List<FileName> ListOfDiff { get; set; }
    public bool HasSeveralFull { get; set; }

    public ListOfFileName()
    {
      ListOfFiles = new List<FileName>();
      ListOfFull = new List<FileName>();
      ListOfDiff = new List<FileName>();
      HasSeveralFull = false;
    }
  }
}
