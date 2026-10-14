import { useEffect, useState } from "react";

const moduleFaculties = [
  { id: 1, name: "test1" },
  { id: 2, name: "test2" },
  { id: 3, name: "test3" },
  { id: 4, name: "test4" },
  { id: 5, name: "test5" },
];

const modules = [
  { id: 1, name: "test1" },
  { id: 2, name: "test2" },
  { id: 3, name: "test3" },
  { id: 4, name: "test4" },
  { id: 5, name: "test5" },
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
