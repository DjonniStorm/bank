import { useClients } from '@/hooks/useClients';
import { useClerks } from '@/hooks/useClerks';
import { useDeposits } from '@/hooks/useDeposits';
import { useCreditPrograms } from '@/hooks/useCreditPrograms';
import React from 'react';
import { DataTable, type ColumnDef } from '../layout/DataTable';
import { AppSidebar } from '../layout/Sidebar';
import type { ClientBindingModel } from '@/types/types';
import { DialogForm } from '../layout/DialogForm';
import { ClientFormAdd, ClientFormEdit } from '../features/ClientForm';
import { toast } from 'sonner';

const columns: ColumnDef<
  ClientBindingModel & {
    clerkName?: string;
    depositsList?: string;
    creditProgramsList?: string;
  }
>[] = [
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
    accessorKey: 'balance',
    header: 'Баланс',
  },
  {
    accessorKey: 'clerkName',
    header: 'Клерк',
  },
  {
    accessorKey: 'depositsList',
    header: 'Вклады',
  },
  {
    accessorKey: 'creditProgramsList',
    header: 'Кредиты',
  },
];

export const Clients = (): React.JSX.Element => {
  const {
    clients,
    isLoading: isClientsLoading,
    error: clientsError,
    updateClient,
    createClient,
  } = useClients();
  const {
    clerks,
    isLoading: isClerksLoading,
    error: clerksError,
  } = useClerks();
  const { deposits, isLoading: isDepositsLoading } = useDeposits();
  const { creditPrograms, isLoading: isCreditProgramsLoading } =
    useCreditPrograms();

  const [isAddDialogOpen, setIsAddDialogOpen] = React.useState<boolean>(false);
  const [isEditDialogOpen, setIsEditDialogOpen] =
    React.useState<boolean>(false);
  const [selectedItem, setSelectedItem] = React.useState<
    ClientBindingModel | undefined
  >();

  const finalData = React.useMemo(() => {
    if (!clients || !clerks || !deposits || !creditPrograms) return [];

    return clients.map((client) => {
      const clerk = clerks.find((c) => c.id === client.clerkId);

      // Находим вклады клиента
      const clientDeposits = deposits?.filter(() => {
        // Учитывая, что мы удалили depositClients из модели, эта проверка будет всегда возвращать false
        // Здесь нужно реализовать другой способ связи, или просто удалить эту функциональность
        return false; // Больше не можем определить связь через deposit.depositClients
      });

      // Находим кредитные программы клиента
      const clientCreditPrograms = creditPrograms.filter((creditProgram) =>
        client.creditProgramClients?.some(
          (cpc) => cpc.creditProgramId === creditProgram.id,
        ),
      );

      // Формируем строки с информацией о вкладах и кредитах
      const depositsList =
        clientDeposits.length > 0
          ? clientDeposits
              .map((d) => `${d.interestRate}% (${d.period} мес.)`)
              .join(', ')
          : 'Нет вкладов';

      const creditProgramsList =
        clientCreditPrograms.length > 0
          ? clientCreditPrograms.map((cp) => cp.name).join(', ')
          : 'Нет кредитов';

      return {
        ...client,
        clerkName: clerk ? `${clerk.name} ${clerk.surname}` : 'Неизвестно',
        depositsList,
        creditProgramsList,
      };
    });
  }, [clients, clerks, deposits, creditPrograms]);

  const handleAdd = (data: ClientBindingModel) => {
    createClient(data);
    setIsAddDialogOpen(false);
  };

  const handleEdit = (data: ClientBindingModel) => {
    if (selectedItem) {
      updateClient({
        ...selectedItem,
        ...data,
      });
      setIsEditDialogOpen(false);
      setSelectedItem(undefined);
    }
  };

  const handleSelectItem = (id: string | undefined) => {
    const item = clients?.find((p) => p.id === id);
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

  if (
    isClientsLoading ||
    isClerksLoading ||
    isDepositsLoading ||
    isCreditProgramsLoading
  ) {
    return <main className="container mx-auto py-10">Загрузка...</main>;
  }

  if (clientsError || clerksError) {
    return (
      <main className="container mx-auto py-10">
        Ошибка загрузки данных: {clientsError?.message || clerksError?.message}
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
          <DialogForm<ClientBindingModel>
            title="Форма клиентов"
            description="Добавить клиента"
            isOpen={isAddDialogOpen}
            onClose={() => setIsAddDialogOpen(false)}
            onSubmit={handleAdd}
          >
            <ClientFormAdd onSubmit={handleAdd} />
          </DialogForm>
        )}
        {selectedItem && (
          <DialogForm<ClientBindingModel>
            title="Форма клиентов"
            description="Изменить данные"
            isOpen={isEditDialogOpen}
            onClose={() => setIsEditDialogOpen(false)}
            onSubmit={handleEdit}
          >
            <ClientFormEdit
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
