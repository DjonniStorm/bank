import { useClerks } from '@/hooks/useClerks';
import React from 'react';
import { DataTable, type ColumnDef } from '../layout/DataTable';
import type { ClerkBindingModel } from '@/types/types';

const columns: ColumnDef<ClerkBindingModel>[] = [
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

export const Clerks = (): React.JSX.Element => {
  const { clerks, isLoading, error } = useClerks();

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
      <h1 className="text-2xl font-bold mb-6">Клерки</h1>
      <DataTable
        data={clerks || []}
        columns={columns}
        onRowSelected={console.log}
      />
    </main>
  );
};
