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
import type { CurrencyBindingModel } from '@/types/types';
import { useStorekeepers } from '@/hooks/useStorekeepers'; // Импорт хука для кладовщиков

const formSchema = z.object({
  id: z.string().optional(),
  name: z.string().min(1, 'Укажите название валюты'),
  abbreviation: z.string().min(1, 'Укажите аббревиатуру'),
  cost: z.coerce.number().min(0, 'Стоимость не может быть отрицательной'),
  storekeeperId: z.string().min(1, 'Выберите кладовщика'),
});

type FormValues = z.infer<typeof formSchema>;

type CurrencyFormProps = {
  onSubmit: (data: CurrencyBindingModel) => void;
};

export const CurrencyForm = ({
  onSubmit,
}: CurrencyFormProps): React.JSX.Element => {
  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      id: '',
      name: '',
      abbreviation: '',
      cost: 0,
      storekeeperId: '',
    },
  });

  const {
    storekeepers,
    isLoading: isLoadingStorekeepers,
    error: storekeepersError,
  } = useStorekeepers(); // Получаем данные кладовщиков

  const handleSubmit = (data: FormValues) => {
    const payload: CurrencyBindingModel = {
      ...data,
      id: data.id || crypto.randomUUID(),
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
        <FormField
          control={form.control}
          name="storekeeperId"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Кладовщик</FormLabel>
              <Select onValueChange={field.onChange} value={field.value}>
                <FormControl>
                  <SelectTrigger disabled={isLoadingStorekeepers}>
                    {' '}
                    {/* Отключаем выбор, пока данные загружаются */}
                    <SelectValue placeholder="Выберите кладовщика" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {isLoadingStorekeepers ? ( // Индикатор загрузки
                    <SelectItem value="loading" disabled>
                      Загрузка...
                    </SelectItem>
                  ) : storekeepersError ? ( // Сообщение об ошибке
                    <SelectItem value="error" disabled>
                      Ошибка загрузки
                    </SelectItem>
                  ) : (
                    // Реальные данные
                    storekeepers?.map((storekeeper) => (
                      <SelectItem key={storekeeper.id} value={storekeeper.id}>
                        {storekeeper.name} {storekeeper.surname}
                      </SelectItem>
                    ))
                  )}
                </SelectContent>
              </Select>
              {storekeepersError && (
                <FormMessage>{storekeepersError.message}</FormMessage>
              )}{' '}
              {/* Отображаем ошибку под полем */}
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
