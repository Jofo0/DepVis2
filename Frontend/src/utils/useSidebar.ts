import {
  Home,
  GitBranch,
  Package,
  AlertTriangle,
  Frame,
  PieChart,
} from "lucide-react";
import { useParams } from "react-router-dom";

const useSidebar = () => {
  const { id } = useParams<{ id: string }>();

  if (!id) {
    return { data: { navMain: [], user: null } };
  }

  const data = {
    user: {
      name: "shadcn",
      email: "m@example.com",
      avatar: "/avatars/shadcn.jpg",
    },

    navMain: [
      {
        title: "Home",
        url: id,
        icon: Home,
        isActive: true,
      },
      {
        title: "Branches",
        url: `${id}/branches`,
        icon: GitBranch,
        isActive: true,
      },
      {
        title: "Packages",
        url: `${id}/packages`,
        icon: Package,
        isActive: true,
      },
      {
        title: "Vulnerabilities",
        url: `${id}/vulnerabilities`,
        icon: AlertTriangle,
        isActive: false,
      },
      {
        title: "Graph",
        url: `${id}/graph`,
        icon: AlertTriangle,
        isActive: false,
      },
    ],
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
