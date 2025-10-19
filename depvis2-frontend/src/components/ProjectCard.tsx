import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import type { ProjectDto } from "@/types/projects";
import { ArrowBigRight } from "lucide-react";
import { Link } from "react-router-dom";

type ProjectCardProps = {
  project: ProjectDto;
};

const ProjectCard = ({ project }: ProjectCardProps) => {
  return (
    <Card className="w-72">
      <CardHeader>
        <CardTitle>{project.name}</CardTitle>
        <CardDescription>{project.projectType}</CardDescription>
        <CardAction>
          <Link to={`/${project.id}`}>
            <ArrowBigRight />
          </Link>
        </CardAction>
      </CardHeader>
      <CardContent>
        <p>Card Content</p>
      </CardContent>
    </Card>
  );
};

export default ProjectCard;
