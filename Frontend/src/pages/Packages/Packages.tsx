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
import { useCallback, useEffect, useState } from "react";
import PackagesInfo from "./Components/PackagesInfo";
import PageHeader from "@/components/PageHeader";
import { LoadingButton } from "@/components/ui/button";

const Packages = () => {
  const { branch, commit, isLoading: isLoadingBranch } = useBranch();
  const columns = useGetPackagesColumns();
  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);

  const [ecosystemFilter, setEcosystemFilter] = useState("");
  const [depthFilter, setDepthFilter] = useState("");
  const [vulnerabilityFilter, setVulnerabilityFilter] = useState("");
  const [fetchPackages, { data, isLoading: isLoading, isSuccess }] =
    useLazyGetPackagesQuery();
  const [triggerExport, { isFetching: isExporting }] =
    useLazyGetPackagesExportQuery();

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

  const getFullOdata = useCallback(() => {
    const chartFilterOdata = buildPackagesOdata({
      ecosystem: ecosystemFilter || null,
      vulnerability: vulnerabilityFilter || null,
      depth: depthFilter || null,
    });
    const filterOdata = toODataFilter(columnFilters);
    const filter = joinODataFilters([chartFilterOdata, filterOdata]);

    const sortOdata = toODataOrderBy(sorting);

    return [filter, sortOdata].filter(Boolean).join("&");
  }, [
    ecosystemFilter,
    vulnerabilityFilter,
    columnFilters,
    sorting,
    depthFilter,
  ]);

  useEffect(() => {
    if (!branch) return;

    fetchPackages(
      {
        id: branch.id,
        odata: getFullOdata(),
        commitId: commit?.commitId,
      },
      true,
    );
  }, [
    branch,
    commit,
    getFullOdata,
    ecosystemFilter,
    vulnerabilityFilter,
    columnFilters,
    fetchPackages,
    sorting,
  ]);

  const onEcosystemClick = (name: string) => {
    setEcosystemFilter((prev) => (prev === name ? "" : name));
  };

  const onDepthClick = (name: string) => {
    setDepthFilter((prev) => (prev === name ? "" : name));
  };

  const onVulnerabilityClick = (name: string) => {
    setVulnerabilityFilter((prev) => (prev === name ? "" : name));
  };

  const onExportClick = async () => {
    if (!branch) return;
    const blob = await triggerExport(
      {
        id: branch.id,
        commitId: commit?.commitId,
        odata: getFullOdata(),
      },
      true,
    ).unwrap();

    downloadBlob(
      blob,
      `packages-${branch.name}${commit ? `-${commit.commitName}` : ""}-${getPrettyDate()}.csv`,
    );
  };

  return (
    <div className="flex flex-col w-full h-full justify-evenly gap-2">
      <PageHeader
        title="Packages"
        description="View and analyze packages in the selected source"
      >
        <LoadingButton
          isLoading={isExporting}
          text="Export Vulnerabilities"
          onClick={onExportClick}
        />
      </PageHeader>
      <PackagesInfo
        data={data}
        isLoading={isLoading || !isSuccess || isLoadingBranch}
        onEcosystemClick={onEcosystemClick}
        onVulnerabilityClick={onVulnerabilityClick}
        onDepthClick={onDepthClick}
        depthFilter={depthFilter}
        table={table}
        ecosystemFilter={ecosystemFilter}
        vulnerabilityFilter={vulnerabilityFilter}
      />
    </div>
  );
};

export default Packages;
