import {
  Card,
  CardAction,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import type { ProjectDto } from "@/types/projects";
import { ArrowRight } from "lucide-react";
import { Link } from "react-router-dom";
import { Button } from "../ui/button";

type ProjectCardProps = {
  project: ProjectDto;
};

const ProjectCard = ({ project }: ProjectCardProps) => {
  return (
    <Card className="w-80 h-36">
      <CardHeader>
        <CardTitle>{project.name}</CardTitle>
        <CardDescription>
          {project.projectLink.replace("https://github.com/", "")}
        </CardDescription>
        <CardAction>
          <Button variant={"ghost"} className="p-2">
            <Link to={`/${project.id}`}>
              <ArrowRight />
            </Link>
          </Button>
        </CardAction>
      </CardHeader>
    </Card>
  );
};

export default ProjectCard;
