interface IVuApi
{
    Task<List<Department>> GetDepartments();

    Task<List<StudyType>> GetStudyTypes(string department);

    Task<string> GetPrograms(string department, int studyTypeId);

    Task<string> GetCourses(string department, int studyTypeId, string programName);

    Task<string> GetGroups(string department, int studyTypeId, string programName, string course);

    Task<List<Lecture>> GetLectures(string department, string group);
}
