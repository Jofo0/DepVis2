import BranchSelector from "@/components/BranchSelector";
import {
  useLazyGetPackagesExportQuery,
  useLazyGetPackagesQuery,
} from "@/store/api/projectsApi";
import { buildPackagesOdata } from "@/utils/buildPackagesOdata";
import { useGetPackagesColumns } from "@/utils/columns/useGetPackagesColumns";
import { getPrettyDate } from "@/utils/dateHelper";
import { downloadBlob } from "@/utils/downloadBlob";
import { useBranch } from "@/utils/hooks/BranchProvider";
import {
  toODataFilter,
  joinODataFilters,
  toODataOrderBy,
} from "@/utils/odataHelper";
import {
  useReactTable,
  getSortedRowModel,
  getCoreRowModel,
  getPaginationRowModel,
  type SortingState,
  type ColumnFiltersState,
} from "@tanstack/react-table";
import { useEffect, useState } from "react";
import PackagesInfo from "./Components/PackagesInfo";

const Packages = () => {
  const { branch, isLoading: isLoadingBranch } = useBranch();
  const columns = useGetPackagesColumns();
  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);

  const [ecosystemFilter, setEcosystemFilter] = useState("");
  const [vulnerabilityFilter, setVulnerabilityFilter] = useState("");
  const [fetchPackages, { data, isFetching: isLoading }] =
    useLazyGetPackagesQuery();
  const [triggerExport] = useLazyGetPackagesExportQuery();

  const table = useReactTable({
    data: data?.packageItems ?? [],
    columns,
    onSortingChange: setSorting,
    onColumnFiltersChange: setColumnFilters,
    getSortedRowModel: getSortedRowModel(),
    state: {
      sorting,
      columnFilters,
    },
    getPaginationRowModel: getPaginationRowModel(),
    manualSorting: true,
    manualFiltering: true,
    getCoreRowModel: getCoreRowModel(),
  });

  const getFullOdata = () => {
    const chartFilterOdata = buildPackagesOdata({
      ecosystem: ecosystemFilter || null,
      vulnerability: vulnerabilityFilter || null,
    });
    const filterOdata = toODataFilter(columnFilters);
    const filter = joinODataFilters([chartFilterOdata, filterOdata]);

    const sortOdata = toODataOrderBy(sorting);

    return [filter, sortOdata].filter(Boolean).join("&");
  };

  useEffect(() => {
    if (!branch) return;

    fetchPackages(
      {
        id: branch.id,
        odata: getFullOdata(),
      },
      true,
    );
  }, [
    branch,
    ecosystemFilter,
    vulnerabilityFilter,
    columnFilters,
    fetchPackages,
    sorting,
  ]);

  const onEcosystemClick = (name: string) => {
    setEcosystemFilter((prev) => (prev === name ? "" : name));
  };

  const onVulnerabilityClick = (name: string) => {
    setVulnerabilityFilter((prev) => (prev === name ? "" : name));
  };

  const onExportClick = async () => {
    if (!branch) return;

    const blob = await triggerExport(
      {
        id: branch.id,
        odata: getFullOdata(),
      },
      true,
    ).unwrap();

    downloadBlob(blob, `packages-${branch.name}-${getPrettyDate()}.csv`);
  };

  return (
    <div className="flex flex-col gap-3 w-full h-full">
      <div className="flex flex-col w-full h-full justify-evenly gap-2">
        <BranchSelector />
        {!isLoadingBranch && branch && !isLoading && (
          <PackagesInfo
            data={data}
            isLoading={isLoading}
            onEcosystemClick={onEcosystemClick}
            onVulnerabilityClick={onVulnerabilityClick}
            onExportClick={onExportClick}
            table={table}
            ecosystemFilter={ecosystemFilter}
            vulnerabilityFilter={vulnerabilityFilter}
          />
        )}
      </div>
    </div>
  );
};

export default Packages;
