import {
  Card,
  CardAction,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { PlusCircle } from "lucide-react";
import { Link } from "react-router-dom";

const AddProjectCard = () => {
  return (
    <Card className="w-80 h-36 border-dashed">
      <CardHeader>
        <CardTitle className="text-gray-500">Add New Project</CardTitle>
        <CardAction>
          <Link to={`/new`}>
            <PlusCircle className="text-gray-500" />
          </Link>
        </CardAction>
      </CardHeader>
      <CardContent></CardContent>
    </Card>
  );
};

export default AddProjectCard;
