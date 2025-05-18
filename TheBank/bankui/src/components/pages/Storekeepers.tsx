import React from 'react';
import { DataTable } from '../layout/DataTable';
import type { ColumnDef } from '../layout/DataTable';
import { useStorekeepers } from '@/hooks/useStorekeepers';
import type { StorekeeperBindingModel } from '@/types/types';

const columns: ColumnDef<StorekeeperBindingModel>[] = [
  {
    accessorKey: 'id',
    header: 'ID',
  },
  {
    accessorKey: 'name',
    header: 'Имя',
  },
  {
    accessorKey: 'surname',
    header: 'Фамилия',
  },
  {
    accessorKey: 'middleName',
    header: 'Отчество',
  },
  {
    accessorKey: 'login',
    header: 'Логин',
  },
  {
    accessorKey: 'email',
    header: 'Email',
  },
  {
    accessorKey: 'phoneNumber',
    header: 'Телефон',
  },
];

export const Storekeepers = (): React.JSX.Element => {
  const { storekeepers, isLoading, error } = useStorekeepers();

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
    <main className="container mx-auto py-10">
      <h1 className="text-2xl font-bold mb-6">Кладовщики</h1>
      <DataTable data={storekeepers || []} columns={columns} />
    </main>
  );
};
