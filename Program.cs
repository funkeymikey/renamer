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
      string directory = @"C:\Users\mike\OneDrive\Pictures\Children\New";
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

      Regex[] utcRegexes = new Regex[]{
        new Regex(@"PXL_(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)_(?<hour>\d\d)(?<minute>\d\d)(?<second>\d\d)(\d\d\d)?.[jpg|jpeg]", RegexOptions.IgnoreCase),
        new Regex(@"PXL_(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)_(?<hour>\d\d)(?<minute>\d\d)(?<second>\d\d)(\d\d\d)?.NIGHT?.[jpg|jpeg]", RegexOptions.IgnoreCase),
      };

      foreach (string path in files)
      {
        string[] parts = path.Split(@"\");
        string filename = parts[parts.Length - 1];

        //if it's the way i want it, then skip
        if (ideal.IsMatch(filename))
          continue;

        
        bool isUtc = false;
        Match match = null;

        //find if any localtime regexes match
        foreach (Regex regex in regexes)
        {
          if (regex.IsMatch(filename))
          {
            match = regex.Match(filename);
            break;
          }
        }

        //if none of the local regexes matched, check the utc regexes
        if (match == null)
        {
          foreach (Regex regex in utcRegexes)
          {
            if (regex.IsMatch(filename))
            {
              match = regex.Match(filename);
              isUtc = true;
              break;
            }
          }
        }

        //if it's not ideal, and there's no matching regex, then log it
        if(match == null)
        {
          Console.WriteLine($"need to rename: {filename}");
          continue;
        }

        //find the new name for the file
        string newName;
        if(isUtc)
        {
          int yearUtc = int.Parse(match.Groups["year"].Value);
          int monthUtc = int.Parse(match.Groups["month"].Value);
          int dayUtc = int.Parse(match.Groups["day"].Value);
          int hourUtc = int.Parse(match.Groups["hour"].Value);
          int minuteUtc = int.Parse(match.Groups["minute"].Value);
          int secondUtc = int.Parse(match.Groups["second"].Value);

          DateTime utcDate = new DateTime(yearUtc, monthUtc, dayUtc, hourUtc, minuteUtc, secondUtc, DateTimeKind.Utc);
          DateTime localDate = utcDate.ToLocalTime();
          newName = $"{localDate.Year.ToString().PadLeft(4, '0')}-{localDate.Month.ToString().PadLeft(2, '0')}-{localDate.Day.ToString().PadLeft(2, '0')} {localDate.Hour.ToString().PadLeft(2, '0')}.{localDate.Minute.ToString().PadLeft(2, '0')}.{localDate.Second.ToString().PadLeft(2, '0')}.jpg";
        }
        else
        {
          string year = match.Groups["year"].Value;
          string month = match.Groups["month"].Value;
          string day = match.Groups["day"].Value;
          string hour = match.Groups["hour"].Value;
          string minute = match.Groups["minute"].Value;
          string second = match.Groups["second"].Value;
          newName = $"{year}-{month}-{day} {hour}.{minute}.{second}.jpg";
        }
        
        //perform the rename
        Console.WriteLine($"renaming {filename} to {newName}");
        string newPath = path.Replace(filename, newName);
        File.Move(path, newPath);
      }
    }

  }
}