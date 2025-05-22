import type { LoginBindingModel } from '@/types/types';
import { zodResolver } from '@hookform/resolvers/zod';
import React from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '../ui/form';
import { Input } from '../ui/input';
import { Button } from '../ui/button';

interface LoginFormProps {
  onSubmit: (data: LoginBindingModel) => void;
  defaultValues?: Partial<LoginBindingModel>;
}
const loginFormSchema = z.object({
  login: z.string().min(3, 'Логин должен быть не короче 3 символов'),
  password: z.string().min(6, 'Пароль должен быть не короче 6 символов'),
});

type FormValues = z.infer<typeof loginFormSchema>;

export const LoginForm = ({
  onSubmit,
  defaultValues,
}: LoginFormProps): React.JSX.Element => {
  const form = useForm<FormValues>({
    resolver: zodResolver(loginFormSchema),
    defaultValues: {
      login: defaultValues?.login || '',
      password: defaultValues?.password || '',
    },
  });

  const handleSubmit = (data: FormValues) => {
    const payload: LoginBindingModel = {
      ...data,
    };
    onSubmit(payload);
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
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
        <Button type="submit" className="w-full">
          Войти
        </Button>
      </form>
    </Form>
  );
};
