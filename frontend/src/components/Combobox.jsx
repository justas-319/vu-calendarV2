import {
  Combobox,
  ComboboxInput,
  ComboboxOption,
  ComboboxOptions,
} from "@headlessui/react";
import { useState } from "react";

function SearchSelect({ label, placeholder, items, onSelect }) {
  const [selectedItem, setSelectedItem] = useState(null);
  const [query, setQuery] = useState("");

  const filteredItems =
    query === ""
      ? items
      : items.filter((item) =>
          item.name.toLowerCase().includes(query.toLowerCase()),
        );

  function handleChange(item) {
    setSelectedItem(item);
    onSelect(item);
  }

  return (
    <div className="mb-2">
      <div className="form-label mb-1">{label}</div>

      <Combobox
        immediate
        value={selectedItem}
        onChange={handleChange}
        onClose={() => setQuery("")}
      >
        <ComboboxInput
          className="form-control"
          placeholder={placeholder}
          aria-label={label}
          autoComplete="off"
          displayValue={(item) => item?.name || ""}
          onChange={(event) => setQuery(event.target.value)}
        />

        <ComboboxOptions anchor="bottom start" className="list-group combobox">
          {filteredItems.map((item) => (
            <ComboboxOption
              key={item.id}
              value={item}
              className="list-group-item list-group-item-action"
            >
              {item.name}
            </ComboboxOption>
          ))}
        </ComboboxOptions>
      </Combobox>
    </div>
  );
}

export default SearchSelect;
