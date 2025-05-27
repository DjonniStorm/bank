import React from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
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
import { Textarea } from '@/components/ui/textarea';

const emailSchema = z.object({
  email: z.string().email('Введите корректный email адрес'),
  subject: z.string().min(1, 'Тема письма обязательна'),
  body: z.string().min(1, 'Текст письма обязателен'),
});

type EmailFormData = z.infer<typeof emailSchema>;

interface EmailDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: EmailFormData) => void;
  isLoading?: boolean;
  defaultSubject?: string;
  defaultBody?: string;
}

export const EmailDialog: React.FC<EmailDialogProps> = ({
  isOpen,
  onClose,
  onSubmit,
  isLoading = false,
  defaultSubject = '',
  defaultBody = '',
}) => {
  const form = useForm<EmailFormData>({
    resolver: zodResolver(emailSchema),
    defaultValues: {
      email: '',
      subject: defaultSubject,
      body: defaultBody,
    },
  });

  React.useEffect(() => {
    if (isOpen) {
      form.reset({
        email: '',
        subject: defaultSubject,
        body: defaultBody,
      });
    }
  }, [isOpen, defaultSubject, defaultBody, form]);

  const handleSubmit = (data: EmailFormData) => {
    onSubmit(data);
  };

  const handleClose = () => {
    form.reset();
    onClose();
  };

  return (
    <Dialog open={isOpen} onOpenChange={handleClose}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Отправка отчета на почту</DialogTitle>
        </DialogHeader>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-4"
          >
            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Email адрес</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="example@example.com"
                      type="email"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="subject"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Тема письма</FormLabel>
                  <FormControl>
                    <Input placeholder="Тема письма" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="body"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Текст письма</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Текст письма..."
                      className="min-h-[100px]"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <div className="flex justify-end gap-2">
              <Button
                type="button"
                variant="outline"
                onClick={handleClose}
                disabled={isLoading}
              >
                Отмена
              </Button>
              <Button type="submit" disabled={isLoading}>
                {isLoading ? 'Отправка...' : 'Отправить'}
              </Button>
            </div>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
};
