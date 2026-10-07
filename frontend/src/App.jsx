import { useEffect, useState } from "react";
import Header from "./components/Header";
import Box from "./components/Box";
import Combobox from "./components/Combobox";
import Listbox from "./components/Listbox";
import Timetable from "./components/Timetable";

const faculties = [
  { id: 1, name: "Mathematics" },
  { id: 2, name: "Physics" },
  { id: 3, name: "Computer Science" },
];

const courses = [
  { id: 1, name: "Computer Science" },
  { id: 2, name: "Data Science" },
  { id: 3, name: "Physics" },
];

const years = [
  { id: 1, name: "1 year" },
  { id: 2, name: "2 year" },
  { id: 3, name: "3 year" },
  { id: 4, name: "4 year" },
];

const groups = [
  { id: 1, name: "Group 1" },
  { id: 2, name: "Group 2" },
  { id: 3, name: "Group 3" },
  { id: 4, name: "Group 4" },
  { id: 5, name: "Group 5" },
];

function App() {
  const [message, setMessage] = useState("");
  const [year, setYear] = useState(null);
  const [group, setGroup] = useState(null);

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
              />

              <Combobox
                label="Course"
                placeholder="Search course..."
                items={courses}
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
