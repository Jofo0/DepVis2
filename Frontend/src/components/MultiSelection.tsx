import {
  MultiSelect,
  MultiSelectTrigger,
  MultiSelectValue,
  MultiSelectContent,
  MultiSelectGroup,
  MultiSelectItem,
} from "./ui/multi-select";

export type SelectionProps = {
  placeholder?: string;
  data: string[];
  values: string[];
  onValuesChange: (values: string[]) => void;
  className?: string;
  title: string;
};

const MultiSelection = ({
  placeholder,
  data,
  values,
  onValuesChange,
  title,
  className,
}: SelectionProps) => {
  return (
    <div className={"space-y-1 w-full"}>
      <label className="block text-sm font-medium">{title}</label>
      <MultiSelect onValuesChange={onValuesChange} values={values}>
        <MultiSelectTrigger className={className}>
          <MultiSelectValue placeholder={placeholder} />
        </MultiSelectTrigger>

        <MultiSelectContent>
          <MultiSelectGroup>
            {data.map((item) => (
              <MultiSelectItem value={item} key={item}>
                {item}
              </MultiSelectItem>
            ))}
          </MultiSelectGroup>
        </MultiSelectContent>
      </MultiSelect>
    </div>
  );
};

export default MultiSelection;
