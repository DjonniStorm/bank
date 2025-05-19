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
import { useCurrencies } from '@/hooks/useCurrencies';

const formSchema = z.object({
  id: z.string().optional(),
  name: z.string().min(5, 'Название должно быть не короче 5 символов'),
  cost: z.coerce.number().min(0, 'Стоимость не может быть отрицательной'),
  maxCost: z.coerce
    .number()
    .min(0, 'Максимальная стоимость не может быть отрицательной'),
  periodId: z.string().min(1, 'Выберите период'),
  currencyCreditPrograms: z
    .array(z.string())
    .min(1, 'Выберите хотя бы одну валюту'),
});

type FormValues = z.infer<typeof formSchema>;

type CreditProgramFormProps = {
  onSubmit: (data: CreditProgramBindingModel) => void;
};

export const CreditProgramForm = ({
  onSubmit,
}: CreditProgramFormProps): React.JSX.Element => {
  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      id: '',
      name: '',
      cost: 0,
      maxCost: 0,
      periodId: '',
      currencyCreditPrograms: [],
    },
  });

  const { periods } = usePeriods();
  const { currencies } = useCurrencies();

  const storekeeper = useAuthStore((store) => store.user);

  const handleSubmit = (data: FormValues) => {
    const dataWithId = {
      ...data,
      id: crypto.randomUUID(),
    };
    const payload: CreditProgramBindingModel = {
      ...dataWithId,
      currencyCreditPrograms: data.currencyCreditPrograms.map((currencyId) => ({
        currencyId,
      })),
      storekeeperId: storekeeper?.id,
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
              <Select onValueChange={field.onChange} value={field.value}>
                <FormControl>
                  <SelectTrigger>
                    <SelectValue placeholder="Выберите период" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {periods &&
                    periods.map((period) => (
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
        <FormField
          control={form.control}
          name="currencyCreditPrograms"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Валюты</FormLabel>
              <FormControl>
                <div className="relative">
                  <select
                    multiple
                    value={field.value}
                    onChange={(e) => {
                      const selected = Array.from(e.target.selectedOptions).map(
                        (option) => option.value,
                      );
                      field.onChange(selected);
                    }}
                    className="w-full border rounded-md p-2 h-24"
                  >
                    {currencies &&
                      currencies.map((currency) => (
                        <option key={currency.id} value={currency.id}>
                          {currency.name}
                        </option>
                      ))}
                  </select>
                </div>
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
