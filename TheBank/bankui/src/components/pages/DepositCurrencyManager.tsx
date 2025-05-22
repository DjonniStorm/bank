import React from 'react';
import { useDeposits } from '@/hooks/useDeposits';
import { useCurrencies } from '@/hooks/useCurrencies';
import { useClerks } from '@/hooks/useClerks';
import { DataTable, type ColumnDef } from '../layout/DataTable';
import { AppSidebar } from '../layout/Sidebar';
import { DialogForm } from '../layout/DialogForm';
import { toast } from 'sonner';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Button } from '@/components/ui/button';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { cn } from '@/lib/utils';
import type {
  DepositBindingModel,
  DepositCurrencyBindingModel,
} from '@/types/types';

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

type FormValues = {
  currencyIds: string[];
};

const schema = z.object({
  currencyIds: z.array(z.string()),
});

const DepositCurrencyForm = ({
  onSubmit,
  defaultValues,
}: {
  onSubmit: (data: { currencyIds: string[] }) => void;
  defaultValues: Partial<DepositBindingModel>;
}): React.JSX.Element => {
  const { currencies } = useCurrencies();

  const initialCurrencyIds = React.useMemo(
    () =>
      defaultValues?.depositCurrencies
        ?.map((dc) => dc.currencyId)
        .filter((id): id is string => !!id) || [],
    [defaultValues?.depositCurrencies],
  );

  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      currencyIds: initialCurrencyIds,
    },
  });

  React.useEffect(() => {
    if (defaultValues) {
      form.reset({
        currencyIds: initialCurrencyIds,
      });
    }
  }, [defaultValues, form, initialCurrencyIds]);

  const handleSubmit = (data: FormValues) => {
    onSubmit(data);
  };

  const selectedCurrencyIds = form.watch('currencyIds') || [];

  return (
    <Form {...form}>
      <form
        onSubmit={form.handleSubmit(handleSubmit)}
        className="space-y-4 max-w-md mx-auto p-4"
      >
        <FormField
          control={form.control}
          name="currencyIds"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Валюты</FormLabel>
              <Select
                onValueChange={(value) => {
                  const currentValues = field.value || [];
                  if (!currentValues.includes(value)) {
                    field.onChange([...currentValues, value]);
                  }
                }}
              >
                <FormControl>
                  <SelectTrigger>
                    <SelectValue placeholder="Выберите валюты" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {currencies?.map((currency) => (
                    <SelectItem
                      key={currency.id}
                      value={currency.id || ''}
                      className={cn(
                        selectedCurrencyIds.includes(currency.id || '') &&
                          'bg-muted',
                      )}
                    >
                      {`${currency.name} (${currency.abbreviation})`}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <div className="flex flex-wrap gap-2 mt-2">
                {selectedCurrencyIds.map((currencyId) => {
                  const currency = currencies?.find((c) => c.id === currencyId);
                  return (
                    <div
                      key={currencyId}
                      className="bg-muted px-2 py-1 rounded-md flex items-center gap-1"
                    >
                      <span>
                        {currency
                          ? `${currency.name} (${currency.abbreviation})`
                          : currencyId}
                      </span>
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        className="h-4 w-4 rounded-full"
                        onClick={() => {
                          const newValues = selectedCurrencyIds.filter(
                            (id) => id !== currencyId,
                          );
                          form.setValue('currencyIds', newValues);
                        }}
                      >
                        ×
                      </Button>
                    </div>
                  );
                })}
              </div>
              <FormMessage />
            </FormItem>
          )}
        />
        <Button type="submit" className="w-full">
          Сохранить
        </Button>
      </form>
    </Form>
  );
};

export const DepositCurrencyManager = (): React.JSX.Element => {
  const {
    deposits,
    isLoading: isDepositsLoading,
    error: depositsError,
    updateDeposit,
  } = useDeposits();

  const { currencies, isLoading: isCurrenciesLoading } = useCurrencies();
  const { clerks, isLoading: isClerksLoading } = useClerks();

  const [isEditDialogOpen, setIsEditDialogOpen] =
    React.useState<boolean>(false);
  const [selectedItem, setSelectedItem] = React.useState<
    DepositBindingModel | undefined
  >();

  const finalData = React.useMemo(() => {
    if (!deposits || !currencies || !clerks) return [];

    return deposits.map((deposit) => {
      // Находим клерка по ID
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
        clerkName: clerk
          ? `${clerk.name} ${clerk.surname}`
          : 'Неизвестный клерк',
        currenciesDisplay,
      };
    });
  }, [deposits, currencies, clerks]);

  const handleEdit = (data: { currencyIds: string[] }) => {
    if (selectedItem) {
      // Проверка на дублирование валют
      const uniqueCurrencyIds = new Set(data.currencyIds);
      if (uniqueCurrencyIds.size !== data.currencyIds.length) {
        toast.error(
          'Обнаружены дублирующиеся валюты. Пожалуйста, убедитесь что каждая валюта выбрана только один раз.',
        );
        return;
      }

      // Формируем массив связей, сохраняя существующие ID где это возможно
      const depositCurrencies: DepositCurrencyBindingModel[] =
        data.currencyIds.map((currencyId) => {
          // Ищем существующую связь с этой валютой
          const existingRelation = selectedItem.depositCurrencies?.find(
            (dc) => dc.currencyId === currencyId,
          );

          // Если связь уже существует, возвращаем её с оригинальным ID
          if (existingRelation) {
            return { ...existingRelation };
          }

          // Если это новая связь, создаем объект без ID
          return {
            currencyId,
            depositId: selectedItem.id,
          };
        });

      console.log('Обновляем депозит с данными:', {
        ...selectedItem,
        depositCurrencies,
      });

      // Обновляем вклад, сохраняя все оригинальные поля и связи
      updateDeposit({
        ...selectedItem, // Сохраняем все существующие поля
        depositCurrencies, // Обновляем только связи с валютами
      });

      setIsEditDialogOpen(false);
      setSelectedItem(undefined);
      toast.success('Связи валют с вкладом успешно обновлены');
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
      toast('Выберите вклад для добавления валют');
      return;
    }

    setIsEditDialogOpen(true);
  };

  if (isDepositsLoading || isCurrenciesLoading || isClerksLoading) {
    return <main className="container mx-auto py-10">Загрузка...</main>;
  }

  if (depositsError) {
    return (
      <main className="container mx-auto py-10">
        Ошибка загрузки данных: {depositsError.message}
      </main>
    );
  }

  return (
    <main className="flex-1 flex relative">
      <AppSidebar
        onAddClick={() => {
          toast(
            'Кладовщик не может создавать вклады, только связывать их с валютами',
          );
        }}
        onEditClick={() => {
          openEditForm();
        }}
      />
      <div className="flex-1 p-4">
        {selectedItem && (
          <DialogForm
            title="Управление валютами вклада"
            description="Выберите валюты для связи с вкладом"
            isOpen={isEditDialogOpen}
            onClose={() => setIsEditDialogOpen(false)}
            onSubmit={handleEdit}
          >
            <DepositCurrencyForm
              onSubmit={handleEdit}
              defaultValues={selectedItem}
            />
          </DialogForm>
        )}
        <div className="mb-4">
          <h2 className="text-2xl font-bold">
            Управление связями вкладов и валют
          </h2>
          <p className="text-muted-foreground">
            Кладовщик может связывать существующие вклады с валютами, но не
            может создавать новые вклады.
          </p>
        </div>
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
