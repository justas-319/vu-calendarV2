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
      <Header />
      <div className="selectors">
        <Box>
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

            <Listbox
              label="Year"
              items={years}
              value={year}
              onChange={setYear}
              placeholder="Select year..."
            />

            <Listbox
              label="Group"
              items={groups}
              value={group}
              onChange={setGroup}
              placeholder="Select group..."
            />
          </div>
        </Box>
      </div>

      <Timetable message={message} />

      <Box>
        <h3>Selected modules:</h3>
      </Box>
    </div>
  );
}

export default App;
