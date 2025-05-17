import React from 'react';
import { AppSidebar } from '../layout/Sidebar';
import { useCreditPrograms } from '@/hooks/useCreditPrograms';
import { DialogForm } from '../layout/DialogForm';
import { DataTable } from '../layout/DataTable';
import { CreditProgramForm } from '../features/CreditProgramForm';
import type { CreditProgramBindingModel } from '@/types/types';

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

  return (
    <main className="flex-1 flex relative">
      <AppSidebar
        onAddClick={() => {
          setIsDialogOpen(true);
        }}
        onEditClick={function (): void {}}
      />

      <div className="flex-1 p-4">
        {isError && (
          <div className="text-red-500">Ошибка загрузки: {error?.message}</div>
        )}

        <DialogForm<CreditProgramBindingModel>
          title="Форма"
          description="Описание"
          isOpen={isDialogOpen}
          onClose={() => setIsDialogOpen(false)}
          onSubmit={handleAdd}
        >
          <CreditProgramForm />
        </DialogForm>
        <div className="">
          <DataTable data={[]} columns={[]} />
        </div>
      </div>
    </main>
  );
};
