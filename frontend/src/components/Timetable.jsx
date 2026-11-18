const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];

const times = [];
// creates an array from 7-22 with 30 min intervals
for (let hour = 7; hour < 22; hour++) {
  times.push(`${hour}:00`);
  times.push(`${hour}:30`);
}

function timeToMinutes(time) {
  const [hours, minutes] = time.split(":");
  const mins = Number(hours) * 60 + Number(minutes);
  
  return mins;
}

function timeToRow(time) {
  const startTimeInMinutes = 420; // 07 hour
  const interval = 30;

  const minutes = timeToMinutes(time);
  const minutesPassed = minutes - startTimeInMinutes;
  const slotsPassed = minutesPassed / interval;

  // CSS Grid lines start at 1
  const row = slotsPassed + 1;

  return row;
}

function Timetable({ lectures }) {
  return (
    <div className="card h-100">
      <div className="card-body">
        <div className="calendar-header">
          <div>Time</div>

          {days.map((day) => (
            <div key={day}>{day}</div>
          ))}
        </div>

        <div className="calendar-body">
          <div>
            {times.map((time) => (
              <div key={time} className="time-slot">
                {time.endsWith(":00") && time}
              </div>
            ))}
          </div>

          {days.map((day) => (
            <div key={day} className="calendar-day">
              {lectures
                .filter((lecture) => lecture.day === day)
                .map((lecture, index) => (
                  <div
                    key={index}
                    className={`calendar-lecture border rounded p-2 mb-2 small ${
                      lecture.clashes ? "border-danger bg-danger-subtle" : ""
                    }`}
                    style={{
                      gridRow: `${timeToRow(lecture.start)} / ${timeToRow(lecture.end)}`,
                    }}
                  >
                    <div className="fw-bold">
                      {lecture.start}–{lecture.end}
                    </div>
                    <div>{lecture.module}</div>
                    <div className="text-muted">
                      {lecture.room || "Room not provided"}
                    </div>
                  </div>
                ))}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

export default Timetable;
