static class LectureExtension
{
    public static bool HasClash(this IEnumerable<Lecture> lectures, Lecture lecture)
    {
        return lectures.Any(other => other != lecture && other.Slot.Overlaps(lecture.Slot));
    }
}
