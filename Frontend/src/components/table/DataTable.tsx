import { flexRender } from "@tanstack/react-table";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { cn } from "@/lib/utils";
import { DataTablePagination } from "../DataTablePagination";

interface DataTableProps<TData> {
  table: import("@tanstack/table-core").Table<TData>;
  className?: string;
  isLoading?: boolean;
  onClick?: (row: TData) => void;
  loadingRows?: number;
}

const WithTooltip = ({
  children,
  tooltip,
  className,
}: {
  children: React.ReactNode;
  tooltip: React.ReactNode;
  className?: string;
}) => {
  return (
    <Tooltip>
      <TooltipTrigger asChild>
        <span
          className={cn(
            "block whitespace-nowrap overflow-hidden text-ellipsis",
            className
          )}
          title={typeof tooltip === "string" ? tooltip : undefined}
        >
          {children}
        </span>
      </TooltipTrigger>
      <TooltipContent className="max-w-sm break-words">
        {tooltip}
      </TooltipContent>
    </Tooltip>
  );
};

export function DataTable<TData>({
  className,
  table,
  isLoading = false,
  loadingRows = 20,
  onClick,
}: DataTableProps<TData>) {
  const visibleColCount =
    table.getVisibleFlatColumns?.().length ?? table.getAllColumns().length;

  return (
    <div
      className={cn(
        "rounded-md border overflow-x-auto flex flex-col justify-between",
        className
      )}
    >
      <TooltipProvider delayDuration={150}>
        <Table className="w-full table-fixed h-full">
          <TableHeader className="sticky top-0 z-10 bg-background">
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => {
                  const headerNode = header.isPlaceholder
                    ? null
                    : flexRender(
                        header.column.columnDef.header,
                        header.getContext()
                      );

                  return (
                    <TableHead
                      key={header.id}
                      style={{ width: header.column.getSize() }}
                      className="p-2"
                    >
                      <WithTooltip tooltip={headerNode ?? ""}>
                        {headerNode}
                      </WithTooltip>
                    </TableHead>
                  );
                })}
              </TableRow>
            ))}
          </TableHeader>

          <TableBody>
            {isLoading ? (
              Array.from({ length: loadingRows }).map((_, i) => (
                <TableRow key={`loading-${i}`} className="animate-pulse">
                  <TableCell colSpan={visibleColCount} className="h-12">
                    <div className="flex items-center gap-3">
                      <div className="h-4 w-full rounded bg-muted" />
                    </div>
                  </TableCell>
                </TableRow>
              ))
            ) : table.getRowModel().rows?.length ? (
              table.getRowModel().rows.map((row) => (
                <TableRow
                  key={row.id}
                  onClick={onClick ? () => onClick(row.original) : undefined}
                  data-state={row.getIsSelected() && "selected"}
                >
                  {row.getVisibleCells().map((cell) => {
                    const cellNode = flexRender(
                      cell.column.columnDef.cell,
                      cell.getContext()
                    );
                    return (
                      <TableCell
                        key={cell.id}
                        style={{ width: cell.column.getSize() }}
                        className="p-2"
                      >
                        <WithTooltip tooltip={cellNode}>{cellNode}</WithTooltip>
                      </TableCell>
                    );
                  })}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={visibleColCount}
                  className="h-full text-center"
                >
                  No results.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TooltipProvider>
      <DataTablePagination table={table} />
    </div>
  );
}
