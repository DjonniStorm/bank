import { useReplenishments } from '@/hooks/useReplenishments';
import React from 'react';
import { AppSidebar } from '../layout/Sidebar';
import { DataTable, type ColumnDef } from '../layout/DataTable';
import type { ReplenishmentBindingModel } from '@/types/types';

const columns: ColumnDef<ReplenishmentBindingModel>[] = [
  {
    accessorKey: 'id',
    header: 'ID',
  },
  {
    accessorKey: 'amout',
    header: 'Сумма',
  },
  {
    accessorKey: 'date',
    header: 'Стоимость',
    renderCell: (item) => new Date(item.date).toLocaleDateString(),
  },
  {
    accessorKey: 'depositName',
    header: 'Вклад',
  },
  {
    accessorKey: 'clerkName',
    header: 'Клерк',
  },
];

export const Replenishments = (): React.JSX.Element => {
  const { replenishments, isLoading, error } = useReplenishments();

  if (isLoading) {
    return <main className="container mx-auto py-10">Загрузка...</main>;
  }

  if (error) {
    return (
      <main className="container mx-auto py-10">
        Ошибка загрузки данных: {error.message}
      </main>
    );
  }

  return (
    <main className="flex-1 flex relative">
      <AppSidebar
        onAddClick={() => {
          //   setIsAddDialogOpen(true);
        }}
        onEditClick={() => {
          //   openEditForm();
        }}
      />
      <div className="flex-1 p-4">
        <div>
          <DataTable
            data={replenishments || []}
            columns={columns}
            onRowSelected={console.log}
          />
        </div>
      </div>
    </main>
  );
};
