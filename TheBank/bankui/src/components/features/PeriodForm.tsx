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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Button } from '@/components/ui/button';
import type { PeriodBindingModel } from '@/types/types';
import { Calendar } from '@/components/ui/calendar';
import { format } from 'date-fns';
import { CalendarIcon } from 'lucide-react';
import { cn } from '@/lib/utils';
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/components/ui/popover';
import { useStorekeepers } from '@/hooks/useStorekeepers';

const formSchema = z.object({
  id: z.string().optional(),
  startTime: z.date({
    required_error: 'Укажите время начала',
    invalid_type_error: 'Неверный формат даты',
  }),
  endTime: z.date({
    required_error: 'Укажите время окончания',
    invalid_type_error: 'Неверный формат даты',
  }),
  storekeeperId: z.string().min(1, 'Выберите кладовщика'),
});

type FormValues = z.infer<typeof formSchema>;

type PeriodFormProps = {
  onSubmit: (data: PeriodBindingModel) => void;
};

export const PeriodForm = ({
  onSubmit,
}: PeriodFormProps): React.JSX.Element => {
  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      id: '',
      storekeeperId: '',
    },
  });

  const {
    storekeepers,
    isLoading: isLoadingStorekeepers,
    error: storekeepersError,
  } = useStorekeepers();

  const handleSubmit = (data: FormValues) => {
    const payload: PeriodBindingModel = {
      ...data,
      id: data.id || crypto.randomUUID(),
      startTime: data.startTime,
      endTime: data.endTime,
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
          name="startTime"
          render={({ field }) => (
            <FormItem className="flex flex-col">
              <FormLabel>Время начала</FormLabel>
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
          name="endTime"
          render={({ field }) => (
            <FormItem className="flex flex-col">
              <FormLabel>Время окончания</FormLabel>
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
          name="storekeeperId"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Кладовщик</FormLabel>
              <Select
                onValueChange={(value) => field.onChange(value)}
                value={field.value}
              >
                <FormControl>
                  <SelectTrigger disabled={isLoadingStorekeepers}>
                    <SelectValue placeholder="Выберите кладовщика" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {isLoadingStorekeepers ? (
                    <SelectItem value="loading" disabled>
                      Загрузка...
                    </SelectItem>
                  ) : storekeepersError ? (
                    <SelectItem value="error" disabled>
                      Ошибка загрузки
                    </SelectItem>
                  ) : (
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
              )}
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
