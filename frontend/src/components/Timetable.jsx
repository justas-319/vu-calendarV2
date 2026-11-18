const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];

// how many pixels each 30min interval is
// if you change this you also have to change styles.css .time-slot height
const SLOT_HEIGHT = 27;

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

  return slotsPassed;
}

function getHorizontalPosition(lecture, lectures) {
  // currently it only finds lectures that overlap with the exact same time and day
  const matching = lectures.filter(
    (other) =>
      other.day === lecture.day &&
      other.start === lecture.start &&
      other.end === lecture.end,
  );

  const width = 100 / matching.length;
  const column = matching.indexOf(lecture);

  return {
    left: `${column * width}%`,
    width: `${width}%`,
  };
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
                    className={`calendar-lecture border rounded p-2 small ${
                      lecture.clashes ? "border-danger bg-danger-subtle" : ""
                    }`}
                    style={{
                      top: timeToRow(lecture.start) * SLOT_HEIGHT,
                      height:
                        (timeToRow(lecture.end) - timeToRow(lecture.start)) *
                        SLOT_HEIGHT,
                      ...getHorizontalPosition(lecture, lectures),
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
