import React, { useEffect } from 'react';
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
import type { CurrencyBindingModel } from '@/types/types';
import { useAuthStore } from '@/store/workerStore';

type BaseFormValues = {
  id?: string;
  name: string;
  abbreviation: string;
  cost: number;
};

type EditFormValues = {
  id?: string;
  name?: string;
  abbreviation?: string;
  cost?: number;
};

const baseSchema = z.object({
  id: z.string().optional(),
  name: z.string({ message: 'Введите название' }),
  abbreviation: z.string({ message: 'Введите аббревиатуру' }),
  cost: z.coerce.number({ message: 'Введите стоимость' }),
});

const addSchema = baseSchema;

const editSchema = z.object({
  id: z.string().optional(),
  name: z.string().min(1, 'Укажите название валюты').optional(),
  abbreviation: z.string().min(1, 'Укажите аббревиатуру').optional(),
  cost: z.coerce
    .number()
    .min(0, 'Стоимость не может быть отрицательной')
    .optional(),
});

interface BaseCurrencyFormProps {
  onSubmit: (data: CurrencyBindingModel) => void;
  schema: z.ZodType<BaseFormValues | EditFormValues>;
  defaultValues?: Partial<BaseFormValues | EditFormValues>;
}

const BaseCurrencyForm = ({
  onSubmit,
  schema,
  defaultValues,
}: BaseCurrencyFormProps): React.JSX.Element => {
  const form = useForm<BaseFormValues | EditFormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      id: defaultValues?.id || '',
      name: defaultValues?.name || '',
      abbreviation: defaultValues?.abbreviation || '',
      cost: defaultValues?.cost || 0,
    },
  });

  useEffect(() => {
    if (defaultValues) {
      form.reset({
        id: defaultValues.id || '',
        name: defaultValues.name || '',
        abbreviation: defaultValues.abbreviation || '',
        cost: defaultValues.cost || 0,
      });
    }
  }, [defaultValues, form]);

  const storekeeper = useAuthStore((store) => store.user);

  const handleSubmit = (data: BaseFormValues | EditFormValues) => {
    // Если это форма редактирования, используем только заполненные поля
    const payload: CurrencyBindingModel = {
      id: data.id || crypto.randomUUID(),
      storekeeperId: storekeeper?.id,
      name: 'name' in data && data.name !== undefined ? data.name : '',
      abbreviation:
        'abbreviation' in data && data.abbreviation !== undefined
          ? data.abbreviation
          : '',
      cost: 'cost' in data && data.cost !== undefined ? data.cost : 0,
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
          name="name"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Название</FormLabel>
              <FormControl>
                <Input placeholder="Например, Доллар США" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="abbreviation"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Аббревиатура</FormLabel>
              <FormControl>
                <Input placeholder="Например, USD" {...field} />
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
                <Input type="number" placeholder="Например, 1.0" {...field} />
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

export const CurrencyFormAdd = ({
  onSubmit,
}: {
  onSubmit: (data: CurrencyBindingModel) => void;
}): React.JSX.Element => {
  return <BaseCurrencyForm onSubmit={onSubmit} schema={addSchema} />;
};

export const CurrencyFormEdit = ({
  onSubmit,
  defaultValues,
}: {
  onSubmit: (data: CurrencyBindingModel) => void;
  defaultValues: Partial<BaseFormValues>;
}): React.JSX.Element => {
  return (
    <BaseCurrencyForm
      onSubmit={onSubmit}
      schema={editSchema}
      defaultValues={defaultValues}
    />
  );
};
