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
import { useAuthStore } from '@/store/workerStore';

type BaseFormValues = {
  id?: string;
  startTime: Date;
  endTime: Date;
};

type EditFormValues = {
  id?: string;
  startTime?: Date;
  endTime?: Date;
};

const baseSchema = z.object({
  id: z.string().optional(),
  startTime: z.date({
    required_error: 'Укажите время начала',
    invalid_type_error: 'Неверный формат даты',
  }),
  endTime: z.date({
    required_error: 'Укажите время окончания',
    invalid_type_error: 'Неверный формат даты',
  }),
});

const addSchema = baseSchema;

const editSchema = z.object({
  id: z.string().optional(),
  startTime: z
    .date({
      required_error: 'Укажите время начала',
      invalid_type_error: 'Неверный формат даты',
    })
    .optional(),
  endTime: z
    .date({
      required_error: 'Укажите время окончания',
      invalid_type_error: 'Неверный формат даты',
    })
    .optional(),
});

interface BasePeriodFormProps {
  onSubmit: (data: PeriodBindingModel) => void;
  schema: z.ZodType<BaseFormValues | EditFormValues>;
  defaultValues?: Partial<BaseFormValues | EditFormValues>;
}

const BasePeriodForm = ({
  onSubmit,
  schema,
  defaultValues,
}: BasePeriodFormProps): React.JSX.Element => {
  const form = useForm<BaseFormValues | EditFormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      id: defaultValues?.id || '',
      startTime: defaultValues?.startTime || new Date(),
      endTime: defaultValues?.endTime || new Date(),
    },
  });

  useEffect(() => {
    if (defaultValues) {
      form.reset({
        id: defaultValues.id || '',
        startTime: defaultValues.startTime || new Date(),
        endTime: defaultValues.endTime || new Date(),
      });
    }
  }, [defaultValues, form]);

  const storekeeper = useAuthStore((store) => store.user);

  const handleSubmit = (data: BaseFormValues | EditFormValues) => {
    const payload: PeriodBindingModel = {
      id: data.id || crypto.randomUUID(),
      storekeeperId: storekeeper?.id,
      startTime:
        'startTime' in data && data.startTime !== undefined
          ? data.startTime
          : new Date(),
      endTime:
        'endTime' in data && data.endTime !== undefined
          ? data.endTime
          : new Date(),
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

        <Button type="submit" className="w-full">
          Сохранить
        </Button>
      </form>
    </Form>
  );
};

export const PeriodFormAdd = ({
  onSubmit,
}: {
  onSubmit: (data: PeriodBindingModel) => void;
}): React.JSX.Element => {
  return <BasePeriodForm onSubmit={onSubmit} schema={addSchema} />;
};

export const PeriodFormEdit = ({
  onSubmit,
  defaultValues,
}: {
  onSubmit: (data: PeriodBindingModel) => void;
  defaultValues: Partial<BaseFormValues>;
}): React.JSX.Element => {
  return (
    <BasePeriodForm
      onSubmit={onSubmit}
      schema={editSchema}
      defaultValues={defaultValues}
    />
  );
};
