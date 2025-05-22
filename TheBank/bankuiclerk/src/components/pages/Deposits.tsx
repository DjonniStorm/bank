import { useDeposits } from '@/hooks/useDeposits';
import { useClerks } from '@/hooks/useClerks';
import { useCurrencies } from '@/hooks/useCurrencies';
import React from 'react';
import { AppSidebar } from '../layout/Sidebar';
import { DataTable, type ColumnDef } from '../layout/DataTable';
import type { DepositBindingModel } from '@/types/types';
import { DialogForm } from '../layout/DialogForm';
import { DepositFormAdd, DepositFormEdit } from '../features/DepositForm';
import { toast } from 'sonner';

type DepositRowData = DepositBindingModel & {
  clerkName: string;
  currenciesDisplay: string;
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
    accessorKey: 'currenciesDisplay',
    header: 'Валюты',
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
  const { currencies, isLoading: isCurrenciesLoading } = useCurrencies();

  const [isAddDialogOpen, setIsAddDialogOpen] = React.useState<boolean>(false);
  const [isEditDialogOpen, setIsEditDialogOpen] =
    React.useState<boolean>(false);
  const [selectedItem, setSelectedItem] = React.useState<
    DepositBindingModel | undefined
  >();

  const finalData = React.useMemo(() => {
    if (!deposits || !clerks || !currencies) return [];

    return deposits.map((deposit) => {
      const clerk = clerks.find((c) => c.id === deposit.clerkId);

      // Формирование списка валют
      const currenciesDisplay =
        deposit.depositCurrencies
          ?.map((dc) => {
            const currency = currencies?.find((c) => c.id === dc.currencyId);
            return currency
              ? `${currency.name} (${currency.abbreviation})`
              : dc.currencyId;
          })
          .join(', ') || 'Нет валют';

      return {
        ...deposit,
        clerkName: clerk ? `${clerk.name} ${clerk.surname}` : 'Неизвестно',
        currenciesDisplay,
      };
    });
  }, [deposits, clerks, currencies]);

  const handleAdd = (data: DepositBindingModel) => {
    console.log('Добавление вклада с данными:', data);
    createDeposit(data);
    setIsAddDialogOpen(false);
  };

  const handleEdit = (data: DepositBindingModel) => {
    if (selectedItem) {
      console.log('Обновление вклада с данными:', { ...selectedItem, ...data });
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

  if (isDepositsLoading || isClerksLoading || isCurrenciesLoading) {
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
