import React from 'react';
import { AppSidebar } from '../layout/Sidebar';
import { useCreditPrograms } from '@/hooks/useCreditPrograms';
import { DialogForm } from '../layout/DialogForm';
import { DataTable } from '../layout/DataTable';
import { CreditProgramForm } from '../features/CreditProgramForm';
import type { CreditProgramBindingModel } from '@/types/types';
import type { ColumnDef } from '../layout/DataTable';

const columns: ColumnDef<CreditProgramBindingModel>[] = [
  {
    accessorKey: 'id',
    header: 'ID',
  },
  {
    accessorKey: 'name',
    header: 'Название',
  },
  {
    accessorKey: 'cost',
    header: 'Стоимость',
  },
  {
    accessorKey: 'maxCost',
    header: 'Макс. стоимость',
  },
  {
    accessorKey: 'storekeeperId',
    header: 'ID Кладовщика',
  },
  {
    accessorKey: 'periodId',
    header: 'ID Периода',
  },
];

export const CreditPrograms = (): React.JSX.Element => {
  const {
    isLoading,
    isError,
    error,
    creditPrograms,
    createCreditProgram,
    updateCreditProgram,
  } = useCreditPrograms();

  const [isDialogOpen, setIsDialogOpen] = React.useState<boolean>(false);

  const handleAdd = (data: CreditProgramBindingModel) => {
    console.log(data);
    createCreditProgram(data);
  };

  if (isLoading) {
    return <main className="container mx-auto py-10">Загрузка...</main>;
  }

  if (isError) {
    return (
      <main className="container mx-auto py-10">
        Ошибка загрузки: {error?.message}
      </main>
    );
  }

  return (
    <main className="flex-1 flex relative">
      <AppSidebar
        onAddClick={() => {
          setIsDialogOpen(true);
        }}
        onEditClick={function (): void {}}
      />

      <div className="flex-1 p-4">
        <DialogForm<CreditProgramBindingModel>
          title="Форма"
          description="Описание"
          isOpen={isDialogOpen}
          onClose={() => setIsDialogOpen(false)}
          onSubmit={handleAdd}
          children={<CreditProgramForm />}
        />
        <div className="">
          <DataTable data={creditPrograms || []} columns={columns} />
        </div>
      </div>
    </main>
  );
};
