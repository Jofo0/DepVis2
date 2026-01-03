import { useEffect, useRef, useState } from "react";
import type { Column } from "@tanstack/react-table";
import { Input } from "../ui/input";
import { cn } from "@/lib/utils";
import { Search } from "lucide-react";
import { Button } from "../ui/button";

export type SearchFilterProps<T> = {
  column: Column<T, unknown>;
  placeholder?: string;
  className?: string;
  debounceMs?: number;
};

export const SearchFilter = <T,>({
  column,
  placeholder = "Search...",
  className,
  debounceMs = 450,
}: SearchFilterProps<T>) => {
  const inputRef = useRef<HTMLInputElement>(null);
  const initial = (column.getFilterValue() as string) ?? "";
  const [value, setValue] = useState(initial);
  const [showFilter, setShowFilter] = useState(false);

  useEffect(() => {
    setValue(((column.getFilterValue() as string) ?? "") as string);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [column.getFilterValue()]);

  useEffect(() => {
    const t = window.setTimeout(() => {
      const v = value.trim();
      column.setFilterValue(v.length ? v : undefined);
    }, debounceMs);

    return () => window.clearTimeout(t);
  }, [value, debounceMs, column]);

  useEffect(() => {
    if (!showFilter) return;

    requestAnimationFrame(() => {
      inputRef.current?.focus();
    });
  }, [showFilter]);

  return showFilter ? (
    <div>
      <Input
        ref={inputRef}
        value={value}
        onChange={(e) => setValue(e.target.value)}
        placeholder={placeholder}
        className={cn("w-24", className)}
      />
    </div>
  ) : (
    <Button variant="ghost" onClick={() => setShowFilter(true)}>
      <Search />
    </Button>
  );
};
