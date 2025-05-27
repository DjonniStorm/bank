import React from 'react';
import { ReportSidebar } from '../layout/ReportSidebar';
import { ReportViewer } from '../features/ReportViewer';
import { reportsApi } from '@/api/reports';
import { format } from 'date-fns';
import { toast } from 'sonner';
import { EmailDialog } from '@/components/ui/EmailDialog';
import type {
  CreditProgramReportMailSendInfoBindingModel,
  DepositReportMailSendInfoBindingModel,
} from '@/types/types';

type ReportCategory = 'pdf' | 'word-excel' | null;
type FileFormat = 'doc' | 'xls';

export const Reports = (): React.JSX.Element => {
  const [selectedCategory, setSelectedCategory] =
    React.useState<ReportCategory>(null);
  const [pdfReport, setPdfReport] = React.useState<{
    blob: Blob;
    fileName: string;
    mimeType: string;
  } | null>(null);
  const [isEmailDialogOpen, setIsEmailDialogOpen] = React.useState(false);
  const [isEmailLoading, setIsEmailLoading] = React.useState(false);
  const [pendingEmailAction, setPendingEmailAction] = React.useState<
    | {
        type: 'pdf';
        data: { fromDate: Date; toDate: Date };
      }
    | {
        type: 'word-excel';
        data: { format: FileFormat; creditProgramIds: string[] };
      }
    | null
  >(null);

  const handleGeneratePdf = async (fromDate: Date, toDate: Date) => {
    try {
      const blob = await reportsApi.getPdfReport(
        format(fromDate, 'yyyy-MM-dd'),
        format(toDate, 'yyyy-MM-dd'),
      );
      setPdfReport({
        blob,
        fileName: `report-${format(new Date(), 'yyyy-MM-dd')}.pdf`,
        mimeType: 'application/pdf',
      });
      toast.success('PDF отчет успешно сгенерирован');
    } catch (error) {
      toast.error('Ошибка при генерации PDF отчета');
      console.error(error);
    }
  };

  const handleDownloadPdf = async (fromDate: Date, toDate: Date) => {
    try {
      const blob = await reportsApi.getPdfReport(
        format(fromDate, 'yyyy-MM-dd'),
        format(toDate, 'yyyy-MM-dd'),
      );
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `pdf-report-${format(new Date(), 'yyyy-MM-dd')}.pdf`;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
      toast.success('PDF отчет успешно скачан');
    } catch (error) {
      toast.error('Ошибка при скачивании PDF отчета');
      console.error(error);
    }
  };

  const handleSendPdfEmail = (fromDate: Date, toDate: Date) => {
    setPendingEmailAction({
      type: 'pdf',
      data: { fromDate, toDate },
    });
    setIsEmailDialogOpen(true);
  };

  const handleDownloadWordExcel = async (
    fileFormat: FileFormat,
    creditProgramIds: string[],
  ) => {
    try {
      console.log('cpIds>>', creditProgramIds);
      const blob =
        fileFormat === 'doc'
          ? await reportsApi.getWordReport(creditProgramIds)
          : await reportsApi.getExcelReport(creditProgramIds);

      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `${fileFormat}-report-${format(new Date(), 'yyyy-MM-dd')}.${
        fileFormat === 'doc' ? 'docx' : 'xlsx'
      }`;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
      toast.success(`${fileFormat.toUpperCase()} отчет успешно скачан`);
    } catch (error) {
      toast.error(`Ошибка при скачивании ${fileFormat.toUpperCase()} отчета`);
      console.error(error);
    }
  };

  const handleSendWordExcelEmail = (
    fileFormat: FileFormat,
    creditProgramIds: string[],
  ) => {
    setPendingEmailAction({
      type: 'word-excel',
      data: { format: fileFormat, creditProgramIds },
    });
    setIsEmailDialogOpen(true);
  };

  const handleEmailSubmit = async (emailData: {
    email: string;
    subject: string;
    body: string;
  }) => {
    if (!pendingEmailAction) {
      toast.error('Нет данных для отправки');
      return;
    }

    setIsEmailLoading(true);
    try {
      if (pendingEmailAction.type === 'pdf') {
        const { fromDate, toDate } = pendingEmailAction.data;
        const mailInfo: DepositReportMailSendInfoBindingModel = {
          email: emailData.email,
          toEmail: emailData.email,
          subject: emailData.subject,
          body: emailData.body,
          fromDate: format(fromDate, 'yyyy-MM-dd'),
          toDate: format(toDate, 'yyyy-MM-dd'),
        };
        await reportsApi.sendPdfReportByEmail(
          mailInfo,
          format(fromDate, 'yyyy-MM-dd'),
          format(toDate, 'yyyy-MM-dd'),
        );
      } else {
        const { format: fileFormat, creditProgramIds } =
          pendingEmailAction.data;
        const mailInfo: CreditProgramReportMailSendInfoBindingModel = {
          email: emailData.email,
          toEmail: emailData.email,
          subject: emailData.subject,
          body: emailData.body,
          creditProgramIds,
        };
        if (fileFormat === 'doc') {
          await reportsApi.sendWordReportByEmail(mailInfo);
        } else {
          await reportsApi.sendExcelReportByEmail(mailInfo);
        }
      }

      toast.success('Отчет успешно отправлен на email');
      setIsEmailDialogOpen(false);
      setPendingEmailAction(null);
    } catch (error) {
      toast.error('Ошибка при отправке отчета на email');
      console.error(error);
    } finally {
      setIsEmailLoading(false);
    }
  };

  const handleCategoryChange = (category: ReportCategory) => {
    setSelectedCategory(category);
    // Сбрасываем PDF отчет при смене категории
    if (category !== 'pdf') {
      setPdfReport(null);
    }
  };

  return (
    <>
      <div className="flex h-screen">
        <ReportSidebar
          selectedCategory={selectedCategory}
          onCategoryChange={handleCategoryChange}
        />

        <ReportViewer
          selectedCategory={selectedCategory}
          onGeneratePdf={handleGeneratePdf}
          onDownloadPdf={handleDownloadPdf}
          onSendPdfEmail={handleSendPdfEmail}
          onDownloadWordExcel={handleDownloadWordExcel}
          onSendWordExcelEmail={handleSendWordExcelEmail}
          pdfReport={pdfReport}
        />
      </div>

      <EmailDialog
        isOpen={isEmailDialogOpen}
        onClose={() => {
          setIsEmailDialogOpen(false);
          setPendingEmailAction(null);
        }}
        onSubmit={handleEmailSubmit}
        isLoading={isEmailLoading}
        defaultSubject={
          pendingEmailAction?.type === 'pdf'
            ? 'Отчет по вкладам и кредитным программам по валютам'
            : pendingEmailAction?.data.format === 'doc'
            ? 'Word отчет по вкладам по кредитным программам'
            : 'Excel отчет по вкладам по кредитным программам'
        }
        defaultBody={
          pendingEmailAction?.type === 'pdf'
            ? `Отчет по вкладам и кредитным программам по валютам за период с ${
                pendingEmailAction.data.fromDate
                  ? format(pendingEmailAction.data.fromDate, 'dd.MM.yyyy')
                  : ''
              } по ${
                pendingEmailAction.data.toDate
                  ? format(pendingEmailAction.data.toDate, 'dd.MM.yyyy')
                  : ''
              }`
            : 'В приложении находится отчет по вкладам по кредитным программам.'
        }
      />
    </>
  );
};
