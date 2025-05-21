import { useQuery } from '@tanstack/react-query';
import { type ReportParams } from '@/api/client';

const fetchReport = async (
  url: string,
  format: 'word' | 'excel' | 'pdf',
  params?: ReportParams,
) => {
  const query = params
    ? Object.entries(params)
        .filter(([, value]) => value !== undefined)
        .map(([key, value]) => `${key}=${encodeURIComponent(value as string)}`)
        .join('&')
    : '';
  const fullUrl = `${url}${query ? '?' + query : ''}`;

  const res = await fetch(fullUrl, {
    credentials: 'include',
  });

  if (!res.ok) {
    throw new Error(
      `Не удалось загрузить отчет с URL ${fullUrl}: ${res.statusText}`,
    );
  }

  const blob = await res.blob();
  const contentDisposition = res.headers.get('Content-Disposition');
  let fileName = 'report'; // Default filename
  let fileExtension = '';

  // Determine file extension based on format if not in Content-Disposition
  switch (format) {
    case 'word':
      fileExtension = '.docx';
      break;
    case 'excel':
      fileExtension = '.xlsx';
      break;
    case 'pdf':
      fileExtension = '.pdf';
      break;
  }

  if (contentDisposition && contentDisposition.includes('filename=')) {
    fileName = contentDisposition
      .split('filename=')[1]
      .replace(/"/g, '')
      .trim();
    // Use filename from header if available, but ensure correct extension
    if (!fileName.toLowerCase().endsWith(fileExtension)) {
      fileName = fileName + fileExtension;
    }
  } else {
    // Use default filename with determined extension
    fileName = fileName + fileExtension;
  }

  const mimeType = res.headers.get('Content-Type') || '';

  return { blob, fileName, mimeType };
};

export const useWordReport = (params?: ReportParams) => {
  return useQuery({
    queryKey: ['wordReport', params],
    queryFn: () =>
      fetchReport('/api/Report/LoadDepositByCreditProgram', 'word', params),
    enabled: false,
    staleTime: Infinity,
  });
};

export const useExcelReport = (params?: ReportParams) => {
  return useQuery({
    queryKey: ['excelReport', params],
    queryFn: () =>
      fetchReport(
        '/api/Report/LoadExcelDepositByCreditProgram',
        'excel',
        params,
      ),
    enabled: false,
    staleTime: Infinity,
  });
};

export const usePdfReport = (params?: ReportParams) => {
  return useQuery({
    queryKey: ['pdfReport', params],
    queryFn: () =>
      fetchReport(
        '/api/Report/GetDepositAndCreditProgramByCurrency',
        'pdf',
        params,
      ),
    enabled: false,
    staleTime: Infinity,
  });
};
