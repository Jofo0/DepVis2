import { useGetProjectStatsQuery } from "../../store/api/projectsApi";
import type { ProjectBranchDto } from "../../types/projects";

type ProjectStatsProps = {
  branch: ProjectBranchDto;
};

export const ProjectStats = ({ branch }: ProjectStatsProps) => {
  const { data, isLoading, error } = useGetProjectStatsQuery({ id: branch.id });

  if (isLoading) return <p>Loading stats…</p>;
  if (error) return <p>Failed to load stats</p>;
  if (!data) return <p>No stats available</p>;

  return (
    <div>
      <p>Packages: {data.packageCount}</p>
      <p>Vulnerabilities: {data.vulnerabilityCount}</p>
    </div>
  );
};
