import { useReplenishments } from '@/hooks/useReplenishments';
import { useClerks } from '@/hooks/useClerks';
import { useDeposits } from '@/hooks/useDeposits';
import React from 'react';
import { AppSidebar } from '../layout/Sidebar';
import { DataTable, type ColumnDef } from '../layout/DataTable';
import type { ReplenishmentBindingModel } from '@/types/types';
import { DialogForm } from '../layout/DialogForm';
import {
  ReplenishmentFormAdd,
  ReplenishmentFormEdit,
} from '../features/ReplenishmentForm';
import { toast } from 'sonner';

type ReplenishmentRowData = ReplenishmentBindingModel & {
  clerkName: string;
  depositDisplay: string;
};

const columns: ColumnDef<ReplenishmentRowData>[] = [
  {
    accessorKey: 'id',
    header: 'ID',
  },
  {
    accessorKey: 'amount',
    header: 'Сумма',
  },
  {
    accessorKey: 'date',
    header: 'Дата',
    renderCell: (item) => new Date(item.date).toLocaleDateString(),
  },
  {
    accessorKey: 'depositDisplay',
    header: 'Вклад',
  },
  {
    accessorKey: 'clerkName',
    header: 'Клерк',
  },
];

export const Replenishments = (): React.JSX.Element => {
  const {
    replenishments,
    isLoading: isReplenishmentsLoading,
    error: replenishmentsError,
    createReplenishment,
    updateReplenishment,
  } = useReplenishments();
  const {
    clerks,
    isLoading: isClerksLoading,
    error: clerksError,
  } = useClerks();
  const {
    deposits,
    isLoading: isDepositsLoading,
    error: depositsError,
  } = useDeposits();

  const [isAddDialogOpen, setIsAddDialogOpen] = React.useState<boolean>(false);
  const [isEditDialogOpen, setIsEditDialogOpen] =
    React.useState<boolean>(false);
  const [selectedItem, setSelectedItem] = React.useState<
    ReplenishmentBindingModel | undefined
  >();

  const finalData = React.useMemo(() => {
    if (!replenishments || !clerks || !deposits) return [];

    return replenishments.map((replenishment) => {
      const clerk = clerks.find((c) => c.id === replenishment.clerkId);
      const deposit = deposits.find((d) => d.id === replenishment.depositId);

      return {
        ...replenishment,
        clerkName: clerk ? `${clerk.name} ${clerk.surname}` : 'Неизвестно',
        depositDisplay: deposit
          ? `Вклад ${deposit.interestRate}% - ${deposit.cost}₽`
          : 'Неизвестно',
      };
    });
  }, [replenishments, clerks, deposits]);

  const handleAdd = (data: ReplenishmentBindingModel) => {
    createReplenishment(data);
    setIsAddDialogOpen(false);
  };

  const handleEdit = (data: ReplenishmentBindingModel) => {
    if (selectedItem) {
      updateReplenishment({
        ...selectedItem,
        ...data,
      });
      setIsEditDialogOpen(false);
      setSelectedItem(undefined);
    }
  };

  const handleSelectItem = (id: string | undefined) => {
    const item = replenishments?.find((p) => p.id === id);
    if (item) {
      setSelectedItem({
        ...item,
      });
    } else {
      setSelectedItem(undefined);
    }
  };

  const openEditForm = () => {
    if (!selectedItem) {
      toast('Выберите элемент для редактирования');
      return;
    }

    setIsEditDialogOpen(true);
  };

  if (isReplenishmentsLoading || isClerksLoading || isDepositsLoading) {
    return <main className="container mx-auto py-10">Загрузка...</main>;
  }

  if (replenishmentsError || clerksError || depositsError) {
    return (
      <main className="container mx-auto py-10">
        Ошибка загрузки данных:{' '}
        {replenishmentsError?.message ||
          clerksError?.message ||
          depositsError?.message}
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
        {!selectedItem && (
          <DialogForm<ReplenishmentBindingModel>
            title="Форма пополнений"
            description="Добавить пополнение"
            isOpen={isAddDialogOpen}
            onClose={() => setIsAddDialogOpen(false)}
            onSubmit={handleAdd}
          >
            <ReplenishmentFormAdd />
          </DialogForm>
        )}
        {selectedItem && (
          <DialogForm<ReplenishmentBindingModel>
            title="Форма пополнений"
            description="Изменить данные"
            isOpen={isEditDialogOpen}
            onClose={() => setIsEditDialogOpen(false)}
            onSubmit={handleEdit}
          >
            <ReplenishmentFormEdit defaultValues={selectedItem} />
          </DialogForm>
        )}
        <div>
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
