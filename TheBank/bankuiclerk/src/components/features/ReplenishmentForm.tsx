import React, { useEffect, useMemo } from 'react';
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
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import type { ReplenishmentBindingModel } from '@/types/types';
import { useAuthStore } from '@/store/workerStore';
import { useDeposits } from '@/hooks/useDeposits';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { format } from 'date-fns';
import { CalendarIcon } from 'lucide-react';
import { cn } from '@/lib/utils';
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/components/ui/popover';
import { Calendar } from '@/components/ui/calendar';

type BaseFormValues = {
  id?: string;
  amount: number;
  date: Date;
  depositId: string;
};

type EditFormValues = {
  id?: string;
  amount?: number;
  date?: Date;
  depositId?: string;
};

const baseSchema = z.object({
  id: z.string().optional(),
  amount: z.coerce.number().min(0.01, 'Сумма пополнения должна быть больше 0'),
  date: z.date({
    required_error: 'Укажите дату пополнения',
    invalid_type_error: 'Неверный формат даты',
  }),
  depositId: z.string().min(1, 'Выберите вклад'),
});

const addSchema = baseSchema;

const editSchema = z.object({
  id: z.string().optional(),
  amount: z.coerce
    .number()
    .min(0.01, 'Сумма пополнения должна быть больше 0')
    .optional(),
  date: z
    .date({
      required_error: 'Укажите дату пополнения',
      invalid_type_error: 'Неверный формат даты',
    })
    .optional(),
  depositId: z.string().min(1, 'Выберите вклад').optional(),
});

interface BaseReplenishmentFormProps {
  onSubmit: (data: ReplenishmentBindingModel) => void;
  schema: z.ZodType<BaseFormValues | EditFormValues>;
  defaultValues?: Partial<ReplenishmentBindingModel>;
}

const BaseReplenishmentForm = ({
  onSubmit,
  schema,
  defaultValues,
}: BaseReplenishmentFormProps): React.JSX.Element => {
  const { deposits } = useDeposits();

  const form = useForm<BaseFormValues | EditFormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      id: defaultValues?.id || '',
      amount: defaultValues?.amount || 0,
      date: defaultValues?.date ? new Date(defaultValues.date) : new Date(), // Ensure Date object
      depositId: defaultValues?.depositId || '',
    },
  });

  useEffect(() => {
    if (defaultValues) {
      form.reset({
        id: defaultValues.id || '',
        amount: defaultValues.amount || 0,
        date: defaultValues.date ? new Date(defaultValues.date) : new Date(), // Ensure Date object
        depositId: defaultValues.depositId || '',
      });
    }
  }, [defaultValues, form]);

  const clerk = useAuthStore((store) => store.user);

  const handleSubmit = (data: BaseFormValues | EditFormValues) => {
    const payload: ReplenishmentBindingModel = {
      id: data.id || crypto.randomUUID(),
      clerkId: clerk?.id,
      amount: 'amount' in data && data.amount !== undefined ? data.amount : 0,
      date: 'date' in data && data.date !== undefined ? data.date : new Date(),
      depositId:
        'depositId' in data && data.depositId !== undefined
          ? data.depositId
          : '',
    };

    onSubmit(payload);
  };

  return (
    <Form {...form}>
      <form
        onSubmit={form.handleSubmit(handleSubmit)}
        className="space-y-4 max-w-md mx-auto p-4"
      >
        <FormField
          control={form.control}
          name="id"
          render={({ field }) => <input type="hidden" {...field} />}
        />
        <FormField
          control={form.control}
          name="amount"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Сумма</FormLabel>
              <FormControl>
                <Input
                  type="number"
                  placeholder="Введите сумму пополнения"
                  {...field}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="date"
          render={({ field }) => (
            <FormItem className="flex flex-col">
              <FormLabel>Дата пополнения</FormLabel>
              <Popover>
                <PopoverTrigger asChild>
                  <FormControl>
                    <Button
                      variant={'outline'}
                      className={cn(
                        'w-full pl-3 text-left font-normal',
                        !field.value && 'text-muted-foreground',
                      )}
                    >
                      {field.value ? (
                        format(field.value, 'PPP')
                      ) : (
                        <span>Выберите дату</span>
                      )}
                      <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                    </Button>
                  </FormControl>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <Calendar
                    mode="single"
                    selected={field.value}
                    onSelect={field.onChange}
                    initialFocus
                  />
                </PopoverContent>
              </Popover>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="depositId"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Вклад</FormLabel>
              <Select onValueChange={field.onChange} value={field.value || ''}>
                <FormControl>
                  <SelectTrigger>
                    <SelectValue placeholder="Выберите вклад" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {deposits?.map((deposit) => (
                    <SelectItem key={deposit.id} value={deposit.id || ''}>
                      {`Вклад ${deposit.interestRate}% - ${deposit.cost}₽`}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
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

export const ReplenishmentFormAdd = ({
  onSubmit,
}: {
  onSubmit: (data: ReplenishmentBindingModel) => void;
}): React.JSX.Element => {
  return <BaseReplenishmentForm onSubmit={onSubmit} schema={addSchema} />;
};

export const ReplenishmentFormEdit = ({
  onSubmit,
  defaultValues,
}: {
  onSubmit: (data: ReplenishmentBindingModel) => void;
  defaultValues: Partial<ReplenishmentBindingModel>;
}): React.JSX.Element => {
  return (
    <BaseReplenishmentForm
      onSubmit={onSubmit}
      schema={editSchema}
      defaultValues={defaultValues}
    />
  );
};



