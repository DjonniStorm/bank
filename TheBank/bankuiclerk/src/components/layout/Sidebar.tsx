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

import { Plus, Pencil } from 'lucide-react';

import { Link } from 'react-router-dom';
type SidebarProps = {
  onAddClick: () => void;
  onEditClick: () => void;
};

const availableTasks = [
  {
    name: 'Добавить',
    link: '',
  },
  {
    name: 'Редактировать',
    link: '',
  },
];

export const AppSidebar = ({
  onAddClick,
  onEditClick,
}: SidebarProps): React.JSX.Element => {
  return (
    <SidebarProvider>
      <Sidebar variant="floating" collapsible="icon">
        <SidebarTrigger />
        <SidebarContent />
        <SidebarGroupContent className="">
          <SidebarMenu>
            <SidebarMenuItem>
              <SidebarMenuButton asChild onClick={onAddClick}>
                <Link to={availableTasks[0].link}>
                  <Plus />
                  <span>{availableTasks[0].name}</span>
                </Link>
              </SidebarMenuButton>
            </SidebarMenuItem>
            <SidebarMenuItem>
              <SidebarMenuButton asChild onClick={onEditClick}>
                <Link to={availableTasks[1].link}>
                  <Pencil />
                  <span>{availableTasks[1].name}</span>
                </Link>
              </SidebarMenuButton>
            </SidebarMenuItem>
          </SidebarMenu>
        </SidebarGroupContent>
      </Sidebar>
    </SidebarProvider>
  );
};
