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
import { Badge } from "../ui/badge";
import str from "string-to-color";

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
      <div className="self-end mt-auto">
        {project.ecoSystems.slice(0, 4).map((eco) => (
          <Badge
            key={eco}
            variant="outline"
            style={{ color: str(eco) }}
            className="mr-1"
          >
            {eco}
          </Badge>
        ))}
        {project.ecoSystems.length > 4 && (
          <Badge variant="outline">+{project.ecoSystems.length - 4} more</Badge>
        )}
      </div>
    </Card>
  );
};

export default ProjectCard;
