const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];

function Timetable({ lectures }) {
  return (
    <div className="card h-100">
      <div className="card-body text-center">
        <h4>Timetable</h4>

        <div className="row row-cols-5 g-2">
          {days.map((day) => (
            <div className="col" key={day}>
              <h6 className="text-center border-bottom pb-2">{day}</h6>

              {lectures
                .filter((lecture) => lecture.day === day)
                .map((lecture) => (
                  <div
                    key={`${lecture.start}-${lecture.module}`}
                    className={`border rounded p-2 mb-2 small ${
                      lecture.clashes ? "border-danger bg-danger-subtle" : ""
                    }`}
                  >
                    <div className="fw-bold">
                      {lecture.start}–{lecture.end}
                    </div>
                    <div>{lecture.module}</div>
                    <div className="text-muted">{lecture.room}</div>
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