import React from 'react';
import { AppSidebar } from '../layout/Sidebar';
import { DialogForm } from '../layout/DialogForm';
import { DataTable } from '../layout/DataTable';
import { usePeriods } from '@/hooks/usePeriods';
import { useStorekeepers } from '@/hooks/useStorekeepers';
import type { PeriodBindingModel } from '@/types/types';
import type { ColumnDef } from '../layout/DataTable';
import { PeriodFormAdd, PeriodFormEdit } from '../features/PeriodForm';
import { toast } from 'sonner';

interface PeriodTableData extends PeriodBindingModel {
  storekeeperName: string;
}

const columns: ColumnDef<PeriodTableData>[] = [
  {
    accessorKey: 'id',
    header: 'ID',
  },
  {
    accessorKey: 'startTime',
    header: 'Время начала',
    renderCell: (item) => new Date(item.startTime).toLocaleDateString(),
  },
  {
    accessorKey: 'endTime',
    header: 'Время окончания',
    renderCell: (item) => new Date(item.endTime).toLocaleDateString(),
  },
  {
    accessorKey: 'storekeeperName',
    header: 'Кладовщик',
  },
];

export const Periods = (): React.JSX.Element => {
  const { isLoading, isError, error, periods, createPeriod, updatePeriod } =
    usePeriods();
  const { storekeepers } = useStorekeepers();

  const finalData = React.useMemo(() => {
    if (!periods || !storekeepers) return [];

    return periods.map((period) => {
      const storekeeper = storekeepers.find(
        (s) => s.id === period.storekeeperId,
      );
      const storekeeperName = storekeeper
        ? [storekeeper.surname, storekeeper.name, storekeeper.middleName]
            .filter(Boolean)
            .join(' ') || 'Неизвестный кладовщик'
        : 'Неизвестный кладовщик';

      return {
        ...period,
        storekeeperName,
      };
    });
  }, [periods, storekeepers]);

  const [isAddDialogOpen, setIsAddDialogOpen] = React.useState<boolean>(false);
  const [isEditDialogOpen, setIsEditDialogOpen] =
    React.useState<boolean>(false);
  const [selectedItem, setSelectedItem] = React.useState<
    PeriodBindingModel | undefined
  >();

  const handleAdd = (data: PeriodBindingModel) => {
    createPeriod(data);
    setIsAddDialogOpen(false);
  };

  const handleEdit = (data: PeriodBindingModel) => {
    if (selectedItem) {
      updatePeriod({
        ...selectedItem,
        ...data,
      });
      setIsEditDialogOpen(false);
      setSelectedItem(undefined);
    }
  };

  const handleSelectItem = (id: string | undefined) => {
    const item = periods?.find((p) => p.id === id);
    if (item) {
      setSelectedItem({
        ...item,
        startTime: new Date(item.startTime),
        endTime: new Date(item.endTime),
      });
    } else {
      setSelectedItem(undefined);
    }
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
        {!selectedItem && (
          <DialogForm<PeriodBindingModel>
            title="Форма сроков"
            description="Добавить сроки"
            isOpen={isAddDialogOpen}
            onClose={() => setIsAddDialogOpen(false)}
            onSubmit={handleAdd}
          >
            <PeriodFormAdd />
          </DialogForm>
        )}
        {selectedItem && (
          <DialogForm<PeriodBindingModel>
            title="Форма сроков"
            description="Изменить сроки"
            isOpen={isEditDialogOpen}
            onClose={() => setIsEditDialogOpen(false)}
            onSubmit={handleEdit}
          >
            <PeriodFormEdit defaultValues={selectedItem} />
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
