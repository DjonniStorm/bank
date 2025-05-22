import React from 'react';
import { AppSidebar } from '../layout/Sidebar';
import { useCreditPrograms } from '@/hooks/useCreditPrograms';
import { DialogForm } from '../layout/DialogForm';
import { DataTable } from '../layout/DataTable';
import {
  CreditProgramFormAdd,
  CreditProgramFormEdit,
} from '../features/CreditProgramForm';
import type { CreditProgramBindingModel } from '@/types/types';
import type { ColumnDef } from '../layout/DataTable';
import { toast } from 'sonner';
import { usePeriods } from '@/hooks/usePeriods';
import { useStorekeepers } from '@/hooks/useStorekeepers';

interface CreditProgramTableData extends CreditProgramBindingModel {
  formattedPeriod: string;
  storekeeperFullName: string;
}

const columns: ColumnDef<CreditProgramTableData>[] = [
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
    accessorKey: 'storekeeperFullName',
    header: 'Кладовщик',
  },
  {
    accessorKey: 'formattedPeriod',
    header: 'Период',
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
  const { periods } = usePeriods();
  const { storekeepers } = useStorekeepers();

  const finalData = React.useMemo(() => {
    if (!creditPrograms || !periods || !storekeepers) return [];

    return creditPrograms.map((program) => {
      const period = periods?.find((p) => p.id === program.periodId);
      const storekeeper = storekeepers?.find(
        (s) => s.id === program.storekeeperId,
      );

      const formattedPeriod = period
        ? `${new Date(period.startTime).toLocaleDateString()} - ${new Date(
            period.endTime,
          ).toLocaleDateString()}`
        : 'Неизвестный период';

      const storekeeperFullName = storekeeper
        ? [storekeeper.surname, storekeeper.name, storekeeper.middleName]
            .filter(Boolean)
            .join(' ') || 'Неизвестный кладовщик'
        : 'Неизвестный кладовщик';

      return {
        ...program,
        formattedPeriod,
        storekeeperFullName,
      };
    });
  }, [creditPrograms, periods, storekeepers]);

  const [isAddDialogOpen, setIsAddDialogOpen] = React.useState<boolean>(false);
  const [isEditDialogOpen, setIsEditDialogOpen] =
    React.useState<boolean>(false);
  const [selectedItem, setSelectedItem] = React.useState<
    CreditProgramBindingModel | undefined
  >();

  const handleAdd = (data: CreditProgramBindingModel) => {
    console.log('add', data);
    createCreditProgram(data);
    setIsAddDialogOpen(false);
  };

  const handleEdit = (data: CreditProgramBindingModel) => {
    if (selectedItem) {
      updateCreditProgram({
        ...selectedItem,
        ...data,
      });
      console.log('edit', data);
      setIsEditDialogOpen(false);
      setSelectedItem(undefined);
    }
  };

  const handleSelectItem = (id: string | undefined) => {
    const item = creditPrograms?.find((cp) => cp.id === id);
    setSelectedItem(item);
  };

  const openEditForm = () => {
    if (!selectedItem) {
      toast.error('Выберите элемент для редактирования');
      return;
    }

    setIsEditDialogOpen(true);
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
          setIsAddDialogOpen(true);
        }}
        onEditClick={() => {
          openEditForm();
        }}
      />

      <div className="flex-1 p-4">
        <DialogForm<CreditProgramBindingModel>
          title="Форма кредитной программы"
          description="Добавить новую кредитную программу"
          isOpen={isAddDialogOpen}
          onClose={() => setIsAddDialogOpen(false)}
          onSubmit={handleAdd}
        >
          <CreditProgramFormAdd />
        </DialogForm>
        {selectedItem && (
          <DialogForm<CreditProgramBindingModel>
            title="Форма кредитной программы"
            description="Изменить кредитную программу"
            isOpen={isEditDialogOpen}
            onClose={() => setIsEditDialogOpen(false)}
            onSubmit={handleEdit}
          >
            <CreditProgramFormEdit defaultValues={selectedItem} />
          </DialogForm>
        )}
        <div className="">
          <DataTable
            data={finalData}
            columns={columns}
            onRowSelected={(id) => handleSelectItem(id)}
            selectedRow={selectedItem?.id}
          />
        </div>
      </div>
    </main>
  );
};
