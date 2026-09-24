const response = await fetch("/api/message");
const message = await response.text();

function App() {
  return (
    <div className="container-fluid p-3">
      <div className="card">
        <div className="card-body py-2">
          Last updated:
        </div>
      </div>

      <div className="row gy-3 py-3">
        <div className="col-md-3">
          <div className="card mb-3">
            <div className="card-body">
              <h3>Main course</h3>
            </div>
          </div>

          <div className="card">
            <div className="card-body">
              <h3>Modules</h3>
            </div>
          </div>
        </div>

        <div className="col-md-9">
          <div className="card h-100">
            <div className="card-body">
              <h3>Timetable</h3>
              {message}
            </div>
          </div>
        </div>
      </div>

      <div className="card">
        <div className="card-body py-2">
          Selected modules:
        </div>
      </div>
    </div>
  );
}

export default App;
