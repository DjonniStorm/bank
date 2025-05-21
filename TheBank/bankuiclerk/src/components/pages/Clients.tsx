import { useClients } from '@/hooks/useClients';
import { useClerks } from '@/hooks/useClerks';
import React from 'react';
import { DataTable, type ColumnDef } from '../layout/DataTable';
import { AppSidebar } from '../layout/Sidebar';
import type { ClientBindingModel } from '@/types/types';
import { DialogForm } from '../layout/DialogForm';
import { ClientFormAdd, ClientFormEdit } from '../features/ClientForm';
import { toast } from 'sonner';

const columns: ColumnDef<ClientBindingModel>[] = [
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
    accessorKey: 'deposits',
    header: 'Вклады',
  },
  {
    accessorKey: 'creditPrograms',
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

  const [isAddDialogOpen, setIsAddDialogOpen] = React.useState<boolean>(false);
  const [isEditDialogOpen, setIsEditDialogOpen] =
    React.useState<boolean>(false);
  const [selectedItem, setSelectedItem] = React.useState<
    ClientBindingModel | undefined
  >();

  const finalData = React.useMemo(() => {
    if (!clients || !clerks) return [];

    return clients.map((client) => {
      const clerk = clerks.find((c) => c.id === client.clerkId);
      return {
        ...client,
        clerkName: clerk ? `${clerk.name} ${clerk.surname}` : 'Неизвестно',
      };
    });
  }, [clients, clerks]);

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

  if (isClientsLoading || isClerksLoading) {
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
            <ClientFormAdd />
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
              onSubmit={console.log}
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
