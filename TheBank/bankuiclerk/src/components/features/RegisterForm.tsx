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
import type { ClerkBindingModel } from '@/types/types';

interface RegisterFormProps {
  onSubmit: (data: ClerkBindingModel) => void;
  defaultValues?: Partial<ClerkBindingModel>;
}

const registerFormSchema = z.object({
  id: z.string().optional(),
  name: z.string().min(1, 'Имя обязательно'),
  surname: z.string().min(1, 'Фамилия обязательна'),
  middleName: z.string().min(1, 'Отчество обязательно'),
  login: z.string().min(3, 'Логин должен быть не короче 3 символов'),
  password: z.string().min(6, 'Пароль должен быть не короче 6 символов'),
  email: z.string().email('Введите корректный email'),
  phoneNumber: z.string().min(10, 'Введите корректный номер телефона'),
});

type FormValues = z.infer<typeof registerFormSchema>;

export const RegisterForm = ({
  onSubmit,
  defaultValues,
}: RegisterFormProps): React.JSX.Element => {
  const form = useForm<FormValues>({
    resolver: zodResolver(registerFormSchema),
    defaultValues: {
      id: defaultValues?.id || crypto.randomUUID(),
      name: defaultValues?.name || '',
      surname: defaultValues?.surname || '',
      middleName: defaultValues?.middleName || '',
      login: defaultValues?.login || '',
      password: defaultValues?.password || '',
      email: defaultValues?.email || '',
      phoneNumber: defaultValues?.phoneNumber || '',
    },
  });

  const handleSubmit = (data: FormValues) => {
    const payload: ClerkBindingModel = {
      ...data,
      id: data.id || crypto.randomUUID(),
    };
    onSubmit(payload);
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
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
          name="password"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Пароль</FormLabel>
              <FormControl>
                <Input type="password" placeholder="Пароль" {...field} />
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
                <Input type="email" placeholder="Email" {...field} />
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
        <Button type="submit" className="w-full">
          Зарегистрировать
        </Button>
      </form>
    </Form>
  );
};
