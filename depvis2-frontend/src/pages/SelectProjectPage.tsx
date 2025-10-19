import ProjectCard from "@/components/projectCard";
import { useGetProjectsQuery } from "@/store/api/projectsApi";

const SelectProjectPage = () => {
  const { data: projects, isLoading } = useGetProjectsQuery();

  return (
    <div className="flex flex-col gap-4 justify-start h-full w-full pt-40 items-center">
      <h1 className="text-7xl font-oswald font-light">DepVis v2</h1>
      <h2 className="pt-4">Select a Project</h2>
      <div className="grid grid-cols-3 gap-4">
        {projects && projects.map((x) => <ProjectCard project={x} />)}
      </div>
    </div>
  );
};

export default SelectProjectPage;
