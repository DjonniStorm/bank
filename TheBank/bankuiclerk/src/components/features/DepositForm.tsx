import React from 'react';
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
import type { DepositBindingModel } from '@/types/types';
import { useAuthStore } from '@/store/workerStore';

type BaseFormValues = {
  id?: string;
  interestRate: number;
  cost: number;
  period: number;
};

type EditFormValues = {
  id?: string;
  interestRate?: number;
  cost?: number;
  period?: number;
};

const baseSchema = z.object({
  id: z.string().optional(),
  interestRate: z.coerce
    .number()
    .min(0, 'Процентная ставка не может быть отрицательной'),
  cost: z.coerce.number().min(0, 'Стоимость не может быть отрицательной'),
  period: z.coerce.number().int().min(1, 'Срок вклада должен быть не менее 1'),
});

const addSchema = baseSchema;

const editSchema = z.object({
  id: z.string().optional(),
  interestRate: z.coerce
    .number()
    .min(0, 'Процентная ставка не может быть отрицательной')
    .optional(),
  cost: z.coerce
    .number()
    .min(0, 'Стоимость не может быть отрицательной')
    .optional(),
  period: z.coerce
    .number()
    .int()
    .min(1, 'Срок вклада должен быть не менее 1')
    .optional(),
});

interface BaseDepositFormProps {
  onSubmit: (data: DepositBindingModel) => void;
  schema: z.ZodType<BaseFormValues | EditFormValues>;
  defaultValues?: Partial<DepositBindingModel>;
}

const BaseDepositForm = ({
  onSubmit,
  schema,
  defaultValues,
}: BaseDepositFormProps): React.JSX.Element => {
  const form = useForm<BaseFormValues | EditFormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      id: defaultValues?.id || '',
      interestRate: defaultValues?.interestRate || 0,
      cost: defaultValues?.cost || 0,
      period: defaultValues?.period || 1,
    },
  });

  React.useEffect(() => {
    if (defaultValues) {
      form.reset({
        id: defaultValues.id || '',
        interestRate: defaultValues.interestRate || 0,
        cost: defaultValues.cost || 0,
        period: defaultValues.period || 1,
      });
    }
  }, [defaultValues, form]);

  const clerk = useAuthStore((store) => store.user);

  const handleSubmit = (data: BaseFormValues | EditFormValues) => {
    const depositId = data.id || crypto.randomUUID();

    const payload: DepositBindingModel = {
      id: depositId,
      clerkId: clerk?.id,
      interestRate:
        'interestRate' in data && data.interestRate !== undefined
          ? data.interestRate
          : 0,
      cost: 'cost' in data && data.cost !== undefined ? data.cost : 0,
      period: 'period' in data && data.period !== undefined ? data.period : 1,
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
          name="interestRate"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Процентная ставка</FormLabel>
              <FormControl>
                <Input
                  type="number"
                  placeholder="Введите процентную ставку"
                  {...field}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="cost"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Стоимость</FormLabel>
              <FormControl>
                <Input
                  type="number"
                  placeholder="Введите стоимость"
                  {...field}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="period"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Срок вклада (месяцы)</FormLabel>
              <FormControl>
                <Input
                  type="number"
                  placeholder="Введите срок вклада"
                  {...field}
                />
              </FormControl>
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

export const DepositFormAdd = ({
  onSubmit,
}: {
  onSubmit: (data: DepositBindingModel) => void;
}): React.JSX.Element => {
  return <BaseDepositForm onSubmit={onSubmit} schema={addSchema} />;
};

export const DepositFormEdit = ({
  onSubmit,
  defaultValues,
}: {
  onSubmit: (data: DepositBindingModel) => void;
  defaultValues: Partial<DepositBindingModel>;
}): React.JSX.Element => {
  return (
    <BaseDepositForm
      onSubmit={onSubmit}
      schema={editSchema}
      defaultValues={defaultValues}
    />
  );
};
