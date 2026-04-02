import {
  Tooltip as UiTooltip,
  TooltipContent as UiTooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";

type TooltipProps = {
  content: React.ReactNode;
  trigger: React.ReactNode;
};

const IconTooltip = ({ content, trigger }: TooltipProps) => {
  return (
    <TooltipProvider>
      <UiTooltip>
        <TooltipTrigger asChild>{trigger}</TooltipTrigger>
        <UiTooltipContent>{content}</UiTooltipContent>
      </UiTooltip>
    </TooltipProvider>
  );
};

export default IconTooltip;
