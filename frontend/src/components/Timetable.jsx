import { useState } from "react";
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

function overlaps(a, b) {
  return (
    a.day === b.day &&
    timeToMinutes(a.start) < timeToMinutes(b.end) &&
    timeToMinutes(b.start) < timeToMinutes(a.end)
  );
}

function getModuleOverlaps(moduleLecture, lectures, moduleLectures) {
  const allLectures = lectures.concat(moduleLectures);
  const overlapping = [];

  for (const current of allLectures) {
    if (current === moduleLecture) {
      continue;
    }

    if (overlaps(moduleLecture, current)) {
      overlapping.push(current);
    }
  }

  return overlapping;
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

function Timetable({ lectures, moduleLectures }) {
  const [openDay, setOpenDay] = useState(null);

  return (
    <div className="card h-100">
      <div className="card-body">
        <div className="calendar-header">
          <div>Time</div>

          {days.map((day) => {
            const overlappingModules = [];

            for (const moduleLecture of moduleLectures) {
              if (
                moduleLecture.day === day &&
                getModuleOverlaps(moduleLecture, lectures, moduleLectures)
                  .length > 0
              ) {
                overlappingModules.push(moduleLecture);
              }
            }

            const count = overlappingModules.length;

            let label = day;
            let className = "";

            if (count > 0) {
              label = `${day} \u00B7 ${count} overlaps`;
              className =
                "border border-danger bg-danger-subtle text-danger rounded px-2 py-1";
            }

            if (count === 0) {
              return <div key={day}>{label}</div>;
            }

            return (
              <div key={day} className="position-relative">
                <button
                  type="button"
                  className="overlap-button"
                  onClick={() => setOpenDay(day)}
                >
                  <span className={className}>{label}</span>
                </button>

                {openDay === day && (
                  <div className="overlap-popup border rounded p-3 bg-body small fw-normal text-start">
                    <button
                      type="button"
                      className="btn-close float-end"
                      onClick={() => setOpenDay(null)}
                    />

                    {overlappingModules.map((moduleLecture, index) => (
                      <div key={index}>
                        <div className="small">
                          {moduleLecture.start}–{moduleLecture.end}
                        </div>
                        <div className="fw-bold">{moduleLecture.module}</div>
                        <div className="text-muted small">
                          {moduleLecture.room || "Room not provided"}
                        </div>

                        {index < overlappingModules.length - 1 && (
                          <hr className="my-2" />
                        )}
                      </div>
                    ))}
                  </div>
                )}
              </div>
            );
          })}
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
                    className={`calendar-lecture border p-2 small ${
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
                    <div className="small">
                      {lecture.start}–{lecture.end}
                    </div>
                    <div className="fw-bold">{lecture.module}</div>
                    <div className="text-muted small">
                      {lecture.room || "Room not provided"}
                    </div>
                  </div>
                ))}

              {moduleLectures
                .filter((moduleLecture) => moduleLecture.day === day)
                .filter(
                  (moduleLecture) =>
                    getModuleOverlaps(moduleLecture, lectures, moduleLectures)
                      .length === 0,
                )
                .map((moduleLecture, index) => (
                  <div
                    key={index}
                    className="calendar-lecture module-lecture border p-2 small"
                    style={{
                      top: timeToRow(moduleLecture.start) * SLOT_HEIGHT,
                      height:
                        (timeToRow(moduleLecture.end) -
                          timeToRow(moduleLecture.start)) *
                        SLOT_HEIGHT,
                    }}
                  >
                    <div className="small">
                      {moduleLecture.start}–{moduleLecture.end}
                    </div>
                    <div className="fw-bold">{moduleLecture.module}</div>
                    <div className="text-muted small">
                      {moduleLecture.room || "Room not provided"}
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
