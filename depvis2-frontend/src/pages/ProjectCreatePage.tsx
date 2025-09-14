import ProjectCreateForm from "./ProjectCreateForm";

const ProjectCreatePage = () => {
  return (
    <div className="">
      <ProjectCreateForm
        onSuccess={(created) => {
          console.log("Created", created);
        }}
      />
    </div>
  );
};

export default ProjectCreatePage;
