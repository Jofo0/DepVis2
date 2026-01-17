import React from "react";

const InfoRow = ({
  label,
  children,
}: {
  label: string;
  children: React.ReactNode;
}) => (
  <div className="grid grid-cols-[140px,1fr] items-start gap-3">
    <div className="text-sm text-muted-foreground">{label}</div>
    <div className="text-sm font-medium break-all">{children}</div>
  </div>
);

export default InfoRow;
