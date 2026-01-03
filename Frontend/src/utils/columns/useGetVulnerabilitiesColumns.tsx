import { SearchFilter } from "@/components/table/SearchFilter";
import HeaderContainer from "@/components/table/HeaderContainer";
import { SortButton } from "@/components/table/SortButton";
import type { VulnerabilitySmallDto } from "@/types/vulnerabilities";
import type { Column, ColumnDef } from "@tanstack/react-table";
import { useMemo } from "react";

export const useGetVulnerabilitiesColumns =
  (): ColumnDef<VulnerabilitySmallDto>[] =>
    useMemo(
      () => [
        {
          accessorKey: "vulnerabilityId",
          header: ({
            column,
          }: {
            column: Column<VulnerabilitySmallDto, unknown>;
          }) => (
            <HeaderContainer header="Vulnerability Id">
              <SortButton column={column} />
              <SearchFilter column={column} />
            </HeaderContainer>
          ),
          size: 150,
        },
        {
          accessorKey: "packageName",
          header: ({
            column,
          }: {
            column: Column<VulnerabilitySmallDto, unknown>;
          }) => (
            <HeaderContainer header="Package Name">
              <SortButton column={column} />
              <SearchFilter column={column} />
            </HeaderContainer>
          ),
          size: 150,
        },
        {
          accessorKey: "severity",
          header: ({
            column,
          }: {
            column: Column<VulnerabilitySmallDto, unknown>;
          }) => (
            <HeaderContainer header="Severity">
              <SortButton column={column} />
            </HeaderContainer>
          ),
        },
      ],
      []
    );
