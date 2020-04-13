using System;
using System.IO;
using System.Text.RegularExpressions;

namespace renamer
{
  class Program
  {
    static void Main(string[] args)
    {
      string directory = @"C:\Users\mike\OneDrive\Pictures\Blythe\New\11- November";
      string[] files = Directory.GetFiles(directory);

      Regex ideal = new Regex(@"(?<year>\d\d\d\d)-(?<month>\d\d)-(?<day>\d\d) (?<hour>\d\d)\.(?<minute>\d\d)\.(?<second>\d\d).jpg");
      Regex one = new Regex(@"(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)_(?<hour>\d\d)(?<minute>\d\d)(?<second>\d\d)(\(\d\))?.[jpg|jpeg]", RegexOptions.IgnoreCase);
      Regex two = new Regex(@"(?<year>\d\d\d\d)\.(?<month>\d\d)\.(?<day>\d\d) (?<hour>\d\d)-(?<minute>\d\d)-(?<second>\d\d).[jpg|jpeg]", RegexOptions.IgnoreCase);

      foreach (string path in files)
      {
        string[] parts = path.Split(@"\");
        string filename = parts[parts.Length - 1];

        if (ideal.IsMatch(filename))
          continue;

        Match match = null;
        if (one.IsMatch(filename))
          match = one.Match(filename);
        if (two.IsMatch(filename))
          match = two.Match(filename);


        if (match != null)
        {
          string year = match.Groups["year"].Value;
          string month = match.Groups["month"].Value;
          string day = match.Groups["day"].Value;
          string hour = match.Groups["hour"].Value;
          string minute = match.Groups["minute"].Value;
          string second = match.Groups["second"].Value;

          string newName = $"{year}-{month}-{day} {hour}.{minute}.{second}.jpg";
          string newPath = path.Replace(filename, newName);
          Console.WriteLine($"renaming {filename} to {newName}");
          File.Move(path, newPath);
        }
        else
        {
          Console.WriteLine($"need to rename: {filename}");
        }
      }
    }
  }
}