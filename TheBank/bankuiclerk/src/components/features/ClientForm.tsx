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
  ClientBindingModel,
  DepositClientBindingModel,
  ClientCreditProgramBindingModel,
} from '@/types/types';
import { useAuthStore } from '@/store/workerStore';
import { useDeposits } from '@/hooks/useDeposits';
import { useCreditPrograms } from '@/hooks/useCreditPrograms';
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
  name: string;
  surname: string;
  balance: number;
  depositIds: string[];
  creditProgramIds: string[];
};

type EditFormValues = {
  id?: string;
  name?: string;
  surname?: string;
  balance?: number;
  depositIds?: string[];
  creditProgramIds?: string[];
};

const baseSchema = z.object({
  id: z.string().optional(),
  name: z.string().min(1, 'Имя обязательно'),
  surname: z.string().min(1, 'Фамилия обязательна'),
  balance: z.coerce.number().min(0, 'Баланс не может быть отрицательным'),
  depositIds: z.array(z.string()),
  creditProgramIds: z.array(z.string()),
});

const addSchema = baseSchema;

const editSchema = z.object({
  id: z.string().optional(),
  name: z.string().min(1, 'Имя обязательно').optional(),
  surname: z.string().min(1, 'Фамилия обязательна').optional(),
  balance: z.coerce
    .number()
    .min(0, 'Баланс не может быть отрицательным')
    .optional(),
  depositIds: z.array(z.string()).optional(),
  creditProgramIds: z.array(z.string()).optional(),
});

interface BaseClientFormProps {
  onSubmit: (data: ClientBindingModel) => void;
  schema: z.ZodType<BaseFormValues | EditFormValues>;
  defaultValues?: Partial<ClientBindingModel>;
}

const BaseClientForm = ({
  onSubmit,
  schema,
  defaultValues,
}: BaseClientFormProps): React.JSX.Element => {
  const { deposits } = useDeposits();
  const { creditPrograms } = useCreditPrograms();

  const initialDepositIds = useMemo(
    () =>
      defaultValues?.depositClients
        ?.map((dc) => dc.depositId)
        .filter((id): id is string => !!id) || [],
    [defaultValues?.depositClients],
  );

  const initialCreditProgramIds = useMemo(
    () =>
      defaultValues?.creditProgramClients
        ?.map((ccp) => ccp.creditProgramId)
        .filter((id): id is string => !!id) || [],
    [defaultValues?.creditProgramClients],
  );

  const form = useForm<BaseFormValues | EditFormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      id: defaultValues?.id || '',
      name: defaultValues?.name || '',
      surname: defaultValues?.surname || '',
      balance: defaultValues?.balance || 0,
      depositIds: initialDepositIds,
      creditProgramIds: initialCreditProgramIds,
    },
  });

  React.useEffect(() => {
    if (defaultValues) {
      form.reset({
        id: defaultValues.id || '',
        name: defaultValues.name || '',
        surname: defaultValues.surname || '',
        balance: defaultValues.balance || 0,
        depositIds: initialDepositIds,
        creditProgramIds: initialCreditProgramIds,
      });
    }
  }, [defaultValues, form, initialDepositIds, initialCreditProgramIds]);

  const clerk = useAuthStore((store) => store.user);

  const handleSubmit = (data: BaseFormValues | EditFormValues) => {
    const clientId = data.id || crypto.randomUUID();

    const depositClients: DepositClientBindingModel[] = (
      'depositIds' in data && data.depositIds ? data.depositIds : []
    ).map((depositId) => {
      const existingDepositClient = defaultValues?.depositClients?.find(
        (dc) => dc.depositId === depositId,
      );
      return {
        id: existingDepositClient?.id,
        clientId: clientId,
        depositId: depositId,
      };
    });

    const creditProgramClients: ClientCreditProgramBindingModel[] = (
      'creditProgramIds' in data && data.creditProgramIds
        ? data.creditProgramIds
        : []
    ).map((creditProgramId) => {
      const existingCreditProgramClient =
        defaultValues?.creditProgramClients?.find(
          (ccp) => ccp.creditProgramId === creditProgramId,
        );
      console.log(existingCreditProgramClient);
      return {
        id: existingCreditProgramClient?.id,
        clientId: clientId,
        creditProgramId: creditProgramId,
      };
    });

    const payload: ClientBindingModel = {
      id: clientId,
      clerkId: clerk?.id,
      name: 'name' in data && data.name !== undefined ? data.name : '',
      surname:
        'surname' in data && data.surname !== undefined ? data.surname : '',
      balance:
        'balance' in data && data.balance !== undefined ? data.balance : 0,
      depositClients: depositClients,
      creditProgramClients: creditProgramClients,
    };

    onSubmit(payload);
  };

  const selectedDepositIds = form.watch('depositIds') || [];
  const selectedCreditProgramIds = form.watch('creditProgramIds') || [];

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
              <FormLabel>Имя</FormLabel>
              <FormControl>
                <Input placeholder="Введите имя" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="surname"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Фамилия</FormLabel>
              <FormControl>
                <Input placeholder="Введите фамилию" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="balance"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Баланс</FormLabel>
              <FormControl>
                <Input type="number" placeholder="Введите баланс" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="depositIds"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Вклады</FormLabel>
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
                    <SelectValue placeholder="Выберите вклады" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {deposits?.map((deposit) => (
                    <SelectItem
                      key={deposit.id}
                      value={deposit.id || ''}
                      className={cn(
                        selectedDepositIds.includes(deposit.id || '') &&
                          'bg-muted',
                      )}
                    >
                      {`Вклад ${deposit.interestRate}% - ${deposit.cost}₽`}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <div className="flex flex-wrap gap-2 mt-2">
                {field.value?.map((id) => {
                  const deposit = deposits?.find((d) => d.id === id);
                  return deposit ? (
                    <div
                      key={id}
                      className="bg-secondary px-2 py-1 rounded-md text-sm flex items-center gap-2"
                    >
                      <span>{`Вклад ${deposit.interestRate}% - ${deposit.cost}₽`}</span>
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
        <FormField
          control={form.control}
          name="creditProgramIds"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Кредитные программы</FormLabel>
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
                    <SelectValue placeholder="Выберите кредитные программы" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {creditPrograms?.map((program) => (
                    <SelectItem
                      key={program.id}
                      value={program.id}
                      className={cn(
                        selectedCreditProgramIds.includes(program.id) &&
                          'bg-muted',
                      )}
                    >
                      {`${program.name} - ${program.cost}₽`}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <div className="flex flex-wrap gap-2 mt-2">
                {field.value?.map((id) => {
                  const program = creditPrograms?.find((p) => p.id === id);
                  return program ? (
                    <div
                      key={id}
                      className="bg-secondary px-2 py-1 rounded-md text-sm flex items-center gap-2"
                    >
                      <span>{`${program.name} - ${program.cost}₽`}</span>
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

export const ClientFormAdd = ({
  onSubmit,
}: {
  onSubmit: (data: ClientBindingModel) => void;
}): React.JSX.Element => {
  return <BaseClientForm onSubmit={onSubmit} schema={addSchema} />;
};

export const ClientFormEdit = ({
  onSubmit,
  defaultValues,
}: {
  onSubmit: (data: ClientBindingModel) => void;
  defaultValues: Partial<ClientBindingModel>;
}): React.JSX.Element => {
  return (
    <BaseClientForm
      onSubmit={onSubmit}
      schema={editSchema}
      defaultValues={defaultValues}
    />
  );
};
