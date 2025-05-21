import React from 'react';
import type { SelectedReport } from '../pages/Reports';
import { Button } from '../ui/button';
import { PdfViewer } from './PdfViewer';
import { DialogForm } from '../layout/DialogForm';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { toast } from 'sonner';
import {
  sendReportByEmail,
  type ReportType,
  type ReportFormat,
  type ReportParams,
} from '@/api/client';
import {
  useWordReport,
  useExcelReport,
  usePdfReport,
} from '@/hooks/useReports';
import { Calendar } from '../ui/calendar';
import { Popover, PopoverContent, PopoverTrigger } from '../ui/popover';
import { cn } from '@/lib/utils';
import { format } from 'date-fns';
import { CalendarIcon } from 'lucide-react';
import { type QueryObserverResult } from '@tanstack/react-query';

type ReportViewerProps = {
  selectedReport: SelectedReport;
};

type ReportData = { blob: Blob; fileName: string; mimeType: string };

export const ReportViewer = ({
  selectedReport,
}: ReportViewerProps): React.JSX.Element => {
  const [isSendDialogOpen, setIsSendDialogOpen] = React.useState(false);
  const [email, setEmail] = React.useState('');
  const [fromDate, setFromDate] = React.useState<Date | undefined>(undefined);
  const [toDate, setToDate] = React.useState<Date | undefined>(undefined);

  const pdfParams: ReportParams | undefined =
    selectedReport === 'pdf' && fromDate && toDate
      ? {
          fromDate: format(fromDate, 'yyyy-MM-dd'),
          toDate: format(toDate, 'yyyy-MM-dd'),
        }
      : undefined;

  const wordQuery = useWordReport();
  const excelQuery = useExcelReport();
  const pdfQuery = usePdfReport(pdfParams);

  const isLoading =
    wordQuery.isLoading || excelQuery.isLoading || pdfQuery.isLoading;

  // Определяем текущий отчет и ошибку на основе selectedReport
  const currentReportData = React.useMemo(() => {
    switch (selectedReport) {
      case 'word':
        return wordQuery;
      case 'excel':
        return excelQuery;
      case 'pdf':
        return pdfQuery;
      default:
        return { data: undefined, error: undefined } as Partial<
          QueryObserverResult<ReportData, Error>
        >;
    }
  }, [selectedReport, wordQuery, excelQuery, pdfQuery]);

  const { data: currentReport, error: currentError } = currentReportData;
  const isError = !!currentError;

  const reportType: ReportType =
    selectedReport === 'word' || selectedReport === 'excel'
      ? 'depositByCreditProgram'
      : 'depositAndCreditProgramByCurrency';
  const reportFormat: ReportFormat =
    selectedReport === 'pdf'
      ? 'pdf'
      : selectedReport === 'word'
      ? 'word'
      : 'excel';

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

  const handleGenerate = async () => {
    try {
      if (selectedReport === 'pdf') {
        if (!fromDate || !toDate) {
          toast.error('Пожалуйста, выберите обе даты');
          return;
        }
        await pdfQuery.refetch();
      } else if (selectedReport === 'word') {
        await wordQuery.refetch();
      } else if (selectedReport === 'excel') {
        await excelQuery.refetch();
      }
      // React Query автоматически управляет isLoading
    } catch (error) {
      // Ошибки обрабатываются React Query и доступны через isError/error
      console.error(error);
    }
  };

  const handleDownload = () => {
    if (!currentReport) {
      toast.error('Сначала сгенерируйте отчет');
      return;
    }

    const url = URL.createObjectURL(currentReport.blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = currentReport.fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  };

  const handleSend = async () => {
    if (!email) {
      toast.error('Введите email');
      return;
    }

    if (!currentReport) {
      toast.error('Сначала сгенерируйте отчет');
      return;
    }

    if (selectedReport === 'pdf' && (!fromDate || !toDate)) {
      toast.error('Пожалуйста, выберите обе даты для отправки отчета');
      return;
    }

    try {
      const emailParams: ReportParams | undefined =
        selectedReport === 'pdf' && fromDate && toDate
          ? {
              fromDate: format(fromDate, 'yyyy-MM-dd'),
              toDate: format(toDate, 'yyyy-MM-dd'),
            }
          : undefined;

      await sendReportByEmail(reportType, reportFormat, email, emailParams);
      toast.success('Отчет успешно отправлен');
      setIsSendDialogOpen(false);
      setEmail('');
    } catch (error) {
      toast.error('Ошибка при отправке отчета');
      console.error(error);
    }
  };

  const isGenerateDisabled =
    isLoading || (selectedReport === 'pdf' && (!fromDate || !toDate));

  React.useEffect(() => {
    if (currentReport) {
      toast.success('Отчет успешно загружен');
    } else if (currentError) {
      toast.error('Ошибка загрузки отчета: ' + currentError.message);
    }
  }, [currentReport, currentError]);

  return (
    <div className="w-full">
      <div className="text-lg font-semibold mb-4">
        {getReportTitle(selectedReport)}
      </div>
      <div className="flex gap-4 mb-4">
        <Button onClick={handleGenerate} disabled={isGenerateDisabled}>
          {isLoading ? 'Загрузка...' : 'Сгенерировать'}
        </Button>
        <Button onClick={handleDownload} disabled={!currentReport || isLoading}>
          Скачать
        </Button>
        <Button
          onClick={() => setIsSendDialogOpen(true)}
          disabled={!currentReport || isLoading}
        >
          Отправить
        </Button>
      </div>

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

      <DialogForm
        title="Отправка отчета"
        description="Введите email для отправки отчета"
        isOpen={isSendDialogOpen}
        onClose={() => setIsSendDialogOpen(false)}
        onSubmit={handleSend}
      >
        <div className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="email">Email</Label>
            <Input
              id="email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Введите email"
            />
          </div>
        </div>
      </DialogForm>

      <div className="mt-4">
        {isLoading && <div className="p-4">Загрузка документа...</div>}
        {isError && (
          <div className="p-4 text-red-500">
            Ошибка: {currentError?.message}
          </div>
        )}
        {!currentReport && !isLoading && !isError && (
          <div className="p-4">Выберите тип отчета и сгенерируйте его</div>
        )}
        {selectedReport === 'pdf' && currentReport && (
          <PdfViewer report={currentReport} />
        )}
      </div>
    </div>
  );
};
