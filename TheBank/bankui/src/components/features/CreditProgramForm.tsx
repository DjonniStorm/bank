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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Button } from '@/components/ui/button';
import type { CreditProgramBindingModel } from '@/types/types';
import { useAuthStore } from '@/store/workerStore';
import { usePeriods } from '@/hooks/usePeriods';

type BaseFormValues = {
  id?: string;
  name: string;
  cost: number;
  maxCost: number;
  periodId: string;
};

type EditFormValues = Partial<BaseFormValues>;

const baseSchema = z.object({
  id: z.string().optional(),
  name: z.string().min(1, 'Название обязательно'),
  cost: z.coerce.number().min(0, 'Стоимость не может быть отрицательной'),
  maxCost: z.coerce
    .number()
    .min(0, 'Максимальная стоимость не может быть отрицательной'),
  periodId: z.string().min(1, 'Выберите период'),
});

const addSchema = baseSchema;

const editSchema = z.object({
  id: z.string().optional(),
  name: z.string().min(1, 'Название обязательно').optional(),
  cost: z.coerce
    .number()
    .min(0, 'Стоимость не может быть отрицательной')
    .optional(),
  maxCost: z.coerce
    .number()
    .min(0, 'Максимальная стоимость не может быть отрицательной')
    .optional(),
  periodId: z.string().min(1, 'Выберите период').optional(),
});

interface BaseCreditProgramFormProps {
  onSubmit: (data: CreditProgramBindingModel) => void;
  schema: z.ZodType<BaseFormValues | EditFormValues>;
  defaultValues?: Partial<BaseFormValues>;
}

const BaseCreditProgramForm = ({
  onSubmit,
  schema,
  defaultValues,
}: BaseCreditProgramFormProps): React.JSX.Element => {
  const form = useForm<BaseFormValues | EditFormValues>({
    resolver: zodResolver(schema),
    defaultValues: defaultValues
      ? {
          id: defaultValues.id ?? '',
          name: defaultValues.name ?? '',
          cost: defaultValues.cost ?? 0,
          maxCost: defaultValues.maxCost ?? 0,
          periodId: defaultValues.periodId ?? '',
        }
      : {
          id: '',
          name: '',
          cost: 0,
          maxCost: 0,
          periodId: '',
        },
  });

  const { periods } = usePeriods();

  useEffect(() => {
    if (defaultValues) {
      form.reset({
        id: defaultValues.id ?? '',
        name: defaultValues.name ?? '',
        cost: defaultValues.cost ?? 0,
        maxCost: defaultValues.maxCost ?? 0,
        periodId: defaultValues.periodId ?? '',
      });
    }
  }, [defaultValues, form]);

  const storekeeper = useAuthStore((store) => store.user);

  const handleSubmit = (data: BaseFormValues | EditFormValues) => {
    if (!storekeeper?.id) {
      console.error('Storekeeper ID is not available.');
      return;
    }

    let payload: CreditProgramBindingModel;

    if (schema === addSchema) {
      const addData = data as BaseFormValues;
      payload = {
        id: addData.id || crypto.randomUUID(),
        storekeeperId: storekeeper.id,
        name: addData.name,
        cost: addData.cost,
        maxCost: addData.maxCost,
        periodId: addData.periodId,
      };
    } else {
      const editData = data as EditFormValues;
      const currentDefaultValues = defaultValues as Partial<BaseFormValues>;

      const changedData: Partial<CreditProgramBindingModel> = {};

      if (editData.id !== undefined && editData.id !== currentDefaultValues?.id)
        changedData.id = editData.id;
      if (
        editData.name !== undefined &&
        editData.name !== currentDefaultValues?.name
      )
        changedData.name = editData.name;
      if (
        editData.cost !== undefined &&
        editData.cost !== currentDefaultValues?.cost
      )
        changedData.cost = editData.cost;
      if (
        editData.maxCost !== undefined &&
        editData.maxCost !== currentDefaultValues?.maxCost
      )
        changedData.maxCost = editData.maxCost;
      if (
        editData.periodId !== undefined &&
        editData.periodId !== currentDefaultValues?.periodId
      )
        changedData.periodId = editData.periodId;

      if (currentDefaultValues?.id) changedData.id = currentDefaultValues.id;
      changedData.storekeeperId = storekeeper.id;

      payload = {
        ...(defaultValues as CreditProgramBindingModel),
        ...changedData,
      };
    }

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
                <Input placeholder="Название" {...field} />
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
                <Input type="number" placeholder="Стоимость" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="maxCost"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Максимальная стоимость</FormLabel>
              <FormControl>
                <Input
                  type="number"
                  placeholder="Максимальная стоимость"
                  {...field}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="periodId"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Период</FormLabel>
              <Select onValueChange={field.onChange} value={field.value || ''}>
                <FormControl>
                  <SelectTrigger>
                    <SelectValue placeholder="Выберите период" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {periods &&
                    periods?.map((period) => (
                      <SelectItem key={period.id} value={period.id}>
                        {`${new Date(
                          period.startTime,
                        ).toLocaleDateString()} - ${new Date(
                          period.endTime,
                        ).toLocaleDateString()}`}
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

export const CreditProgramFormAdd = ({
  onSubmit,
}: {
  onSubmit: (data: CreditProgramBindingModel) => void;
}): React.JSX.Element => {
  return <BaseCreditProgramForm onSubmit={onSubmit} schema={addSchema} />;
};

export const CreditProgramFormEdit = ({
  onSubmit,
  defaultValues,
}: {
  onSubmit: (data: CreditProgramBindingModel) => void;
  defaultValues: Partial<BaseFormValues>;
}): React.JSX.Element => {
  return (
    <BaseCreditProgramForm
      onSubmit={onSubmit}
      schema={editSchema}
      defaultValues={defaultValues}
    />
  );
};
