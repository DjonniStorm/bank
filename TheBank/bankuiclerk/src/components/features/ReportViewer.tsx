import React from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { format } from 'date-fns';
import { ru } from 'date-fns/locale';
import {
  CalendarIcon,
  FileText,
  Download,
  Mail,
  FileSpreadsheet,
} from 'lucide-react';
import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import { Calendar } from '@/components/ui/calendar';
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/components/ui/popover';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { PdfViewer } from './PdfViewer';
import { EmailDialog } from './EmailDialog';
import { useCreditPrograms } from '@/hooks/useCreditPrograms';
import type { ReportCategory } from './ReportSidebar';

// Схемы валидации
const depositsReportSchema = z
  .object({
    fromDate: z.date({ required_error: 'Выберите дату начала' }),
    toDate: z.date({ required_error: 'Выберите дату окончания' }),
  })
  .refine((data) => data.fromDate <= data.toDate, {
    message: 'Дата начала должна быть раньше даты окончания',
    path: ['toDate'],
  });

const creditProgramsReportSchema = z.object({
  creditProgramIds: z
    .array(z.string())
    .min(1, 'Выберите хотя бы одну кредитную программу'),
  format: z.enum(['word', 'excel'], {
    required_error: 'Выберите формат отчета',
  }),
});

type DepositsReportForm = z.infer<typeof depositsReportSchema>;
type CreditProgramsReportForm = z.infer<typeof creditProgramsReportSchema>;

interface ReportViewerProps {
  category: ReportCategory | null;
  onGenerateReport: (type: string, data: Record<string, unknown>) => void;
  onDownloadReport: (type: string, data: Record<string, unknown>) => void;
  onSendEmail: (
    type: string,
    data: Record<string, unknown>,
    email: string,
    subject: string,
    body: string,
  ) => void;
  pdfReport: { blob: Blob; fileName: string; mimeType: string } | null;
  isLoading: boolean;
}

export const ReportViewer = ({
  category,
  onGenerateReport,
  onDownloadReport,
  onSendEmail,
  pdfReport,
  isLoading,
}: ReportViewerProps) => {
  const { creditPrograms } = useCreditPrograms();

  // Состояние для EmailDialog
  const [isEmailDialogOpen, setIsEmailDialogOpen] = React.useState(false);
  const [emailDialogData, setEmailDialogData] = React.useState<{
    type: string;
    data: Record<string, unknown>;
    defaultSubject: string;
    defaultBody: string;
  } | null>(null);

  // Формы для разных типов отчетов
  const depositsForm = useForm<DepositsReportForm>({
    resolver: zodResolver(depositsReportSchema),
  });

  const creditProgramsForm = useForm<CreditProgramsReportForm>({
    resolver: zodResolver(creditProgramsReportSchema),
    defaultValues: {
      creditProgramIds: [],
      format: 'word',
    },
  });

  // Обработчики для отчетов по депозитам
  const handleGenerateDepositsReport = (data: DepositsReportForm) => {
    onGenerateReport('deposits-pdf', {
      fromDate: format(data.fromDate, 'yyyy-MM-dd'),
      toDate: format(data.toDate, 'yyyy-MM-dd'),
    });
  };

  const handleDownloadDepositsReport = (data: DepositsReportForm) => {
    onDownloadReport('deposits-pdf', {
      fromDate: format(data.fromDate, 'yyyy-MM-dd'),
      toDate: format(data.toDate, 'yyyy-MM-dd'),
    });
  };

  const handleSendDepositsEmail = (data: DepositsReportForm) => {
    setEmailDialogData({
      type: 'deposits-pdf',
      data: {
        fromDate: format(data.fromDate, 'yyyy-MM-dd'),
        toDate: format(data.toDate, 'yyyy-MM-dd'),
      },
      defaultSubject: 'Отчет по депозитам',
      defaultBody: `Отчет по депозитам за период с ${format(
        data.fromDate,
        'dd.MM.yyyy',
      )} по ${format(data.toDate, 'dd.MM.yyyy')}.`,
    });
    setIsEmailDialogOpen(true);
  };

  // Обработчики для отчетов по кредитным программам
  const handleDownloadCreditProgramsReport = (
    data: CreditProgramsReportForm,
  ) => {
    onDownloadReport(`creditPrograms-${data.format}`, {
      creditProgramIds: data.creditProgramIds,
    });
  };

  const handleSendCreditProgramsEmail = (data: CreditProgramsReportForm) => {
    const selectedPrograms = data.creditProgramIds
      .map((id) => creditPrograms?.find((p) => p.id === id)?.name)
      .filter(Boolean)
      .join(', ');

    setEmailDialogData({
      type: `creditPrograms-${data.format}`,
      data: {
        creditProgramIds: data.creditProgramIds,
      },
      defaultSubject: `Отчет по кредитным программам (${data.format.toUpperCase()})`,
      defaultBody: `Отчет по кредитным программам: ${selectedPrograms}.`,
    });
    setIsEmailDialogOpen(true);
  };

  // Проверка валидности форм
  const depositsFormData = depositsForm.watch();
  const isDepositsFormValid =
    depositsFormData.fromDate && depositsFormData.toDate;

  const creditProgramsFormData = creditProgramsForm.watch();
  const isCreditProgramsFormValid =
    creditProgramsFormData.creditProgramIds?.length > 0;

  // Обработка мультиселекта кредитных программ
  const selectedCreditProgramIds =
    creditProgramsForm.watch('creditProgramIds') || [];

  const handleCreditProgramSelect = (creditProgramId: string) => {
    const currentValues = selectedCreditProgramIds;
    if (!currentValues.includes(creditProgramId)) {
      creditProgramsForm.setValue('creditProgramIds', [
        ...currentValues,
        creditProgramId,
      ]);
    }
  };

  const handleCreditProgramRemove = (creditProgramId: string) => {
    const newValues = selectedCreditProgramIds.filter(
      (id) => id !== creditProgramId,
    );
    creditProgramsForm.setValue('creditProgramIds', newValues);
  };

  // Обработчик отправки email
  const handleEmailSubmit = (emailData: {
    email: string;
    subject: string;
    body: string;
  }) => {
    if (emailDialogData) {
      onSendEmail(
        emailDialogData.type,
        emailDialogData.data,
        emailData.email,
        emailData.subject,
        emailData.body,
      );
    }
  };

  if (!category) {
    return (
      <div className="flex-1 flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-2xl font-bold mb-2">Выберите категорию отчета</h2>
          <p className="text-muted-foreground">
            Используйте боковую панель для выбора типа отчета
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="flex-1 p-6">
      {category === 'deposits' && (
        <div className="space-y-6">
          <div>
            <h2 className="text-2xl font-bold mb-2">Отчеты по вкладам</h2>
            <p className="text-muted-foreground">
              PDF отчет с информацией о клиентах по вкладам за выбранный период
            </p>
          </div>

          <Form {...depositsForm}>
            <form className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <FormField
                  control={depositsForm.control}
                  name="fromDate"
                  render={({ field }) => (
                    <FormItem className="flex flex-col">
                      <FormLabel>Дата начала</FormLabel>
                      <Popover>
                        <PopoverTrigger asChild>
                          <FormControl>
                            <Button
                              variant="outline"
                              className={cn(
                                'w-full pl-3 text-left font-normal',
                                !field.value && 'text-muted-foreground',
                              )}
                            >
                              {field.value ? (
                                format(field.value, 'PPP', { locale: ru })
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
                            disabled={(date) =>
                              date > new Date() || date < new Date('1900-01-01')
                            }
                            initialFocus
                          />
                        </PopoverContent>
                      </Popover>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={depositsForm.control}
                  name="toDate"
                  render={({ field }) => (
                    <FormItem className="flex flex-col">
                      <FormLabel>Дата окончания</FormLabel>
                      <Popover>
                        <PopoverTrigger asChild>
                          <FormControl>
                            <Button
                              variant="outline"
                              className={cn(
                                'w-full pl-3 text-left font-normal',
                                !field.value && 'text-muted-foreground',
                              )}
                            >
                              {field.value ? (
                                format(field.value, 'PPP', { locale: ru })
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
                            disabled={(date) =>
                              date > new Date() || date < new Date('1900-01-01')
                            }
                            initialFocus
                          />
                        </PopoverContent>
                      </Popover>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              <div className="flex gap-2">
                <Button
                  type="button"
                  onClick={depositsForm.handleSubmit(
                    handleGenerateDepositsReport,
                  )}
                  disabled={!isDepositsFormValid || isLoading}
                  className="flex items-center gap-2"
                >
                  <FileText className="h-4 w-4" />
                  Сгенерировать на странице
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={depositsForm.handleSubmit(
                    handleDownloadDepositsReport,
                  )}
                  disabled={!isDepositsFormValid || isLoading}
                  className="flex items-center gap-2"
                >
                  <Download className="h-4 w-4" />
                  Скачать
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={depositsForm.handleSubmit(handleSendDepositsEmail)}
                  disabled={!isDepositsFormValid || isLoading}
                  className="flex items-center gap-2"
                >
                  <Mail className="h-4 w-4" />
                  Отправить на почту
                </Button>
              </div>
            </form>
          </Form>

          {pdfReport && (
            <div className="border rounded-lg">
              <PdfViewer report={pdfReport} />
            </div>
          )}
        </div>
      )}

      {category === 'creditPrograms' && (
        <div className="space-y-6">
          <div>
            <h2 className="text-2xl font-bold mb-2">
              Отчеты по кредитным программам
            </h2>
            <p className="text-muted-foreground">
              Word или Excel отчет с информацией о клиентах по выбранным
              кредитным программам
            </p>
          </div>

          <Form {...creditProgramsForm}>
            <form className="space-y-4">
              <FormField
                control={creditProgramsForm.control}
                name="format"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Формат отчета</FormLabel>
                    <div className="flex gap-4">
                      <Button
                        type="button"
                        variant={field.value === 'word' ? 'default' : 'outline'}
                        onClick={() => field.onChange('word')}
                        className="flex items-center gap-2"
                      >
                        <FileText className="h-4 w-4" />
                        Word
                      </Button>
                      <Button
                        type="button"
                        variant={
                          field.value === 'excel' ? 'default' : 'outline'
                        }
                        onClick={() => field.onChange('excel')}
                        className="flex items-center gap-2"
                      >
                        <FileSpreadsheet className="h-4 w-4" />
                        Excel
                      </Button>
                    </div>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={creditProgramsForm.control}
                name="creditProgramIds"
                render={() => (
                  <FormItem>
                    <FormLabel>Кредитные программы</FormLabel>
                    <Select onValueChange={handleCreditProgramSelect}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Выберите кредитные программы" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {creditPrograms?.map((program) => (
                          <SelectItem
                            key={program.id}
                            value={program.id || ''}
                            className={cn(
                              selectedCreditProgramIds.includes(
                                program.id || '',
                              ) && 'bg-muted',
                            )}
                          >
                            {program.name}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <div className="flex flex-wrap gap-2 mt-2">
                      {selectedCreditProgramIds.map((programId) => {
                        const program = creditPrograms?.find(
                          (p) => p.id === programId,
                        );
                        return (
                          <div
                            key={programId}
                            className="bg-muted px-2 py-1 rounded-md flex items-center gap-1"
                          >
                            <span>{program?.name || programId}</span>
                            <Button
                              type="button"
                              variant="ghost"
                              size="icon"
                              className="h-4 w-4 rounded-full"
                              onClick={() =>
                                handleCreditProgramRemove(programId)
                              }
                            >
                              ×
                            </Button>
                          </div>
                        );
                      })}
                    </div>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <div className="flex gap-2">
                <Button
                  type="button"
                  onClick={creditProgramsForm.handleSubmit(
                    handleDownloadCreditProgramsReport,
                  )}
                  disabled={!isCreditProgramsFormValid || isLoading}
                  className="flex items-center gap-2"
                >
                  <Download className="h-4 w-4" />
                  Скачать
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={creditProgramsForm.handleSubmit(
                    handleSendCreditProgramsEmail,
                  )}
                  disabled={!isCreditProgramsFormValid || isLoading}
                  className="flex flex-col items-center gap-1 h-auto py-2 px-3 min-w-[100px]"
                >
                  <Mail className="h-4 w-4" />
                  <span className="text-xs leading-tight text-center">
                    Отправить на почту
                  </span>
                </Button>
              </div>
            </form>
          </Form>
        </div>
      )}

      {/* Email Dialog */}
      <EmailDialog
        isOpen={isEmailDialogOpen}
        onClose={() => setIsEmailDialogOpen(false)}
        onSubmit={handleEmailSubmit}
        isLoading={isLoading}
        title={emailDialogData?.defaultSubject || 'Отправить отчет на почту'}
        description="Заполните данные для отправки отчета"
        defaultSubject={
          emailDialogData?.defaultSubject || 'Отчет из банковской системы'
        }
        defaultBody={
          emailDialogData?.defaultBody ||
          'Во вложении находится запрошенный отчет.'
        }
      />
    </div>
  );
};
