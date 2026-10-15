import { useEffect, useState } from "react";

const modules = [
  { id: 1, name: "module1" },
  { id: 2, name: "module2" },
  { id: 3, name: "module3" },
  { id: 4, name: "module4" },
  { id: 5, name: "module5" },
];

function useModuleSelector() {
  const [moduleFaculties, setModuleFaculties] = useState([]);
  const [selectedModuleFaculty, setSelectedModuleFaculty] = useState(null);
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

  function addModule() {
    if (!selectedModule || !selectedModuleFaculty) {
      return;
    }

    setSelectedModulesArray((currentArray) => {
      return currentArray.concat(selectedModule);
    });
  }

  function removeModule(indexToRemove) {
    setSelectedModulesArray((currentArray) => {
      return currentArray.filter((module, index) => index != indexToRemove);
    });
  }

  return {
    moduleFaculties,
    setModuleFaculties,
    selectedModuleFaculty,
    setSelectedModuleFaculty,
    modules,
    selectedModule,
    setSelectedModule,
    selectedModulesArray,
    addModule,
    removeModule,
  };
}

export default useModuleSelector;
