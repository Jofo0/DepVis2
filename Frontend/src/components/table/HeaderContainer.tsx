const HeaderContainer = ({
  children,
  header,
}: {
  children: React.ReactNode;
  header: string;
}) => {
  return (
    <div className="flex flex-row items-center gap-1">
      {header}
      {children}
    </div>
  );
};

export default HeaderContainer;
