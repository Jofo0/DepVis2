import { PieCustomChart } from "@/components/chart/PieCustomChart";
import { DataTable } from "@/components/table/DataTable";
import type { PackageItemDto, PackagesDetailedDto } from "@/types/packages";
import type { Table } from "@tanstack/react-table";

export type PackagesInfoProps = {
  isLoading: boolean;
  data: PackagesDetailedDto | undefined;
  table: Table<PackageItemDto>;
  onEcosystemClick: (ecosystem: string) => void;
  onVulnerabilityClick: (vulnerability: string) => void;
  ecosystemFilter?: string;
  vulnerabilityFilter?: string;
  depthFilter?: string;
  onDepthClick?: (depth: string) => void;
};

const PackagesInfo = ({
  isLoading,
  data,
  table,
  onEcosystemClick,
  onVulnerabilityClick,
  ecosystemFilter,
  vulnerabilityFilter,
  depthFilter,
  onDepthClick,
}: PackagesInfoProps) => {
  return (
    <div className="flex flex-row gap-10 w-full h-full justify-evenly">
      <div className="h-max-full w-1/2">
        <DataTable
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

        <div className="flex flex-row w-full gap-3">
          <PieCustomChart
            title="Vulnerabilities"
            onSliceClick={onVulnerabilityClick}
            isLoading={isLoading}
            className="min-h-[calc(42vh)] max-h-[calc(42vh)]"
            filteredBy={vulnerabilityFilter}
            pies={data?.vulnerabilities ?? []}
          />
          <PieCustomChart
            title="Depth"
            onSliceClick={onDepthClick}
            isLoading={isLoading}
            className="min-h-[calc(42vh)] max-h-[calc(42vh)]"
            filteredBy={depthFilter}
            pies={data?.depths ?? []}
          />
        </div>
      </div>
    </div>
  );
};

export default PackagesInfo;
