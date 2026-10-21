import { useEffect, useState } from "react";

function useStudySelector() {
  // list of choices
  const [faculties, setFaculties] = useState([]);
  // user selected
  const [selectedFaculty, setSelectedFaculty] = useState(null);

  const [studyTypes, setStudyTypes] = useState([]);

  const [selectedStudyType, setSelectedStudyType] = useState(null);

  const [courses, setCourses] = useState([]);
  const [selectedCourse, setSelectedCourse] = useState(null);

  const [years, setYears] = useState([]);
  const [selectedYear, setSelectedYear] = useState(null);

  const [groups, setGroups] = useState([]);
  const [selectedGroup, setSelectedGroup] = useState(null);

  const [lectures, setLectures] = useState([]);

  // fetch faculty options one time, when the page first loads
  useEffect(() => {
    async function getFaculties() {
      const response = await fetch("/api/departments");
      const data = await response.json();

      // use abbreviation as id for combobox items
      setFaculties(
        data.map((department) => ({
          id: department.abbreviation,
          name: department.name,
          abbreviation: department.abbreviation,
        })),
      );
    }

    getFaculties();
  }, []);

  // fetch study type options one time, when the page first loads
  useEffect(() => {
    async function getStudyTypes() {
      const response = await fetch("/api/study-types");
      const data = await response.json();

      setStudyTypes(data);
    }

    getStudyTypes();
  }, []);

  // fetches courses when the user selects a faculty
  useEffect(() => {
    async function getCourses() {
      const response = await fetch(
        `/api/programs/${selectedFaculty.abbreviation}`,
      );
      const data = await response.json();

      setCourses(
        data.map((course) => ({
          id: course,
          name: course,
        })),
      );
    }

    // so it doesnt request for /api/programs/null
    if (selectedFaculty !== null) {
      getCourses();
    }
  }, [selectedFaculty]);

  // fetches years when the user selects a course
  useEffect(() => {
    async function getYears() {
      const response = await fetch(
        `/api/courses/${selectedFaculty.abbreviation}/${selectedCourse.name}`,
      );
      const data = await response.json();

      setYears(
        data.map((year) => ({
          id: year,
          name: `${year}`,
        })),
      );
    }

    // we need both faculty and course to display years
    if (selectedFaculty !== null && selectedCourse !== null) {
      getYears();
    }
  }, [selectedFaculty, selectedCourse]);

  // fetches groups when the user selects a year
  useEffect(() => {
    async function getGroups() {
      // this only returns the first number instead of "1 Kursas", gives "1"
      const courseNumber = selectedYear.id.split(" ")[0];

      const response = await fetch(
        `/api/groups/${selectedFaculty.abbreviation}/${selectedCourse.name}/${courseNumber}`,
      );

      const data = await response.json();

      setGroups(
        data.map((group) => ({
          id: group.url,
          // "Bakalauro nuolatine, Programu sistemos 1 Kursas 2 Grupe" -> "2 Grupe"
          name: group.name.split(" ").slice(-2).join(" "),
        })),
      );
    }

    if (
      selectedFaculty !== null &&
      selectedCourse !== null &&
      selectedYear !== null
    ) {
      getGroups();
    }
  }, [selectedFaculty, selectedCourse, selectedYear]);

  // fetches the timetable when the user selects a group
  useEffect(() => {
    async function getLectures() {
      const params = new URLSearchParams({
        department: selectedFaculty.abbreviation,
        program: selectedCourse.name,
        course: selectedYear.id.split(" ")[0],
        group: selectedGroup.name,
      });

      const response = await fetch(`/api/timetable?${params}`);
      setLectures(await response.json());
    }

    if (
      selectedFaculty !== null &&
      selectedCourse !== null &&
      selectedYear !== null &&
      selectedGroup !== null
    ) {
      getLectures();
    } else {
      setLectures([]);
    }
  }, [selectedFaculty, selectedCourse, selectedYear, selectedGroup]);

  /*
  useEffect(() => {
  async function getLectures() {
    const response = await fetch(
      `/api/timetable?groupUrl=${encodeURIComponent(selectedGroup.id)}`,
    );
    setLectures(await response.json());
  }

  if (selectedGroup !== null) {
    getLectures();
  }
}, [selectedGroup]);
*/

  // give App access to the states and setters
  return {
    faculties,
    selectedFaculty,
    setSelectedFaculty,
    studyTypes,
    selectedStudyType,
    setSelectedStudyType,
    courses,
    selectedCourse,
    setSelectedCourse,
    years,
    selectedYear,
    setSelectedYear,
    groups,
    selectedGroup,
    setSelectedGroup,
    lectures,
  };
}

export default useStudySelector;
