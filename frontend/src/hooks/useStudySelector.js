import { useEffect, useState } from "react";

function useStudySelector() {
  // list of choices
  const [faculties, setFaculties] = useState([]);
  // user selected
  const [faculty, setFaculty] = useState(null);

  const [courses, setCourses] = useState([]);
  const [course, setCourse] = useState(null);

  const [years, setYears] = useState([]);
  const [year, setYear] = useState(null);

  const [groups, setGroups] = useState([]);
  const [group, setGroup] = useState(null);

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

  // fetches courses when the user selects a faculty
  useEffect(() => {
    async function getCourses() {
      const response = await fetch(`/api/programs/${faculty.abbreviation}`);
      const data = await response.json();

      setCourses(
        data.map((course) => ({
          id: course,
          name: course,
        })),
      );
    }

    // so it doesnt request for /api/programs/null
    if (faculty !== null) {
      getCourses();
    }
  }, [faculty]);

  // fetches years when the user selects a course
  useEffect(() => {
    async function getYears() {
      const response = await fetch(
        `/api/courses/${faculty.abbreviation}/${course.name}`,
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
    if (faculty !== null && course !== null) {
      getYears();
    }
  }, [faculty, course]);

  // fetches groups when the user selects a year
  useEffect(() => {
    async function getGroups() {
      // this only returns the first number instead of "1 Kursas", gives "1"
      const courseNumber = year.id.split(" ")[0];

      const response = await fetch(
        `/api/groups/${faculty.abbreviation}/${course.name}/${courseNumber}`,
      );

      const data = await response.json();

      setGroups(
        data.map((group) => {
          // takes the last 2 elements
          // so "Bakalauro nuolatine, Programu sistemos 1 Kursas 2 Grupe" turns into "2 Grupe"
          const groupName = group.split(" ").slice(-2).join(" ");

          return {
            id: groupName,
            name: groupName,
          };
        }),
      );
    }

    if (faculty !== null && course !== null && year !== null) {
      getGroups();
    }
  }, [faculty, course, year]);

  // give App access to the states and setters
  return {
    faculties,
    faculty,
    setFaculty,
    courses,
    course,
    setCourse,
    years,
    year,
    setYear,
    groups,
    group,
    setGroup,
  };
}

export default useStudySelector;
