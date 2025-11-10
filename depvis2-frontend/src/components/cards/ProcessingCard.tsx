import { type Branch } from "@/types/branches";
import { Card, CardHeader, CardDescription, CardContent } from "../ui/card";
import { CheckCircle2 } from "lucide-react";

type ProcessingCardProps = {
  branches: Branch[];
  header: string;
  description: string;
};
const ProcessingCard = ({
  branches,
  header,
  description,
}: ProcessingCardProps) => {
  return (
    <Card className="w-1/5 max-h-[calc(30rem)] min-h-[calc(10rem)]">
      <CardHeader>{header}</CardHeader>
      <CardDescription className="h-10">{description}</CardDescription>
      <CardContent className="mt-2 flex flex-col items-center py-4">
        {branches.length === 0 ? (
          <p className="text-2xl text-green-800 flex flex-row gap-3">
            All finished! <CheckCircle2 />
          </p>
        ) : (
          branches.map((branch) => <div key={branch.id}>{branch.name}</div>)
        )}
      </CardContent>
    </Card>
  );
};

export default ProcessingCard;
