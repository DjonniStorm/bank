import React from 'react';
import { AppSidebar } from '../layout/Sidebar';
import { DialogForm } from '../layout/DialogForm';
import { DataTable } from '../layout/DataTable';
import { useCurrencies } from '@/hooks/useCurrencies';
import { useStorekeepers } from '@/hooks/useStorekeepers';
import type { CurrencyBindingModel } from '@/types/types';
import type { ColumnDef } from '../layout/DataTable';
import { CurrencyFormAdd, CurrencyFormEdit } from '../features/CurrencyForm';
import { toast } from 'sonner';

interface CurrencyTableData extends CurrencyBindingModel {
  storekeeperName: string;
}

const columns: ColumnDef<CurrencyTableData>[] = [
  {
    accessorKey: 'id',
    header: 'ID',
  },
  {
    accessorKey: 'name',
    header: 'Название',
  },
  {
    accessorKey: 'abbreviation',
    header: 'Аббревиатура',
  },
  {
    accessorKey: 'cost',
    header: 'Стоимость',
  },
  {
    accessorKey: 'storekeeperName',
    header: 'Кладовщик',
  },
];

export const Currencies = (): React.JSX.Element => {
  const {
    isLoading,
    isError,
    error,
    currencies,
    createCurrency,
    updateCurrency,
  } = useCurrencies();
  const { storekeepers } = useStorekeepers();

  const finalData = React.useMemo(() => {
    if (!currencies || !storekeepers) return [];

    return currencies.map((currency) => {
      const storekeeper = storekeepers.find(
        (s) => s.id === currency.storekeeperId,
      );
      const storekeeperName = storekeeper
        ? [storekeeper.surname, storekeeper.name, storekeeper.middleName]
            .filter(Boolean)
            .join(' ') || 'Неизвестный кладовщик'
        : 'Неизвестный кладовщик';

      return {
        ...currency,
        storekeeperName,
      };
    });
  }, [currencies, storekeepers]);

  const [isAddDialogOpen, setIsAddDialogOpen] = React.useState<boolean>(false);
  const [isEditDialogOpen, setIsEditDialogOpen] =
    React.useState<boolean>(false);
  const [selectedItem, setSelectedItem] = React.useState<
    CurrencyBindingModel | undefined
  >();

  const handleAdd = (data: CurrencyBindingModel) => {
    createCurrency(data);
    setIsAddDialogOpen(false);
  };

  const handleEdit = (data: CurrencyBindingModel) => {
    if (selectedItem) {
      updateCurrency({
        ...selectedItem,
        ...data,
      });
      setIsEditDialogOpen(false);
      setSelectedItem(undefined);
    }
  };

  const handleSelectItem = (id: string | undefined) => {
    const item = currencies?.find((c) => c.id === id);
    setSelectedItem(item);
  };

  const openEditForm = () => {
    if (!selectedItem) {
      toast('Выберите элемент для редактирования');
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
        <DialogForm<CurrencyBindingModel>
          title="Форма валюты"
          description="Добавьте новую валюту"
          isOpen={isAddDialogOpen}
          onClose={() => setIsAddDialogOpen(false)}
        >
          <CurrencyFormAdd onSubmit={handleAdd} />
        </DialogForm>
        {selectedItem && (
          <DialogForm<CurrencyBindingModel>
            title="Форма валюты"
            description="Измените валюту"
            isOpen={isEditDialogOpen}
            onClose={() => setIsEditDialogOpen(false)}
            onSubmit={handleEdit}
          >
            <CurrencyFormEdit defaultValues={selectedItem} />
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
