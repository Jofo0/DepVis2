import {
  Tooltip as UiTooltip,
  TooltipContent as UiTooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";

type TooltipProps = {
  content: React.ReactNode;
  trigger: React.ReactNode;
};

const IconTooltip = ({ content, trigger }: TooltipProps) => {
  return (
    <UiTooltip>
      <TooltipTrigger asChild>{trigger}</TooltipTrigger>
      <UiTooltipContent>{content}</UiTooltipContent>
    </UiTooltip>
  );
};

export default IconTooltip;
