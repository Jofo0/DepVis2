import * as React from "react";

import { cn } from "@/lib/utils";
import { Button } from "./button";
import { Loader2 } from "lucide-react";

function Input({ className, type, ...props }: React.ComponentProps<"input">) {
  return (
    <input
      type={type}
      data-slot="input"
      className={cn(
        "file:text-foreground placeholder:text-muted-foreground selection:bg-primary selection:text-primary-foreground dark:bg-input/30 border-input h-9 w-full min-w-0 rounded-md border bg-transparent px-3 py-1 text-base shadow-xs transition-[color,box-shadow] outline-none file:inline-flex file:h-7 file:border-0 file:bg-transparent file:text-sm file:font-medium disabled:pointer-events-none disabled:cursor-not-allowed disabled:opacity-50 md:text-sm",
        "focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px]",
        "aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive",
        className,
      )}
      {...props}
    />
  );
}

const DevInput = ({
  title,
  error,
  className,
  ...props
}: React.ComponentProps<"input"> & { title: string; error?: string }) => {
  return (
    <div className={cn("space-y-1", className)}>
      <label className="block text-sm font-medium">{title}</label>
      <Input {...props} className={`${error ? "border-red-500" : ""}`} />
      {error && <p className="text-xs text-red-500">{error}</p>}
    </div>
  );
};

const InputButton = ({
  title,
  error,
  className,
  isLoading,
  onClick,
  disabledButton,
  buttonText,
  ...props
}: React.ComponentProps<"input"> & {
  title: string;
  error?: string;
  isLoading: boolean;
  onClick: () => void;
  disabledButton: boolean;
  buttonText: string;
}) => {
  return (
    <div className={cn("space-y-1", className)}>
      <label className="block text-sm font-medium">{title}</label>
      <div className="w-full flex flex-row gap-2">
        <Input {...props} className={`${error ? "border-red-500" : ""}`} />
        <Button
          type="button"
          onClick={onClick}
          disabled={disabledButton}
          className="w-26"
        >
          {isLoading ? <Loader2 className="animate-spin" /> : buttonText}
        </Button>
      </div>
      {error && <p className="text-xs text-red-500">{error}</p>}
    </div>
  );
};

export { Input, DevInput, InputButton };
