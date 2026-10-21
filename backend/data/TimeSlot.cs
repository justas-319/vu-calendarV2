readonly record struct TimeSlot(DayOfWeek Day, TimeOnly Start, TimeOnly End) : IComparable<TimeSlot>
{
    public TimeSpan Duration => End - Start;

    public bool Overlaps(TimeSlot other)
    {
        return Day == other.Day && Start < other.End && other.Start < End;
    }

    public int CompareTo(TimeSlot other)
    {
        int thisDay = ((int)Day + 6) % 7;
        int otherDay = ((int)other.Day + 6) % 7;

        int byDay = thisDay.CompareTo(otherDay);
        return byDay != 0 ? byDay : Start.CompareTo(other.Start);
    }
}
