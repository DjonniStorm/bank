import React from 'react';
import { ReportSidebar } from '../layout/ReportSidebar';
import { ReportViewer } from '../features/ReportViewer';
import { reportsApi } from '@/api/reports';
import { format } from 'date-fns';
import { toast } from 'sonner';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';

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
  const [email, setEmail] = React.useState('');
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

  const handleEmailSubmit = async () => {
    if (!email.trim()) {
      toast.error('Пожалуйста, введите email');
      return;
    }

    if (!pendingEmailAction) {
      toast.error('Нет данных для отправки');
      return;
    }

    try {
      if (pendingEmailAction.type === 'pdf') {
        const { fromDate, toDate } = pendingEmailAction.data;
        await reportsApi.sendPdfReportByEmail(
          email,
          format(fromDate, 'yyyy-MM-dd'),
          format(toDate, 'yyyy-MM-dd'),
        );
      } else {
        const { format: fileFormat, creditProgramIds } =
          pendingEmailAction.data;
        if (fileFormat === 'doc') {
          await reportsApi.sendWordReportByEmail({
            toEmail: email,
            creditProgramIds,
          });
        } else {
          await reportsApi.sendExcelReportByEmail({
            toEmail: email,
            creditProgramIds,
          });
        }
      }

      toast.success('Отчет успешно отправлен на email');
      setIsEmailDialogOpen(false);
      setEmail('');
      setPendingEmailAction(null);
    } catch (error) {
      toast.error('Ошибка при отправке отчета на email');
      console.error(error);
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

      <Dialog open={isEmailDialogOpen} onOpenChange={setIsEmailDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Отправка отчета на email</DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <div>
              <Label htmlFor="email">Email адрес</Label>
              <Input
                id="email"
                type="email"
                placeholder="example@example.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
            </div>
            <div className="flex justify-end gap-2">
              <Button
                variant="outline"
                onClick={() => {
                  setIsEmailDialogOpen(false);
                  setEmail('');
                  setPendingEmailAction(null);
                }}
              >
                Отмена
              </Button>
              <Button onClick={handleEmailSubmit}>Отправить</Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </>
  );
};
