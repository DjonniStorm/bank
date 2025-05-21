import { useDeposits } from '@/hooks/useDeposits';
import { useClerks } from '@/hooks/useClerks';
import React from 'react';
import { AppSidebar } from '../layout/Sidebar';
import { DataTable, type ColumnDef } from '../layout/DataTable';
import type { DepositBindingModel, ClientBindingModel } from '@/types/types';
import { DialogForm } from '../layout/DialogForm';
import { DepositFormAdd, DepositFormEdit } from '../features/DepositForm';
import { toast } from 'sonner';

type DepositRowData = DepositBindingModel & {
  clerkName: string;
  clientsDisplay: string;
};

const columns: ColumnDef<DepositRowData>[] = [
  {
    accessorKey: 'id',
    header: 'ID',
  },
  {
    accessorKey: 'interestRate',
    header: 'Процентная ставка',
  },
  {
    accessorKey: 'cost',
    header: 'Стоимость',
  },
  {
    accessorKey: 'period',
    header: 'Срок вклада',
  },
  {
    accessorKey: 'clerkName',
    header: 'Клерк',
  },
  {
    accessorKey: 'clientsDisplay',
    header: 'Клиенты',
  },
];

export const Deposits = (): React.JSX.Element => {
  const {
    deposits,
    isLoading: isDepositsLoading,
    error: depositsError,
    createDeposit,
    updateDeposit,
  } = useDeposits();
  const {
    clerks,
    isLoading: isClerksLoading,
    error: clerksError,
  } = useClerks();

  const [isAddDialogOpen, setIsAddDialogOpen] = React.useState<boolean>(false);
  const [isEditDialogOpen, setIsEditDialogOpen] =
    React.useState<boolean>(false);
  const [selectedItem, setSelectedItem] = React.useState<
    DepositBindingModel | undefined
  >();

  const finalData = React.useMemo(() => {
    if (!deposits || !clerks) return [];

    return deposits.map((deposit) => {
      const clerk = clerks.find((c) => c.id === deposit.clerkId);
      const clientsDisplay =
        deposit.depositClients
          ?.map((dc) => {
            const client = clerks?.find((c) => c.id === dc.clientId);
            return client ? `${client.name} ${client.surname}` : dc.clientId;
          })
          .join(', ') || 'Нет клиентов';

      return {
        ...deposit,
        clerkName: clerk ? `${clerk.name} ${clerk.surname}` : 'Неизвестно',
        clientsDisplay: clientsDisplay,
      };
    });
  }, [deposits, clerks]);

  const handleAdd = (data: DepositBindingModel) => {
    createDeposit(data);
    setIsAddDialogOpen(false);
  };

  const handleEdit = (data: DepositBindingModel) => {
    if (selectedItem) {
      updateDeposit({
        ...selectedItem,
        ...data,
      });
      setIsEditDialogOpen(false);
      setSelectedItem(undefined);
    }
  };

  const handleSelectItem = (id: string | undefined) => {
    const item = deposits?.find((p) => p.id === id);
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

  if (isDepositsLoading || isClerksLoading) {
    return <main className="container mx-auto py-10">Загрузка...</main>;
  }

  if (depositsError || clerksError) {
    return (
      <main className="container mx-auto py-10">
        Ошибка загрузки данных: {depositsError?.message || clerksError?.message}
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
          <DialogForm<DepositBindingModel>
            title="Форма вкладов"
            description="Добавить вклад"
            isOpen={isAddDialogOpen}
            onClose={() => setIsAddDialogOpen(false)}
            onSubmit={handleAdd}
          >
            <DepositFormAdd onSubmit={handleAdd} />
          </DialogForm>
        )}
        {selectedItem && (
          <DialogForm<DepositBindingModel>
            title="Форма вкладов"
            description="Изменить данные"
            isOpen={isEditDialogOpen}
            onClose={() => setIsEditDialogOpen(false)}
            onSubmit={handleEdit}
          >
            <DepositFormEdit
              onSubmit={handleEdit}
              defaultValues={selectedItem}
            />
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
