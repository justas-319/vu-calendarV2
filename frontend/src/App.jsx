import { useEffect, useState } from "react";
import Header from "./components/Header";
import Box from "./components/Box";
import Combobox from "./components/Combobox";
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

const groups = [
  { id: 1, name: "Group 1" },
  { id: 2, name: "Group 2" },
  { id: 3, name: "Group 3" },
  { id: 4, name: "Group 4" },
  { id: 5, name: "Group 5" },
];

function App() {
  const [message, setMessage] = useState("");
  const [year, setYear] = useState("1");

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
      <Header />

      <div className="row g-3 py-3">
        <div className="col-3">
          <Combobox
            label="Faculty"
            placeholder="Search faculty..."
            items={faculties}
          />
        </div>

        <div className="col-3">
          <Combobox
            label="Course"
            placeholder="Search course..."
            items={courses}
          />
        </div>

        <div className="card col-3 align-self-start">
          <div className="card-body">
            <h3>Year</h3>

            <select
              id="year"
              className="form-select"
              value={year}
              onChange={(event) => setYear(event.target.value)}
            >
              <option value="1">1 year</option>
              <option value="2">2 year</option>
              <option value="3">3 year</option>
              <option value="4">4 year</option>
            </select>
          </div>
        </div>

        <div className="col-3">
          <Combobox
            label="Group"
            placeholder="Search group..."
            items={groups}
          />
        </div>
      </div>

      <Timetable message={message} />

      <Box>
        <h3>Selected modules:</h3>
      </Box>
    </div>
  );
}

export default App;
