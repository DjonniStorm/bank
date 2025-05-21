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

  console.log(`Загрузка отчета (${format}) с URL: ${fullUrl}`);

  try {
    const res = await fetch(fullUrl, {
      credentials: 'include',
      // Добавляем явный заголовок Accept для типа возвращаемых данных
      headers: {
        Accept:
          format === 'pdf'
            ? 'application/pdf'
            : format === 'word'
            ? 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
            : 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      },
    });

    if (!res.ok) {
      console.error(
        `Ошибка при загрузке отчета: ${res.status} ${res.statusText}`,
      );
      throw new Error(
        `Не удалось загрузить отчет с URL ${fullUrl}: ${res.statusText} (${res.status})`,
      );
    }

    const blob = await res.blob();
    console.log(
      `Отчет загружен. Тип содержимого: ${blob.type}, размер: ${blob.size} байт`,
    );

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
    console.log(`Имя файла: ${fileName}, MIME-тип: ${mimeType}`);

    return { blob, fileName, mimeType };
  } catch (error) {
    console.error('Ошибка при загрузке отчета:', error);
    throw error;
  }
};

// Хук для получения Word отчета по вкладам по кредитным программам
export const useWordReport = (params?: ReportParams) => {
  return useQuery({
    queryKey: ['wordReport', params],
    queryFn: () =>
      fetchReport('/api/Report/LoadDepositByCreditProgram', 'word', params),
    enabled: false, // Не загружать автоматически
    staleTime: Infinity,
    retry: 1,
  });
};

// Хук для получения Excel отчета по вкладам по кредитным программам
export const useExcelReport = (params?: ReportParams) => {
  return useQuery({
    queryKey: ['excelReport', params],
    queryFn: () =>
      fetchReport(
        '/api/Report/LoadExcelDepositByCreditProgram',
        'excel',
        params,
      ),
    enabled: false, // Не загружать автоматически
    staleTime: Infinity,
    retry: 1,
  });
};

// Хук для получения PDF отчета по вкладам и кредитным программам по валютам
export const usePdfReport = (params?: ReportParams) => {
  return useQuery({
    queryKey: ['pdfReport', params],
    queryFn: () =>
      fetchReport(
        '/api/Report/LoadDepositAndCreditProgramByCurrency',
        'pdf',
        params,
      ),
    enabled: false, // Не загружать автоматически
    staleTime: Infinity,
    retry: 1,
  });
};
