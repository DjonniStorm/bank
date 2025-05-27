import React from 'react';
import { PdfViewer } from './PdfViewer';
import { Button } from '@/components/ui/button';
import { Calendar } from '@/components/ui/calendar';
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/components/ui/popover';
import { cn } from '@/lib/utils';
import { format } from 'date-fns';
import { CalendarIcon, FileText, FileSpreadsheet } from 'lucide-react';
import { ru } from 'date-fns/locale';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';
import { Label } from '@/components/ui/label';

import { useCreditPrograms } from '@/hooks/useCreditPrograms';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';

type ReportCategory = 'pdf' | 'word-excel' | null;
type FileFormat = 'doc' | 'xls';

interface ReportViewerProps {
  selectedCategory: ReportCategory;
  onGeneratePdf: (fromDate: Date, toDate: Date) => void;
  onDownloadPdf: (fromDate: Date, toDate: Date) => void;
  onSendPdfEmail: (fromDate: Date, toDate: Date) => void;
  onDownloadWordExcel: (format: FileFormat, creditProgramIds: string[]) => void;
  onSendWordExcelEmail: (
    format: FileFormat,
    creditProgramIds: string[],
  ) => void;
  pdfReport?: { blob: Blob; fileName: string; mimeType: string } | null;
}

export const ReportViewer = ({
  selectedCategory,
  onGeneratePdf,
  onDownloadPdf,
  onSendPdfEmail,
  onDownloadWordExcel,
  onSendWordExcelEmail,
  pdfReport,
}: ReportViewerProps): React.JSX.Element => {
  const { creditPrograms } = useCreditPrograms();

  const [fromDate, setFromDate] = React.useState<Date>();
  const [toDate, setToDate] = React.useState<Date>();
  const [fileFormat, setFileFormat] = React.useState<FileFormat>('doc');
  const [selectedCreditProgramIds, setSelectedCreditProgramIds] =
    React.useState<string[]>([]);

  const isPdfDatesValid = fromDate && toDate;
  const isWordExcelDataValid = selectedCreditProgramIds.length > 0;

  const handleCreditProgramSelect = (creditProgramId: string) => {
    setSelectedCreditProgramIds((prev) => {
      if (prev.includes(creditProgramId)) {
        return prev.filter((id) => id !== creditProgramId);
      } else {
        return [...prev, creditProgramId];
      }
    });
  };

  const removeCreditProgram = (creditProgramId: string) => {
    setSelectedCreditProgramIds((prev) =>
      prev.filter((id) => id !== creditProgramId),
    );
  };

  if (!selectedCategory) {
    return (
      <div className="flex-1 flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-2xl font-semibold mb-2">Выберите тип отчета</h2>
          <p className="text-muted-foreground">
            Выберите категорию отчета в боковой панели для начала работы
          </p>
        </div>
      </div>
    );
  }

  if (selectedCategory === 'pdf') {
    return (
      <div className="flex-1 flex flex-col">
        {/* Панель управления PDF */}
        <div className="border-b p-4 bg-background">
          <h2 className="text-xl font-semibold mb-4">
            PDF Отчет по валютам и периодам
          </h2>

          {/* Выбор дат */}
          <div className="flex gap-4 mb-4">
            <div className="flex-1">
              <Label>Дата начала</Label>
              <Popover>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    className={cn(
                      'w-full pl-3 text-left font-normal',
                      !fromDate && 'text-muted-foreground',
                    )}
                  >
                    {fromDate ? (
                      format(fromDate, 'PPP', { locale: ru })
                    ) : (
                      <span>Выберите дату</span>
                    )}
                    <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <Calendar
                    mode="single"
                    selected={fromDate}
                    onSelect={setFromDate}
                    locale={ru}
                    initialFocus
                  />
                </PopoverContent>
              </Popover>
            </div>

            <div className="flex-1">
              <Label>Дата окончания</Label>
              <Popover>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    className={cn(
                      'w-full pl-3 text-left font-normal',
                      !toDate && 'text-muted-foreground',
                    )}
                  >
                    {toDate ? (
                      format(toDate, 'PPP', { locale: ru })
                    ) : (
                      <span>Выберите дату</span>
                    )}
                    <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <Calendar
                    mode="single"
                    selected={toDate}
                    onSelect={setToDate}
                    locale={ru}
                    initialFocus
                  />
                </PopoverContent>
              </Popover>
            </div>
          </div>

          {/* Кнопки действий */}
          <div className="flex gap-2">
            <Button
              onClick={() =>
                isPdfDatesValid && onGeneratePdf(fromDate!, toDate!)
              }
              disabled={!isPdfDatesValid}
            >
              Сгенерировать на странице
            </Button>
            <Button
              onClick={() =>
                isPdfDatesValid && onDownloadPdf(fromDate!, toDate!)
              }
              disabled={!isPdfDatesValid}
              variant="outline"
            >
              Скачать
            </Button>
            <Button
              onClick={() =>
                isPdfDatesValid && onSendPdfEmail(fromDate!, toDate!)
              }
              disabled={!isPdfDatesValid}
              variant="outline"
            >
              Отправить на почту
            </Button>
          </div>
        </div>

        {/* Область просмотра PDF */}
        <div className="flex-1 overflow-auto">
          {pdfReport ? (
            <PdfViewer report={pdfReport} />
          ) : (
            <div className="flex items-center justify-center h-full">
              <p className="text-muted-foreground">
                Выберите даты и нажмите "Сгенерировать на странице" для
                просмотра отчета
              </p>
            </div>
          )}
        </div>
      </div>
    );
  }

  if (selectedCategory === 'word-excel') {
    return (
      <div className="flex-1 p-6">
        <h2 className="text-xl font-semibold mb-6">
          Word/Excel Отчет по кредитным программам
        </h2>

        {/* Выбор формата файла */}
        <div className="mb-6">
          <Label className="text-base font-medium mb-3 block">
            Формат файла
          </Label>
          <RadioGroup
            value={fileFormat}
            onValueChange={(value) => setFileFormat(value as FileFormat)}
            className="flex gap-6"
          >
            <div className="flex items-center space-x-2">
              <RadioGroupItem value="doc" id="doc" />
              <Label
                htmlFor="doc"
                className="flex items-center gap-2 cursor-pointer"
              >
                <FileText className="h-4 w-4" />
                Microsoft Word
              </Label>
            </div>
            <div className="flex items-center space-x-2">
              <RadioGroupItem value="xls" id="xls" />
              <Label
                htmlFor="xls"
                className="flex items-center gap-2 cursor-pointer"
              >
                <FileSpreadsheet className="h-4 w-4" />
                Microsoft Excel
              </Label>
            </div>
          </RadioGroup>
        </div>

        {/* Выбор кредитных программ */}
        <div className="mb-6">
          <Label className="text-base font-medium mb-3 block">
            Кредитные программы
          </Label>
          <Select
            onValueChange={(value) => {
              if (!selectedCreditProgramIds.includes(value)) {
                handleCreditProgramSelect(value);
              }
            }}
          >
            <SelectTrigger>
              <SelectValue placeholder="Выберите кредитные программы" />
            </SelectTrigger>
            <SelectContent>
              {creditPrograms?.map((program) => (
                <SelectItem
                  key={program.id}
                  value={program.id || ''}
                  className={cn(
                    selectedCreditProgramIds.includes(program.id || '') &&
                      'bg-muted',
                  )}
                >
                  {program.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>

          {/* Отображение выбранных программ */}
          <div className="flex flex-wrap gap-2 mt-3">
            {selectedCreditProgramIds.map((programId) => {
              const program = creditPrograms?.find((p) => p.id === programId);
              return (
                <div
                  key={programId}
                  className="bg-muted px-3 py-1 rounded-md flex items-center gap-2"
                >
                  <span>{program?.name || programId}</span>
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon"
                    className="h-4 w-4 rounded-full"
                    onClick={() => removeCreditProgram(programId)}
                  >
                    ×
                  </Button>
                </div>
              );
            })}
          </div>
        </div>

        {/* Кнопки действий */}
        <div className="flex gap-2">
          <Button
            onClick={() =>
              isWordExcelDataValid &&
              onDownloadWordExcel(fileFormat, selectedCreditProgramIds)
            }
            disabled={!isWordExcelDataValid}
          >
            Скачать
          </Button>
          <Button
            onClick={() =>
              isWordExcelDataValid &&
              onSendWordExcelEmail(fileFormat, selectedCreditProgramIds)
            }
            disabled={!isWordExcelDataValid}
            variant="outline"
          >
            Отправить на почту
          </Button>
        </div>

        {!isWordExcelDataValid && (
          <p className="text-sm text-muted-foreground mt-4">
            Выберите хотя бы одну кредитную программу для активации кнопок
          </p>
        )}
      </div>
    );
  }

  return <div></div>;
};
