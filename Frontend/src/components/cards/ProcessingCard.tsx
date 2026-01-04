import { type Branch } from "@/types/branches";
import { Card, CardHeader, CardDescription, CardContent } from "../ui/card";
import { CheckCircle2 } from "lucide-react";

type ProcessingCardProps = {
  branches: Branch[];
  header: string;
  isLoading: boolean;
  description: string;
};
const ProcessingCard = ({
  branches,
  header,
  description,
  isLoading,
}: ProcessingCardProps) => {
  if (branches.length === 0) return;

  return (
    <Card
      className={`w-1/5 max-h-[calc(30rem)] min-h-[calc(10rem)] ${
        isLoading && "animate-pulse"
      }`}
    >
      <CardHeader>{header}</CardHeader>
      <CardDescription className="h-10">{description}</CardDescription>
      {!isLoading && (
        <CardContent className="mt-2 grid grid-cols-2 grid- py-4">
          {branches.length === 0 ? (
            <p className="text-xl text-green-800 flex flex-row gap-3 self-end items-center">
              Finished!
              <CheckCircle2 />
            </p>
          ) : (
            branches.map((branch) => <div key={branch.id}>{branch.name}</div>)
          )}
        </CardContent>
      )}
    </Card>
  );
};

export default ProcessingCard;
