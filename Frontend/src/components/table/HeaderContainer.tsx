const HeaderContainer = ({
  children,
  header,
}: {
  children: React.ReactNode;
  header: string;
}) => {
  return (
    <div className="flex flex-row items-center">
      {header}
      {children}
    </div>
  );
};

export default HeaderContainer;
