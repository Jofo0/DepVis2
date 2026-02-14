import { flexRender } from "@tanstack/react-table";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { cn } from "@/lib/utils";
import { DataTablePagination } from "../DataTablePagination";

interface DataTableProps<TData> {
  table: import("@tanstack/table-core").Table<TData>;
  className?: string;
  isLoading?: boolean;
  onClick?: (row: TData) => void;
  loadingRows?: number;
  onExportClick: () => void;
}

export function DataTable<TData>({
  className,
  table,
  isLoading = false,
  onClick,
  onExportClick,
}: DataTableProps<TData>) {
  const visibleColCount =
    table.getVisibleFlatColumns?.().length ?? table.getAllColumns().length;

  if (isLoading) {
    return (
      <div
        className={cn(
          "rounded-md border h-full w-full animate-pulse opacity-20 bg-gray-600",
          className,
        )}
      />
    );
  }
  return (
    <div
      className={cn(
        "rounded-md border overflow-x-auto flex flex-col justify-between",
        className,
      )}
    >
      <Table className="w-full table-fixed h-full">
        <TableHeader className="sticky top-0 z-10 bg-background">
          {table.getHeaderGroups().map((headerGroup) => (
            <TableRow key={headerGroup.id}>
              {headerGroup.headers.map((header) => {
                const headerNode = header.isPlaceholder
                  ? null
                  : flexRender(
                      header.column.columnDef.header,
                      header.getContext(),
                    );

                return (
                  <TableHead
                    key={header.id}
                    style={{ width: header.column.getSize() }}
                    className="p-2"
                  >
                    {headerNode}
                  </TableHead>
                );
              })}
            </TableRow>
          ))}
        </TableHeader>

        <TableBody>
          {table.getRowModel().rows?.length ? (
            table.getRowModel().rows.map((row) => (
              <TableRow
                key={row.id}
                onClick={onClick ? () => onClick(row.original) : undefined}
                data-state={row.getIsSelected() && "selected"}
              >
                {row.getVisibleCells().map((cell) => {
                  const cellNode = flexRender(
                    cell.column.columnDef.cell,
                    cell.getContext(),
                  );
                  return (
                    <TableCell
                      key={cell.id}
                      style={{ width: cell.column.getSize() }}
                      className="p-2"
                    >
                      {cell.column.columnDef.meta?.disableTooltip
                        ? cellNode
                        : cellNode}
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
      <DataTablePagination onExportClick={onExportClick} table={table} />
    </div>
  );
}
