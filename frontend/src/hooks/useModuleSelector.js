import { useEffect, useState } from "react";

const moduleGroups = {
  mif: "individualiuju-studiju-dalykas-modulis-2026-2",
  evaf: "individualios-studijos-kitoms-kryptims-2026",
  chgf: "individualiosios-studijos-2026-3",
};

function useModuleSelector() {
  const [moduleFaculties, setModuleFaculties] = useState([]);
  const [selectedModuleFaculty, setSelectedModuleFaculty] = useState(null);
  const [modules, setModules] = useState([]);
  const [selectedModule, setSelectedModule] = useState(null);

  // array of modules the user has added
  const [selectedModulesArray, setSelectedModulesArray] = useState([]);

  // fetch faculty options one time, when the page first loads
  useEffect(() => {
    async function getModuleFaculties() {
      const response = await fetch("/api/departments");
      const data = await response.json();

      // use abbreviation as id for combobox items
      setModuleFaculties(
        data.map((department) => ({
          id: department.abbreviation,
          name: department.name,
          abbreviation: department.abbreviation,
        })),
      );
    }

    getModuleFaculties();
  }, []);

  // fetch the modules when the user selects a faculty
  useEffect(() => {
    async function getModules() {
      const department = selectedModuleFaculty.abbreviation;
      const response = await fetch(
        `/api/modules/${department}/${moduleGroups[department]}`,
      );
      setModules(await response.json());
    }

    setSelectedModule(null);
    setModules([]);

    if (
      selectedModuleFaculty !== null &&
      moduleGroups[selectedModuleFaculty.abbreviation]
    ) {
      getModules();
    }
  }, [selectedModuleFaculty]);

  async function addModule() {
    if (!selectedModule || !selectedModuleFaculty) {
      return;
    }

    const department = selectedModuleFaculty.abbreviation;
    const response = await fetch(
      `/api/module-lectures/${department}/${moduleGroups[department]}/${selectedModule.id}`,
    );
    const lectures = await response.json();

    setSelectedModulesArray((currentArray) => {
      return currentArray.concat({ ...selectedModule, lectures });
    });
  }

  function removeModule(indexToRemove) {
    setSelectedModulesArray((currentArray) => {
      return currentArray.filter((module, index) => index != indexToRemove);
    });
  }

  // lectures of all added modules
  const moduleLectures = selectedModulesArray.flatMap(
    (module) => module.lectures,
  );

  return {
    moduleFaculties,
    setModuleFaculties,
    selectedModuleFaculty,
    setSelectedModuleFaculty,
    modules,
    selectedModule,
    setSelectedModule,
    selectedModulesArray,
    moduleLectures,
    addModule,
    removeModule,
  };
}

export default useModuleSelector;