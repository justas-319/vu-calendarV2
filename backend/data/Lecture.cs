record Lecture(string Module, string Room, TimeSlot Slot) : IComparable<Lecture>
{
    public int CompareTo(Lecture? other)
    {
        return other is null ? 1 : Slot.CompareTo(other.Slot);
    }
}
