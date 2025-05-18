import React from 'react';
import { AppSidebar } from '../layout/Sidebar';
import { DialogForm } from '../layout/DialogForm';
import { DataTable } from '../layout/DataTable';
import { usePeriods } from '@/hooks/usePeriods';
import { useStorekeepers } from '@/hooks/useStorekeepers';
import type {
  PeriodBindingModel,
  StorekeeperBindingModel,
} from '@/types/types';
import type { ColumnDef } from '../layout/DataTable';
import { PeriodForm } from '../features/PeriodForm';

// Определяем расширенный тип для данных таблицы
interface PeriodTableData extends PeriodBindingModel {
  storekeeperName: string;
}

// Определяем столбцы
const columns: ColumnDef<PeriodTableData>[] = [
  {
    accessorKey: 'id',
    header: 'ID',
  },
  {
    accessorKey: 'startTime',
    header: 'Время начала',
  },
  {
    accessorKey: 'endTime',
    header: 'Время окончания',
  },
  {
    accessorKey: 'storekeeperName',
    header: 'Кладовщик',
  },
];

export const Periods = (): React.JSX.Element => {
  const { isLoading, isError, error, periods, createPeriod } = usePeriods();
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

  const [isDialogOpen, setIsDialogOpen] = React.useState<boolean>(false);

  const handleAdd = (data: PeriodBindingModel) => {
    console.log(data);
    createPeriod(data);
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
        onEditClick={() => {}}
      />
      <div className="flex-1 p-4">
        <DialogForm<PeriodBindingModel>
          title="Форма"
          description="Описание"
          isOpen={isDialogOpen}
          onClose={() => setIsDialogOpen(false)}
          onSubmit={handleAdd}
        >
          <PeriodForm />
        </DialogForm>
        <div>
          <DataTable data={finalData} columns={columns} />
        </div>
      </div>
    </main>
  );
};
