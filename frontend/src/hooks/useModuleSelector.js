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

  return {
    moduleFaculties,
    selectedModuleFaculty,
    setSelectedModuleFaculty,
    modules,
    selectedModule,
    setSelectedModule,
  };
}

export default useModuleSelector;
