function Timetable({ message }) {
  return (
    <div className="position-fixed bottom-0 start-0 w-100 timetable bg-light p-3 overflow-auto">
      <h2>Timetable</h2>
      <p>{message}</p>
    </div>
  );
}

export default Timetable;
