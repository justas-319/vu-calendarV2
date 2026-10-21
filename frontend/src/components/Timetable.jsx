function Timetable({ lectures }) {
  return (
    <div className="card h-100">
      <div className="card-body">
        <h4>Timetable</h4>
        <ul className="list-group">
          {lectures.map((lecture) => (
            <li
              key={`${lecture.day}-${lecture.start}-${lecture.module}`}
              className={`list-group-item ${lecture.clashes ? "list-group-item-danger" : ""}`}
            >
              {lecture.day} {lecture.start}–{lecture.end}: {lecture.module} ({lecture.room})
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}

export default Timetable;
