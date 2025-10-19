"use client";

import * as React from "react";
import { AudioWaveform } from "lucide-react";

import { NavMain } from "@/components/nav-main";
import { TeamSwitcher } from "@/components/team-switcher";
import {
  Sidebar,
  SidebarContent,
  SidebarHeader,
  SidebarRail,
} from "@/components/ui/sidebar";
import { useGetProjectsQuery } from "@/store/api/projectsApi";
import useSidebar from "@/utils/useSidebar";

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
  const { data: projects, isLoading } = useGetProjectsQuery();
  const { data: sidebarData } = useSidebar();
  return (
    <Sidebar collapsible="icon" {...props}>
      <SidebarHeader>
        <TeamSwitcher
          isLoading={isLoading}
          projects={
            projects?.map((x) => ({
              id: x.id,
              name: x.name,
              logo: AudioWaveform,
            })) ?? []
          }
        />
      </SidebarHeader>
      <SidebarContent>
        <NavMain items={sidebarData.navMain} />
      </SidebarContent>
      <SidebarRail />
    </Sidebar>
  );
}
