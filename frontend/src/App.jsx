import { useEffect, useState } from "react";
import Header from "./components/Header";
import Box from "./components/Box";
import Combobox from "./components/Combobox";
import Listbox from "./components/Listbox";
import Timetable from "./components/Timetable";

function App() {
  const [message, setMessage] = useState("");

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

  useEffect(() => {
    async function getMessage() {
      const response = await fetch("/api/message");
      const data = await response.text();
      setMessage(data);
    }

    getMessage();
  }, []);

  return (
    <div className="container-fluid p-3">
      {/* 
      top: last updated - use it to view when the timetable was updated.
      will have a button to update it 
      */}
      <Header />

      <div className="row gy-3 py-3">
        {/* top left: course selector */}
        <div className="col-md-3">
          <Box>
            <h4>Study selection</h4>

            <div className="py-2">
              <Combobox
                label="Faculty"
                placeholder="Search faculty..."
                items={faculties}
                onSelect={setFaculty}
              />

              <Combobox
                label="Course"
                placeholder="Search course..."
                items={courses}
                onSelect={setCourse}
              />

              {/* makes year and group be next to each other */}
              <div className="row">
                <div className="col-6">
                  <Listbox
                    label="Year"
                    items={years}
                    value={year}
                    onChange={setYear}
                    placeholder="Select year..."
                  />
                </div>

                <div className="col-6">
                  <Listbox
                    label="Group"
                    items={groups}
                    value={group}
                    onChange={setGroup}
                    placeholder="Select group..."
                  />
                </div>
              </div>
            </div>
          </Box>

          {/* bottom left: module selector */}
          <div className="mt-3">
            <Box>
              <h4>Modules</h4>
            </Box>
          </div>
        </div>

        {/* right: timetable */}
        <div className="col-md-9">
          <Timetable message={message} />
        </div>
      </div>

      {/* bottom: selected modules - view your modules and remove them */}
      <div className="card">
        <div className="card-body py-2">Selected modules:</div>
      </div>
    </div>
  );
}

export default App;
