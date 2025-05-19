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
import type { StorekeeperBindingModel } from '@/types/types';

// Схема для редактирования профиля (все поля опциональны)
const profileEditSchema = z.object({
  id: z.string().optional(),
  name: z.string().optional(),
  surname: z.string().optional(),
  middleName: z.string().optional(),
  login: z.string().optional(),
  email: z.string().email('Неверный формат email').optional(),
  phoneNumber: z.string().optional(),
  // Пароль, вероятно, должен редактироваться отдельно, но добавим опционально
  password: z.string().optional(),
});

type ProfileFormValues = z.infer<typeof profileEditSchema>;

interface ProfileFormProps {
  onSubmit: (data: Partial<StorekeeperBindingModel>) => void;
  defaultValues: ProfileFormValues;
}

export const ProfileForm = ({
  onSubmit,
  defaultValues,
}: ProfileFormProps): React.JSX.Element => {
  const form = useForm<ProfileFormValues>({
    resolver: zodResolver(profileEditSchema),
    defaultValues: defaultValues,
  });

  useEffect(() => {
    if (defaultValues) {
      form.reset(defaultValues);
    }
  }, [defaultValues, form]);

  const handleSubmit = (data: ProfileFormValues) => {
    const changedData: ProfileFormValues = {};
    (Object.keys(data) as (keyof ProfileFormValues)[]).forEach((key) => {
      changedData[key] = data[key];
      if (data[key] !== defaultValues[key]) {
        changedData[key] = data[key];
      }
    });

    if (defaultValues.id) {
      changedData.id = defaultValues.id;
    }

    onSubmit(changedData);
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
              <FormLabel>Имя</FormLabel>
              <FormControl>
                <Input placeholder="Имя" {...field} />
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
                <Input placeholder="Фамилия" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="middleName"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Отчество</FormLabel>
              <FormControl>
                <Input placeholder="Отчество" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="login"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Логин</FormLabel>
              <FormControl>
                <Input placeholder="Логин" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="email"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Email</FormLabel>
              <FormControl>
                <Input placeholder="Email" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="phoneNumber"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Номер телефона</FormLabel>
              <FormControl>
                <Input placeholder="Номер телефона" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        {/* Поле пароля можно добавить здесь, если требуется его редактирование */}
        {/*
        <FormField
          control={form.control}
          name="password"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Новый пароль (оставьте пустым, если не меняете)</FormLabel>
              <FormControl>
                <Input type="password" placeholder="Пароль" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        */}

        <Button type="submit" className="w-full">
          Сохранить изменения
        </Button>
      </form>
    </Form>
  );
};
