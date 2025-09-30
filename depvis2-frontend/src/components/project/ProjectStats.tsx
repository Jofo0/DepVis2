import { useGetProjectStatsQuery } from "../../store/api/projectsApi";

export const ProjectStats = ({
  id,
  branch,
}: {
  id: string;
  branch: string;
}) => {
  const { data, isLoading, error } = useGetProjectStatsQuery({ id, branch });

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
