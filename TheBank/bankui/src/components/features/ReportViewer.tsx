import React from 'react';
import type { SelectedReport } from '../pages/Reports';
import { Button } from '../ui/button';
import { PdfViewer } from './PdfViewer';
import { DialogForm } from '../layout/DialogForm';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { toast } from 'sonner';
import { Calendar } from '../ui/calendar';
import { Popover, PopoverContent, PopoverTrigger } from '../ui/popover';
import { cn } from '@/lib/utils';
import { format } from 'date-fns';
import { CalendarIcon } from 'lucide-react';
import { zodResolver } from '@hookform/resolvers/zod';
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
import { ConfigManager } from '@/lib/config';

type ReportViewerProps = {
  selectedReport: SelectedReport;
};

type ReportData = { blob: Blob; fileName: string; mimeType: string };

const emailFormSchema = z.object({
  toEmail: z.string().email({ message: 'Введите корректный email' }),
  subject: z.string().min(1, { message: 'Тема обязательна' }),
  body: z.string().min(1, { message: 'Текст сообщения обязателен' }),
});

const API_URL = ConfigManager.loadUrl();

export const ReportViewer = ({
  selectedReport,
}: ReportViewerProps): React.JSX.Element => {
  const [isSendDialogOpen, setIsSendDialogOpen] = React.useState(false);
  const [fromDate, setFromDate] = React.useState<Date | undefined>(undefined);
  const [toDate, setToDate] = React.useState<Date | undefined>(undefined);
  const [isLoading, setIsLoading] = React.useState(false);
  const [error, setError] = React.useState<Error | null>(null);
  const [report, setReport] = React.useState<ReportData | null>(null);

  const form = useForm<z.infer<typeof emailFormSchema>>({
    resolver: zodResolver(emailFormSchema),
    defaultValues: {
      toEmail: '',
      subject: getDefaultSubject(selectedReport),
      body: getDefaultBody(selectedReport, fromDate, toDate),
    },
  });

  React.useEffect(() => {
    form.setValue('subject', getDefaultSubject(selectedReport));
    form.setValue('body', getDefaultBody(selectedReport, fromDate, toDate));

    setReport(null);
  }, [selectedReport, fromDate, toDate, form]);

  const getReportTitle = (report: SelectedReport) => {
    switch (report) {
      case 'word':
        return 'Отчет Word по вкладам по кредитным программам';
      case 'excel':
        return 'Отчет Excel по вкладам по кредитным программам';
      case 'pdf':
        return 'Отчет PDF по вкладам и кредитным программам по валютам';
      default:
        return 'Выберите тип отчета';
    }
  };

  function getDefaultSubject(report: SelectedReport | undefined): string {
    switch (report) {
      case 'word':
        return 'Отчет по вкладам по кредитным программам';
      case 'excel':
        return 'Excel отчет по вкладам по кредитным программам';
      case 'pdf':
        return 'Отчет по вкладам и кредитным программам по валютам';
      default:
        return 'Отчет';
    }
  }

  function getDefaultBody(
    report: SelectedReport | undefined,
    fromDate?: Date,
    toDate?: Date,
  ): string {
    switch (report) {
      case 'word':
        return 'В приложении находится отчет по вкладам по кредитным программам.';
      case 'excel':
        return 'В приложении находится Excel отчет по вкладам по кредитным программам.';
      case 'pdf':
        return `В приложении находится отчет по вкладам и кредитным программам по валютам${
          fromDate && toDate
            ? ` за период с ${format(fromDate, 'dd.MM.yyyy')} по ${format(
                toDate,
                'dd.MM.yyyy',
              )}`
            : ''
        }.`;
      default:
        return '';
    }
  }

  const getReportUrl = (
    selectedReport: SelectedReport,
    fromDate?: Date,
    toDate?: Date,
  ): string => {
    switch (selectedReport) {
      case 'word':
        return `${API_URL}/api/Report/LoadDepositByCreditProgram`;
      case 'excel':
        return `${API_URL}/api/Report/LoadExcelDepositByCreditProgram`;
      case 'pdf': {
        if (!fromDate || !toDate) {
          throw new Error('Необходимо выбрать даты для PDF отчета');
        }
        const fromDateStr = format(fromDate, 'yyyy-MM-dd');
        const toDateStr = format(toDate, 'yyyy-MM-dd');
        return `${API_URL}/api/Report/LoadDepositAndCreditProgramByCurrency?fromDate=${fromDateStr}&toDate=${toDateStr}`;
      }
      default:
        throw new Error('Выберите тип отчета');
    }
  };

  const getSendEmailUrl = (selectedReport: SelectedReport): string => {
    switch (selectedReport) {
      case 'word':
        return `${API_URL}/api/Report/SendReportDepositByCreditProgram`;
      case 'excel':
        return `${API_URL}/api/Report/SendExcelReportDepositByCreditProgram`;
      case 'pdf': {
        if (!fromDate || !toDate) {
          throw new Error('Необходимо выбрать даты для PDF отчета');
        }
        const fromDateStr = format(fromDate, 'yyyy-MM-dd');
        const toDateStr = format(toDate, 'yyyy-MM-dd');
        return `${API_URL}/api/Report/SendReportByCurrency?fromDate=${fromDateStr}&toDate=${toDateStr}`;
      }
      default:
        throw new Error('Выберите тип отчета');
    }
  };

  const fetchReport = async (): Promise<ReportData> => {
    try {
      setIsLoading(true);
      setError(null);

      const url = getReportUrl(selectedReport, fromDate, toDate);
      console.log(`Загружаем отчет с URL: ${url}`);

      const acceptHeader =
        selectedReport === 'pdf'
          ? 'application/pdf'
          : selectedReport === 'word'
          ? 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
          : 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';

      const response = await fetch(url, {
        method: 'GET',
        headers: {
          Accept: acceptHeader,
        },
        credentials: 'include',
      });

      if (!response.ok) {
        throw new Error(
          `Ошибка загрузки отчета: ${response.status} ${response.statusText}`,
        );
      }

      const blob = await response.blob();
      console.log(
        `Отчет загружен. Тип: ${blob.type}, размер: ${blob.size} байт`,
      );

      const contentDisposition = response.headers.get('Content-Disposition');
      const defaultExtension =
        selectedReport === 'pdf'
          ? '.pdf'
          : selectedReport === 'word'
          ? '.docx'
          : '.xlsx';
      let fileName = `report${defaultExtension}`;

      if (contentDisposition && contentDisposition.includes('filename=')) {
        fileName = contentDisposition
          .split('filename=')[1]
          .replace(/"/g, '')
          .trim();
      }

      const mimeType = response.headers.get('Content-Type') || '';

      const reportData = { blob, fileName, mimeType };
      setReport(reportData);
      return reportData;
    } catch (error) {
      console.error('Ошибка при загрузке отчета:', error);
      const err =
        error instanceof Error ? error : new Error('Неизвестная ошибка');
      setError(err);
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const handleGenerate = async () => {
    try {
      await fetchReport();
      toast.success('Отчет успешно загружен');
    } catch (error) {
      toast.error(
        `Ошибка загрузки отчета: ${
          error instanceof Error ? error.message : 'Неизвестная ошибка'
        }`,
      );
    }
  };

  const handleDownload = async () => {
    try {
      let reportData = report;
      // Для PDF всегда делаем новый запрос с актуальными датами
      if (selectedReport === 'pdf') {
        if (!fromDate || !toDate) {
          toast.error('Пожалуйста, выберите даты для PDF отчета');
          return;
        }
        toast.loading('Загрузка отчета...');
        reportData = await fetchReport();
      } else if (!reportData) {
        toast.loading('Загрузка отчета...');
        reportData = await fetchReport();
      }

      // Скачиваем отчет
      const url = URL.createObjectURL(reportData.blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = reportData.fileName;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
      toast.success('Отчет успешно скачан');
    } catch (error) {
      toast.error(
        `Ошибка при скачивании отчета: ${
          error instanceof Error ? error.message : 'Неизвестная ошибка'
        }`,
      );
    }
  };

  const handleSendFormSubmit = async (
    values: z.infer<typeof emailFormSchema>,
  ) => {
    try {
      // Если выбран PDF отчет, проверяем наличие дат
      if (selectedReport === 'pdf' && (!fromDate || !toDate)) {
        toast.error('Пожалуйста, выберите даты для PDF отчета');
        return;
      }

      setIsLoading(true);

      // Формируем данные для отправки
      const url = getSendEmailUrl(selectedReport);

      // Параметры для запроса
      const data: Record<string, string> = {
        toEmail: values.toEmail,
        subject: values.subject,
        body: values.body,
      };

      // Отправляем запрос
      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(
          `Ошибка при отправке отчета: ${response.status} ${response.statusText}\n${errorText}`,
        );
      }

      toast.success('Отчет успешно отправлен на почту');
      setIsSendDialogOpen(false);
      form.reset();
    } catch (error) {
      console.error('Ошибка при отправке отчета:', error);
      toast.error(
        `Ошибка при отправке отчета: ${
          error instanceof Error ? error.message : 'Неизвестная ошибка'
        }`,
      );
    } finally {
      setIsLoading(false);
    }
  };

  // Проверка, можно ли сгенерировать/скачать/отправить PDF отчет
  const isPdfActionDisabled =
    selectedReport === 'pdf' && (!fromDate || !toDate || isLoading);

  // Отображение ошибки, если она есть
  const renderError = () => {
    if (!error) return null;

    return (
      <div className="p-4 border border-red-300 bg-red-50 rounded-md mt-2">
        <h3 className="text-red-700 font-semibold mb-1">Детали ошибки:</h3>
        <p className="text-red-600 whitespace-pre-wrap break-words">
          {error.message}
        </p>
      </div>
    );
  };

  return (
    <div className="w-full">
      <div className="text-lg font-semibold mb-4">
        {getReportTitle(selectedReport)}
      </div>

      {/* Кнопки действий */}
      <div className="flex gap-4 mb-4">
        {/* Кнопка "Сгенерировать" только для PDF с выбранными датами */}
        {selectedReport === 'pdf' && (
          <Button onClick={handleGenerate} disabled={isPdfActionDisabled}>
            {isLoading ? 'Загрузка...' : 'Сгенерировать'}
          </Button>
        )}

        {/* Кнопки "Скачать" и "Отправить" только когда выбран тип отчета */}
        {selectedReport && (
          <>
            <Button
              onClick={handleDownload}
              disabled={isPdfActionDisabled || isLoading}
            >
              {isLoading ? 'Загрузка...' : 'Скачать'}
            </Button>

            <Button
              onClick={() => setIsSendDialogOpen(true)}
              disabled={isPdfActionDisabled || isLoading}
            >
              Отправить
            </Button>
          </>
        )}
      </div>

      {/* Календари для выбора периода для PDF отчета */}
      {selectedReport === 'pdf' && (
        <div className="flex gap-4 mb-4">
          <div className="grid gap-2">
            <Label htmlFor="fromDate">От даты</Label>
            <Popover>
              <PopoverTrigger asChild>
                <Button
                  variant={'outline'}
                  className={cn(
                    'w-[240px] justify-start text-left font-normal',
                    !fromDate && 'text-muted-foreground',
                  )}
                >
                  <CalendarIcon className="mr-2 h-4 w-4" />
                  {fromDate ? (
                    format(fromDate, 'PPP')
                  ) : (
                    <span>Выберите дату</span>
                  )}
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-auto p-0">
                <Calendar
                  mode="single"
                  selected={fromDate}
                  onSelect={setFromDate}
                  initialFocus
                />
              </PopoverContent>
            </Popover>
          </div>
          <div className="grid gap-2">
            <Label htmlFor="toDate">До даты</Label>
            <Popover>
              <PopoverTrigger asChild>
                <Button
                  variant={'outline'}
                  className={cn(
                    'w-[240px] justify-start text-left font-normal',
                    !toDate && 'text-muted-foreground',
                  )}
                >
                  <CalendarIcon className="mr-2 h-4 w-4" />
                  {toDate ? format(toDate, 'PPP') : <span>Выберите дату</span>}
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-auto p-0">
                <Calendar
                  mode="single"
                  selected={toDate}
                  onSelect={setToDate}
                  initialFocus
                />
              </PopoverContent>
            </Popover>
          </div>
        </div>
      )}

      {/* Форма отправки отчета на почту */}
      <DialogForm
        title="Отправка отчета"
        description="Введите данные для отправки отчета"
        isOpen={isSendDialogOpen}
        onClose={() => setIsSendDialogOpen(false)}
        onSubmit={form.handleSubmit(handleSendFormSubmit)}
      >
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSendFormSubmit)}
            className="space-y-4"
          >
            <FormField
              control={form.control}
              name="toEmail"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Email получателя</FormLabel>
                  <FormControl>
                    <Input placeholder="example@mail.ru" {...field} />
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
                    <Input {...field} />
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
                  <FormLabel>Текст сообщения</FormLabel>
                  <FormControl>
                    <Input {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <Button type="submit">Отправить</Button>
          </form>
        </Form>
      </DialogForm>

      <div className="mt-4">
        {isLoading && <div className="p-4">Загрузка документа...</div>}

        {renderError()}

        {!selectedReport && !isLoading && !error && (
          <div className="p-4">Выберите тип отчета из боковой панели</div>
        )}

        {selectedReport && !report && !isLoading && !error && (
          <div className="p-4">
            {selectedReport === 'pdf'
              ? 'Выберите даты и нажмите "Сгенерировать"'
              : 'Нажмите "Скачать" для загрузки отчета'}
          </div>
        )}

        {selectedReport === 'pdf' && report && <PdfViewer report={report} />}
      </div>
    </div>
  );
};
