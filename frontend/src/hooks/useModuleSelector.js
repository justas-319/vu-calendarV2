import { useEffect, useState } from "react";

const moduleFaculties = [
  { id: 1, name: "faculty1" },
  { id: 2, name: "faculty2" },
  { id: 3, name: "faculty3" },
  { id: 4, name: "faculty4" },
  { id: 5, name: "faculty5" },
];

const modules = [
  { id: 1, name: "module1" },
  { id: 2, name: "module2" },
  { id: 3, name: "module3" },
  { id: 4, name: "module4" },
  { id: 5, name: "module5" },
];

function useModuleSelector() {
  const [selectedModuleFaculty, setSelectedModuleFaculty] = useState(null);
  const [selectedModule, setSelectedModule] = useState(null);

  // array of modules the user has added
  const [selectedModulesArray, setSelectedModulesArray] = useState([]);

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
