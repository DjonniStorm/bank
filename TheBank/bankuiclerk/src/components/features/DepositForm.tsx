import React, { useMemo } from 'react';
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
import type {
  DepositBindingModel,
  DepositClientBindingModel,
} from '@/types/types';
import { useAuthStore } from '@/store/workerStore';
import { useClients } from '@/hooks/useClients';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { cn } from '@/lib/utils';

type BaseFormValues = {
  id?: string;
  interestRate: number;
  cost: number;
  period: number;
  clientIds: string[];
};

type EditFormValues = {
  id?: string;
  interestRate?: number;
  cost?: number;
  period?: number;
  clientIds?: string[];
};

const baseSchema = z.object({
  id: z.string().optional(),
  interestRate: z.coerce
    .number()
    .min(0, 'Процентная ставка не может быть отрицательной'),
  cost: z.coerce.number().min(0, 'Стоимость не может быть отрицательной'),
  period: z.coerce.number().int().min(1, 'Срок вклада должен быть не менее 1'),
  clientIds: z.array(z.string()),
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
  clientIds: z.array(z.string()).optional(),
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
  const { clients } = useClients();

  const initialClientIds = useMemo(
    () =>
      defaultValues?.depositClients
        ?.map((dc) => dc.clientId)
        .filter((id): id is string => !!id) || [],
    [defaultValues?.depositClients],
  );

  const form = useForm<BaseFormValues | EditFormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      id: defaultValues?.id || '',
      interestRate: defaultValues?.interestRate || 0,
      cost: defaultValues?.cost || 0,
      period: defaultValues?.period || 1,
      clientIds: initialClientIds,
    },
  });

  React.useEffect(() => {
    if (defaultValues) {
      form.reset({
        id: defaultValues.id || '',
        interestRate: defaultValues.interestRate || 0,
        cost: defaultValues.cost || 0,
        period: defaultValues.period || 1,
        clientIds: initialClientIds,
      });
    }
  }, [defaultValues, form, initialClientIds]);

  const clerk = useAuthStore((store) => store.user);

  const handleSubmit = (data: BaseFormValues | EditFormValues) => {
    const depositId = data.id || crypto.randomUUID();

    const depositClients: DepositClientBindingModel[] = (
      'clientIds' in data && data.clientIds ? data.clientIds : []
    ).map((clientId) => {
      const existingDepositClient = defaultValues?.depositClients?.find(
        (dc) => dc.clientId === clientId,
      );
      return {
        id: existingDepositClient?.id, // Use existing relationship ID if available
        clientId: clientId,
        depositId: depositId,
      };
    });

    const payload: DepositBindingModel = {
      id: depositId,
      clerkId: clerk?.id,
      interestRate:
        'interestRate' in data && data.interestRate !== undefined
          ? data.interestRate
          : 0,
      cost: 'cost' in data && data.cost !== undefined ? data.cost : 0,
      period: 'period' in data && data.period !== undefined ? data.period : 1,
      depositClients: depositClients,
    };

    onSubmit(payload);
  };

  const selectedClientIds = form.watch('clientIds') || [];

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
        <FormField
          control={form.control}
          name="clientIds"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Клиенты</FormLabel>
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
                    <SelectValue placeholder="Выберите клиентов" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {clients?.map((client) => (
                    <SelectItem
                      key={client.id}
                      value={client.id || ''}
                      className={cn(
                        selectedClientIds.includes(client.id || '') &&
                          'bg-muted',
                      )}
                    >
                      {`${client.name} ${client.surname}`}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <div className="flex flex-wrap gap-2 mt-2">
                {field.value?.map((id) => {
                  const client = clients?.find((c) => c.id === id);
                  return client ? (
                    <div
                      key={id}
                      className="bg-secondary px-2 py-1 rounded-md text-sm flex items-center gap-2"
                    >
                      <span>{`${client.name} ${client.surname}`}</span>
                      <button
                        type="button"
                        onClick={() => {
                          field.onChange(field.value?.filter((v) => v !== id));
                        }}
                        className="text-destructive hover:text-destructive/80"
                      >
                        ×
                      </button>
                    </div>
                  ) : null;
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
