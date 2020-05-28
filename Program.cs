using System;
using System.IO;
using System.Text.RegularExpressions;

namespace renamer
{
  class Program
  {
    static void Main(string[] args)
    {
      // dotnet run
      string directory = @"C:\Users\mikek\OneDrive\Pictures\Blythe\New\04 - April";
      string[] files = Directory.GetFiles(directory);

      Regex ideal = new Regex(@"(?<year>\d\d\d\d)-(?<month>\d\d)-(?<day>\d\d) (?<hour>\d\d)\.(?<minute>\d\d)\.(?<second>\d\d).jpg");

      Regex[] regexes = new Regex[]{
        new Regex(@"(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)_(?<hour>\d\d)(?<minute>\d\d)(?<second>\d\d)(\(\d\))?.[jpg|jpeg]", RegexOptions.IgnoreCase),
        new Regex(@"(?<year>\d\d\d\d)\.(?<month>\d\d)\.(?<day>\d\d) (?<hour>\d\d)-(?<minute>\d\d)-(?<second>\d\d).[jpg|jpeg]", RegexOptions.IgnoreCase),
        new Regex(@"(?<year>\d\d\d\d)-(?<month>\d\d)-(?<day>\d\d) (?<hour>\d\d)\.(?<minute>\d\d)\.(?<second>\d\d)(-\d)?.[jpg|jpeg]", RegexOptions.IgnoreCase),
        new Regex(@"(?<year>\d\d\d\d)-(?<month>\d\d)-(?<day>\d\d) (?<hour>\d\d),(?<minute>\d\d),(?<second>\d\d).[jpg|jpeg]", RegexOptions.IgnoreCase),
        new Regex(@"(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)_(?<hour>\d\d)(?<minute>\d\d)(?<second>\d\d)(_\d\d\d)?.[jpg|jpeg]", RegexOptions.IgnoreCase),
        new Regex(@"(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)_(?<hour>\d\d)(?<minute>\d\d)(?<second>\d\d)(_\d)?.[jpg|jpeg]", RegexOptions.IgnoreCase),
        new Regex(@"BURST(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)(?<hour>\d\d)(?<minute>\d\d)(?<second>\d\d)(\d\d\d_COVER)?.[jpg|jpeg]", RegexOptions.IgnoreCase),
      };

      foreach (string path in files)
      {
        string[] parts = path.Split(@"\");
        string filename = parts[parts.Length - 1];

        //if it's the way i want it, then skip
        if (ideal.IsMatch(filename))
          continue;

        //find any matching regex
        Match match = null;
        foreach (Regex regex in regexes)
        {
          if (regex.IsMatch(filename))
          {
            match = regex.Match(filename);
            break;
          }
        }

        //if it's not ideal, and there's no matching regex, then log it
        if (match == null)
        {
          Console.WriteLine($"need to rename: {filename}");
          continue;
        }

        //rename the file
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
    }

  }
}