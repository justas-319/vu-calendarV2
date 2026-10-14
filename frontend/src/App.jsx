import { useEffect, useState } from "react";
import Header from "./components/Header";
import Box from "./components/Box";
import Combobox from "./components/Combobox";
import Listbox from "./components/Listbox";
import Timetable from "./components/Timetable";
import useStudySelector from "./hooks/useStudySelector";

function App() {
  const {
    faculties,
    setFaculty,
    courses,
    setCourse,
    years,
    year,
    setYear,
    groups,
    group,
    setGroup,
  } = useStudySelector();

  const [message, setMessage] = useState("");

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
