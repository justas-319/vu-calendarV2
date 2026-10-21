using System.Text.RegularExpressions;

static class DemoTimetables
{
    private static Lecture Make(string module, string room, DayOfWeek day, int startHour, int startMinute, int endHour, int endMinute)
    {
        return new Lecture(
            module,
            room,
            new TimeSlot(day, new TimeOnly(startHour, startMinute), new TimeOnly(endHour, endMinute))
        );
    }

    // Lectures that every group of the year attends
    private static readonly Lecture[] SharedLectures =
    [
        Make("Object-Oriented Programming (lecture)", "Room 101", DayOfWeek.Monday, 8, 0, 9, 30),
        Make("Databases (lecture)", "Room 202", DayOfWeek.Wednesday, 10, 0, 11, 30),
        Make("Discrete Mathematics (lecture)", "Room 305", DayOfWeek.Friday, 12, 0, 13, 30)
    ];

    // Practicals, different for every group
    private static readonly Dictionary<int, Lecture[]> GroupLectures = new()
    {
        [1] =
        [
            Make("OOP practical", "Lab 1", DayOfWeek.Tuesday, 8, 0, 9, 30),
            Make("Databases practical", "Lab 2", DayOfWeek.Thursday, 10, 0, 11, 30)
        ],
        [2] =
        [
            Make("OOP practical", "Lab 1", DayOfWeek.Tuesday, 10, 0, 11, 30),
            Make("Databases practical", "Lab 2", DayOfWeek.Thursday, 12, 0, 13, 30)
        ],
        [3] =
        [
            // deliberately overlaps the Monday OOP lecture to show a clash
            Make("OOP practical", "Lab 1", DayOfWeek.Monday, 9, 0, 10, 30),
            Make("Databases practical", "Lab 2", DayOfWeek.Thursday, 14, 0, 15, 30)
        ],
        [4] =
        [
            Make("OOP practical", "Lab 1", DayOfWeek.Wednesday, 14, 0, 15, 30),
            Make("Databases practical", "Lab 2", DayOfWeek.Friday, 8, 0, 9, 30)
        ]
    };

    // "Programų sistemos" -> "programu sistemos" so diacritics don't matter
    private static string Normalize(string text)
    {
        string decomposed = text.Normalize(System.Text.NormalizationForm.FormD);
        string withoutMarks = new string(
            decomposed.Where(c => char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray()
        );
        return withoutMarks.ToLowerInvariant();
    }

    public static bool Matches(string department, string program, string course)
    {
        return department.Equals("mif", StringComparison.OrdinalIgnoreCase)
            && Normalize(program).Contains("programu sistemos")
            && course == "2";
    }

    // group arrives as e.g. "3 Grupe" -> 3
    public static IEnumerable<Lecture> ForGroup(string group)
    {
        Match match = Regex.Match(group, @"\d+");

        if (!match.Success || !GroupLectures.TryGetValue(int.Parse(match.Value), out Lecture[]? practicals))
        {
            return [];
        }

        return SharedLectures.Concat(practicals);
    }
}
