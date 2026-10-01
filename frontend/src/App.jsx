import {
  Combobox,
  ComboboxInput,
  ComboboxOption,
  ComboboxOptions,
} from "@headlessui/react";
import { useState } from "react";

const response = await fetch("/api/message");
const message = await response.text();

const courses = [
  { id: 1, name: "test1" },
  { id: 2, name: "test2" },
  { id: 3, name: "test3" },
  { id: 4, name: "test4" },
  { id: 5, name: "test5" },
];

function App() {
  const [selectedCourse, setSelectedCourse] = useState(null);
  const [query, setQuery] = useState("");

  const filteredCourses =
    query === ""
      ? courses
      : courses.filter((course) => {
          return course.name.toLowerCase().includes(query.toLowerCase());
        });

  return (
    <div className="container-fluid p-3">
      <div className="card">
        <div className="card-body py-2">Last updated:</div>
      </div>

      <div className="row gy-3 py-3">
        <div className="col-md-3">
          <div className="card mb-3">
            <div className="card-body">
              <h3>Main course</h3>
              <Combobox
                immediate
                value={selectedCourse}
                onChange={setSelectedCourse}
                onClose={() => setQuery("")}
              >
                <ComboboxInput
                  className="form-control"
                  placeholder="Search course..."
                  aria-label="Course"
                  autoComplete="off"
                  displayValue={(course) => course?.name}
                  onChange={(event) => setQuery(event.target.value)}
                />

                <ComboboxOptions
                  anchor="bottom start"
                  className="list-group"
                  style={{
                    width: "var(--input-width)",
                    maxHeight: "200px",
                    overflowY: "auto",
                  }}
                >
                  {filteredCourses.map((course) => (
                    <ComboboxOption
                      key={course.id}
                      value={course}
                      className="list-group-item list-group-item-action"
                    >
                      {course.name}
                    </ComboboxOption>
                  ))}
                </ComboboxOptions>
              </Combobox>
            </div>
          </div>

          <div className="card">
            <div className="card-body">
              <h3>Modules</h3>
            </div>
          </div>
        </div>

        <div className="col-md-9">
          <div className="card h-100">
            <div className="card-body">
              <h3>Timetable</h3>
              {message}
            </div>
          </div>
        </div>
      </div>

      <div className="card">
        <div className="card-body py-2">Selected modules:</div>
      </div>
    </div>
  );
}

export default App;
