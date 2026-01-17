import { AppSidebar } from "@/components/app-sidebar";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
} from "@/components/ui/breadcrumb";
import { SidebarInset, SidebarProvider } from "@/components/ui/sidebar";
import { BranchProvider } from "@/utils/hooks/BranchProvider";
import { Outlet, useLocation } from "react-router-dom";

const Layout = () => {
  const location = useLocation();

  let pageName = location.pathname;

  if (pageName.includes("vulnerabilities")) {
    pageName = "Vulnerabilities";
  } else if (pageName.includes("branches")) {
    pageName = "Branches";
  } else if (pageName.includes("graph")) {
    pageName = "Graph";
  } else if (pageName.includes("packages")) {
    pageName = "Packages";
  } else if (pageName.includes("branch-history")) {
    pageName = "Branch History";
  } else {
    pageName = "Home";
  }

  return (
    <SidebarProvider>
      <BranchProvider>
        <AppSidebar />
        <SidebarInset className="z-1">
          <header className="flex h-16 shrink-0 items-center gap-2 transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-12">
            <div className="flex items-center gap-2 px-4">
              <Breadcrumb>
                <BreadcrumbList>
                  <BreadcrumbItem className="hidden md:block">
                    <BreadcrumbLink className="text-3xl">
                      {pageName}
                    </BreadcrumbLink>
                  </BreadcrumbItem>
                </BreadcrumbList>
              </Breadcrumb>
            </div>
          </header>
          <div className="px-16 h-full w-full">
            <Outlet />
          </div>
        </SidebarInset>
      </BranchProvider>
    </SidebarProvider>
  );
};

export default Layout;
