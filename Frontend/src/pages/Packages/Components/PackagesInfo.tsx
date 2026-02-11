import { PieCustomChart } from "@/components/chart/PieCustomChart";
import { DataTable } from "@/components/table/DataTable";
import type { PackageItemDto, PackagesDetailedDto } from "@/types/packages";
import type { Table } from "@tanstack/react-table";

export type PackagesInfoProps = {
  isLoading: boolean;
  data: PackagesDetailedDto | undefined;
  table: Table<PackageItemDto>;
  onExportClick: () => void;
  onEcosystemClick: (ecosystem: string) => void;
  onVulnerabilityClick: (vulnerability: string) => void;
  ecosystemFilter?: string;
  vulnerabilityFilter?: string;
};

const PackagesInfo = ({
  isLoading,
  data,
  table,
  onExportClick,
  onEcosystemClick,
  onVulnerabilityClick,
  ecosystemFilter,
  vulnerabilityFilter,
}: PackagesInfoProps) => {
  return (
    <div className="flex flex-row gap-10 w-full h-full justify-evenly">
      <div className="h-max-full w-1/2">
        <DataTable
          onExportClick={onExportClick}
          isLoading={isLoading}
          className="min-h-[calc(100vh-9rem)] max-h-[calc(100vh-9rem)]"
          table={table}
        />
      </div>
      <div className="flex flex-col gap-6 w-1/2 h-full">
        <PieCustomChart
          title="Ecosystems"
          className="min-h-[calc(42vh)] max-h-[calc(42vh)]"
          pies={data?.ecoSystems ?? []}
          isLoading={isLoading}
          filteredBy={ecosystemFilter}
          onSliceClick={onEcosystemClick}
        />

        <PieCustomChart
          title="Vulnerabilities"
          onSliceClick={onVulnerabilityClick}
          isLoading={isLoading}
          className="min-h-[calc(42vh)] max-h-[calc(42vh)]"
          filteredBy={vulnerabilityFilter}
          pies={data?.vulnerabilities ?? []}
        />
      </div>
    </div>
  );
};

export default PackagesInfo;
