import {
  Listbox,
  ListboxButton,
  ListboxOption,
  ListboxOptions,
} from "@headlessui/react";

function SelectListbox({
  label,
  items,
  value,
  onChange,
  placeholder = "Select...",
}) {
  return (
    <div className="mb-2">
      <h3>{label}</h3>

      <Listbox value={value} onChange={onChange}>
        <ListboxButton className="form-select text-start" aria-label={label}>
          {value ? value.name : placeholder}
        </ListboxButton>

        <ListboxOptions anchor="bottom start" className="list-group listbox">
          {items.map((item) => (
            <ListboxOption
              key={item.id}
              value={item}
              className="list-group-item list-group-item-action"
            >
              {item.name}
            </ListboxOption>
          ))}
        </ListboxOptions>
      </Listbox>
    </div>
  );
}

export default SelectListbox;
