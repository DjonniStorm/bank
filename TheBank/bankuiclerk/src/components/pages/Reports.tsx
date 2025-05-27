import React from 'react';
import { toast } from 'sonner';
import {
  ReportSidebar,
  type ReportCategory,
} from '@/components/features/ReportSidebar';
import { ReportViewer } from '@/components/features/ReportViewer';
import { useReports } from '@/hooks/useReports';

export const Reports = (): React.JSX.Element => {
  const [selectedCategory, setSelectedCategory] =
    React.useState<ReportCategory | null>(null);
  const [pdfReport, setPdfReport] = React.useState<{
    blob: Blob;
    fileName: string;
    mimeType: string;
  } | null>(null);

  const {
    generateDepositsPdfReport,
    isGeneratingDepositsPdf,
    sendDepositsPdfReport,
    isSendingDepositsPdf,
    generateCreditProgramsWordReport,
    isGeneratingCreditProgramsWord,
    sendCreditProgramsWordReport,
    isSendingCreditProgramsWord,
    generateCreditProgramsExcelReport,
    isGeneratingCreditProgramsExcel,
    sendCreditProgramsExcelReport,
    isSendingCreditProgramsExcel,
  } = useReports();

  const isLoading =
    isGeneratingDepositsPdf ||
    isSendingDepositsPdf ||
    isGeneratingCreditProgramsWord ||
    isSendingCreditProgramsWord ||
    isGeneratingCreditProgramsExcel ||
    isSendingCreditProgramsExcel;

  const handleCategorySelect = (category: ReportCategory) => {
    setSelectedCategory(category);
    setPdfReport(null); // Сбрасываем PDF при смене категории
  };

  const handleReset = () => {
    setSelectedCategory(null);
    setPdfReport(null);
  };

  const downloadFile = (blob: Blob, fileName: string, mimeType?: string) => {
    // Просто используем имя файла как есть, если оно уже содержит расширение
    let finalFileName = fileName;

    // Проверяем, есть ли уже расширение в имени файла
    const hasExtension = /\.(docx|xlsx|pdf|doc|xls)$/i.test(fileName);

    if (!hasExtension) {
      // Только если нет расширения, пытаемся его добавить
      if (mimeType && mimeType !== 'application/octet-stream') {
        const extension = getExtensionFromMimeType(mimeType);
        if (extension) {
          finalFileName = `${fileName}${extension}`;
        }
      } else {
        // Fallback: определяем по имени файла
        const extension = getExtensionFromFileName(fileName);
        if (extension) {
          finalFileName = `${fileName}${extension}`;
        }
      }
    }

    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = finalFileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  };

  const getExtensionFromMimeType = (mimeType: string): string => {
    switch (mimeType.toLowerCase()) {
      case 'application/vnd.openxmlformats-officedocument.wordprocessingml.document':
        return '.docx';
      case 'application/msword':
        return '.doc';
      case 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet':
        return '.xlsx';
      case 'application/vnd.ms-excel':
        return '.xls';
      case 'application/pdf':
        return '.pdf';
      default:
        return '';
    }
  };

  const getExtensionFromFileName = (fileName: string): string => {
    const lowerFileName = fileName.toLowerCase();
    if (lowerFileName.includes('clientsbycreditprogram')) {
      return '.docx'; // Word файл для клиентов по кредитным программам
    }
    if (lowerFileName.includes('excel') || lowerFileName.includes('.xlsx')) {
      return '.xlsx';
    }
    if (lowerFileName.includes('pdf') || lowerFileName.includes('deposit')) {
      return '.pdf';
    }
    return '';
  };

  const handleGenerateReport = (
    type: string,
    data: Record<string, unknown>,
  ) => {
    if (type === 'deposits-pdf') {
      const { fromDate, toDate } = data as { fromDate: string; toDate: string };
      generateDepositsPdfReport(
        { fromDate, toDate },
        {
          onSuccess: (report) => {
            setPdfReport(report);
            toast.success('PDF отчет успешно сгенерирован');
          },
          onError: (error) => {
            console.error('Ошибка генерации PDF отчета:', error);
            toast.error('Ошибка при генерации PDF отчета');
          },
        },
      );
    }
  };

  const handleDownloadReport = (
    type: string,
    data: Record<string, unknown>,
  ) => {
    if (type === 'deposits-pdf') {
      const { fromDate, toDate } = data as { fromDate: string; toDate: string };
      generateDepositsPdfReport(
        { fromDate, toDate },
        {
          onSuccess: (report) => {
            downloadFile(report.blob, report.fileName, report.mimeType);
            toast.success('PDF отчет успешно скачан');
          },
          onError: (error) => {
            console.error('Ошибка скачивания PDF отчета:', error);
            toast.error('Ошибка при скачивании PDF отчета');
          },
        },
      );
    } else if (type === 'creditPrograms-word') {
      const { creditProgramIds } = data as { creditProgramIds: string[] };
      generateCreditProgramsWordReport(
        { creditProgramIds },
        {
          onSuccess: (report) => {
            downloadFile(report.blob, report.fileName, report.mimeType);
            toast.success('Word отчет успешно скачан');
          },
          onError: (error) => {
            console.error('Ошибка скачивания Word отчета:', error);
            toast.error('Ошибка при скачивании Word отчета');
          },
        },
      );
    } else if (type === 'creditPrograms-excel') {
      const { creditProgramIds } = data as { creditProgramIds: string[] };
      generateCreditProgramsExcelReport(
        { creditProgramIds },
        {
          onSuccess: (report) => {
            downloadFile(report.blob, report.fileName, report.mimeType);
            toast.success('Excel отчет успешно скачан');
          },
          onError: (error) => {
            console.error('Ошибка скачивания Excel отчета:', error);
            toast.error('Ошибка при скачивании Excel отчета');
          },
        },
      );
    }
  };

  const handleSendEmail = (
    type: string,
    data: Record<string, unknown>,
    email: string,
  ) => {
    if (type === 'deposits-pdf') {
      const { fromDate, toDate } = data as { fromDate: string; toDate: string };
      sendDepositsPdfReport(
        { fromDate, toDate, email },
        {
          onSuccess: () => {
            toast.success(`PDF отчет успешно отправлен на ${email}`);
          },
          onError: (error) => {
            console.error('Ошибка отправки PDF отчета:', error);
            toast.error('Ошибка при отправке PDF отчета на email');
          },
        },
      );
    } else if (type === 'creditPrograms-word') {
      const { creditProgramIds } = data as { creditProgramIds: string[] };
      sendCreditProgramsWordReport(
        { creditProgramIds, email },
        {
          onSuccess: () => {
            toast.success(`Word отчет успешно отправлен на ${email}`);
          },
          onError: (error) => {
            console.error('Ошибка отправки Word отчета:', error);
            toast.error('Ошибка при отправке Word отчета на email');
          },
        },
      );
    } else if (type === 'creditPrograms-excel') {
      const { creditProgramIds } = data as { creditProgramIds: string[] };
      sendCreditProgramsExcelReport(
        { creditProgramIds, email },
        {
          onSuccess: () => {
            toast.success(`Excel отчет успешно отправлен на ${email}`);
          },
          onError: (error) => {
            console.error('Ошибка отправки Excel отчета:', error);
            toast.error('Ошибка при отправке Excel отчета на email');
          },
        },
      );
    }
  };

  return (
    <main className="flex-1 flex relative">
      <ReportSidebar
        selectedCategory={selectedCategory}
        onCategorySelect={handleCategorySelect}
        onReset={handleReset}
      />
      <ReportViewer
        category={selectedCategory}
        onGenerateReport={handleGenerateReport}
        onDownloadReport={handleDownloadReport}
        onSendEmail={handleSendEmail}
        pdfReport={pdfReport}
        isLoading={isLoading}
      />
    </main>
  );
};
