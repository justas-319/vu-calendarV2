import Header from "./components/Header";
import Box from "./components/Box";
import Combobox from "./components/Combobox";
import Listbox from "./components/Listbox";
import Timetable from "./components/Timetable";
import useStudySelector from "./hooks/useStudySelector";
import useModuleSelector from "./hooks/useModuleSelector";

function App() {
  const {
    faculties,
    setSelectedFaculty,
    studyTypes,
    selectedStudyType,
    setSelectedStudyType,
    courses,
    setSelectedCourse,
    years,
    selectedYear,
    setSelectedYear,
    groups,
    selectedGroup,
    setSelectedGroup,
    lectures,
  } = useStudySelector();

  const {
    moduleFaculties,
    selectedModuleFaculty,
    setSelectedModuleFaculty,
    modules,
    selectedModule,
    setSelectedModule,
    selectedModulesArray,
    addModule,
    removeModule,
  } = useModuleSelector();

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
                onSelect={setSelectedFaculty}
              />

              <Listbox
                label="Study type"
                items={studyTypes}
                value={selectedStudyType}
                onChange={setSelectedStudyType}
                placeholder="Select study type..."
              />

              <Combobox
                label="Course"
                placeholder="Search course..."
                items={courses}
                onSelect={setSelectedCourse}
              />

              {/* makes year and group be next to each other */}
              <div className="row">
                <div className="col-6">
                  <Listbox
                    label="Year"
                    items={years}
                    value={selectedYear}
                    onChange={setSelectedYear}
                    placeholder="Select year..."
                  />
                </div>

                <div className="col-6">
                  <Listbox
                    label="Group"
                    items={groups}
                    value={selectedGroup}
                    onChange={setSelectedGroup}
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
              <div className="py-2">
                <Combobox
                  label="Faculty"
                  placeholder="Search faculty..."
                  items={moduleFaculties}
                  onSelect={setSelectedModuleFaculty}
                />

                <Combobox
                  label="Module"
                  placeholder="Search module..."
                  items={modules}
                  onSelect={setSelectedModule}
                />

                <button
                  type="button"
                  className="btn btn-primary"
                  onClick={addModule}
                  disabled={!selectedModuleFaculty || !selectedModule}
                >
                  Add module
                </button>
              </div>
            </Box>
          </div>
        </div>

        {/* right: timetable */}
        <div className="col-md-9">
          <Timetable lectures={lectures} />
        </div>
      </div>

      {/* bottom: selected modules - view your modules and remove them */}
      <div className="card">
        <div className="card-body py-2 d-flex align-items-center gap-2">
          Selected modules:
          {selectedModulesArray.map((module, index) => (
            <div
              key={index}
              className="border rounded px-2 d-flex align-items-center gap-2"
            >
              {module.name}

              <button
                type="button"
                className="btn text-danger p-0 border-0"
                onClick={() => removeModule(index)}
              >
                X
              </button>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

export default App;
