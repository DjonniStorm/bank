// reportsApi.ts
import { useQuery, useMutation } from '@tanstack/react-query';
import {
  getReport,
  sendReportByEmail,
  type ReportParams,
  type ReportType,
  type ReportFormat,
} from '@/api/client';
import type { MailSendInfoBindingModel } from '@/types/types';

export const useReports = (reportType: ReportType, params?: ReportParams) => {
  const requiresDates =
    reportType === 'clientsByDeposit' ||
    reportType === 'depositAndCreditProgramByCurrency';
  const isEnabled: boolean =
    Boolean(reportType) &&
    (!requiresDates || (Boolean(params?.fromDate) && Boolean(params?.toDate)));

  const pdfQuery = useQuery({
    queryKey: ['pdf-document', reportType, params] as const,
    queryFn: () => getReport(reportType, 'pdf', params),
    enabled: isEnabled,
  });

  const wordQuery = useQuery({
    queryKey: ['word-document', reportType, params] as const,
    queryFn: () => getReport(reportType, 'word', params),
    enabled: isEnabled,
  });

  const excelQuery = useQuery({
    queryKey: ['excel-document', reportType, params] as const,
    queryFn: () => getReport(reportType, 'excel', params),
    enabled: isEnabled,
  });

  const sendReport = useMutation({
    mutationFn: ({
      reportType,
      format,
      mailInfo,
      params,
    }: {
      reportType: ReportType;
      format: ReportFormat;
      mailInfo: MailSendInfoBindingModel;
      params?: ReportParams;
    }) => sendReportByEmail(reportType, format, mailInfo, params),
  });

  return {
    pdfReport: pdfQuery.data,
    pdfError: pdfQuery.error,
    isPdfError: pdfQuery.isError,
    isPdfLoading: pdfQuery.isLoading,
    wordReport: wordQuery.data,
    wordError: wordQuery.error,
    isWordError: wordQuery.isError,
    isWordLoading: wordQuery.isLoading,
    excelReport: excelQuery.data,
    excelError: excelQuery.error,
    isExcelError: excelQuery.isError,
    isExcelLoading: excelQuery.isLoading,
    sendReport,
  };
};
