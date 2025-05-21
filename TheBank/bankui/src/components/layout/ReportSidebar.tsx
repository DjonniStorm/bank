import React from 'react';
import {
  Sidebar,
  SidebarContent,
  SidebarGroupContent,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarProvider,
  SidebarTrigger,
} from '@/components/ui/sidebar';

type SidebarProps = {
  onWordClick: () => void;
  onPdfClick: () => void;
  onExcelClick: () => void;
};
export const ReportSidebar = ({
  onWordClick,
  onExcelClick,
  onPdfClick,
}: SidebarProps): React.JSX.Element => {
  return (
    <SidebarProvider className="w-[400px]">
      <Sidebar variant="floating" collapsible="none">
        <SidebarContent />
        <SidebarGroupContent className="">
          <SidebarMenu>
            <SidebarMenuItem>
              <SidebarMenuButton asChild onClick={onWordClick}>
                <span>отчет word КЛАДОВЩИКА</span>
              </SidebarMenuButton>
            </SidebarMenuItem>
            <SidebarMenuItem>
              <SidebarMenuButton asChild onClick={onExcelClick}>
                <span>отчет excel КЛАДОВЩИКА</span>
              </SidebarMenuButton>
            </SidebarMenuItem>
            <SidebarMenuItem>
              <SidebarMenuButton asChild onClick={onPdfClick}>
                <span>отчет pdf КЛАДОВЩИКА</span>
              </SidebarMenuButton>
            </SidebarMenuItem>
          </SidebarMenu>
        </SidebarGroupContent>
      </Sidebar>
    </SidebarProvider>
  );
};
