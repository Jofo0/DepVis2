import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import { Plus, Minus } from "lucide-react";
import { DeltaBadge } from "./StatCard";
import { Separator } from "@radix-ui/react-separator";
import { Badge } from "@/components/ui/badge";

export type CompareCardProps = {
  addedCount: number;
  removedCount: number;
  net: number;
  title: string;
  description?: string;
};

const CompareCard = ({
  addedCount,
  removedCount,
  net,
  title,
  description,
}: CompareCardProps) => {
  return (
    <Card>
      <CardHeader className="pb-3">
        <CardTitle className="text-base">{title}</CardTitle>
        {description && (
          <CardDescription className="text-xs">{description}</CardDescription>
        )}
      </CardHeader>
      <CardContent className="space-y-3">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2 text-sm">
            <Plus className="h-4 w-4" /> Added
          </div>
          <Badge className="tabular-nums">{addedCount}</Badge>
        </div>
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2 text-sm">
            <Minus className="h-4 w-4" /> Removed
          </div>
          <Badge variant="secondary" className="tabular-nums">
            {removedCount}
          </Badge>
        </div>
        <Separator />
        <div className="flex items-center justify-between">
          <div className="text-xs text-muted-foreground">Net</div>
          <DeltaBadge delta={net} />
        </div>
      </CardContent>
    </Card>
  );
};

export default CompareCard;
