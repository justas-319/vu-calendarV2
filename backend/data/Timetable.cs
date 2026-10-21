using System.Collections;

class Timetable : IEnumerable<Lecture>
{
    private readonly List<Lecture> _lectures = [];

    public string Owner { get; set; } = "";

    public int Count => _lectures.Count;

    public void Add(Lecture lecture)
    {
        _lectures.Add(lecture);
    }

    public bool Remove(Lecture lecture)
    {
        return _lectures.Remove(lecture);
    }

    public bool Overlaps(Lecture candidate)
    {
        return _lectures.Any(lecture => lecture.Slot.Overlaps(candidate.Slot));
    }

    // Always enumerates in chronological order, uses Lecture.IComparable
    public IEnumerator<Lecture> GetEnumerator()
    {
        foreach (Lecture lecture in _lectures.Order())
        {
            yield return lecture;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
