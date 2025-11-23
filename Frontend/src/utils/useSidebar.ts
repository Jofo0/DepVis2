import {
  Home,
  GitBranch,
  Package,
  AlertTriangle,
  Frame,
  PieChart,
  CircleDashed,
} from "lucide-react";
import { useParams, useLocation } from "react-router-dom";

const useSidebar = () => {
  const { id } = useParams<{ id: string }>();
  const location = useLocation();

  if (!id) {
    return { data: { navMain: [], user: null } };
  }

  const navItems = [
    {
      title: "Home",
      url: `/${id}`,
      icon: Home,
    },
    {
      title: "Branches",
      url: `/${id}/branches`,
      icon: GitBranch,
    },
    {
      title: "Packages",
      url: `/${id}/packages`,
      icon: Package,
    },
    {
      title: "Vulnerabilities",
      url: `/${id}/vulnerabilities`,
      icon: AlertTriangle,
    },
    {
      title: "Graph",
      url: `/${id}/graph`,
      icon: CircleDashed,
    },
  ];

  const data = {
    user: {
      name: "shadcn",
      email: "m@example.com",
      avatar: "/avatars/shadcn.jpg",
    },

    navMain: navItems.map((item) => ({
      ...item,
      isActive: location.pathname === item.url,
    })),

    projects: [
      {
        name: "Design Engineering",
        url: "#",
        icon: Frame,
      },
      {
        name: "Sales & Marketing",
        url: "#",
        icon: PieChart,
      },
      {
        name: "Travel",
        url: "#",
        icon: Map,
      },
    ],
  };

  return { data };
};

export default useSidebar;
