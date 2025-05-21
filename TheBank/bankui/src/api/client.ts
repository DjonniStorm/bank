import { ConfigManager } from '@/lib/config';

const API_URL = ConfigManager.loadUrl();

export async function getData<T>(path: string): Promise<T[]> {
  const res = await fetch(`${API_URL}/${path}`, {
    credentials: 'include',
  });
  if (!res.ok) {
    throw new Error(`Не получается загрузить ${path}: ${res.statusText}`);
  }
  const data = (await res.json()) as T[];
  return data;
}

export async function postData<T>(path: string, data: T) {
  const res = await fetch(`${API_URL}/${path}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      // mode: 'no-cors',
    },
    credentials: 'include',
    body: JSON.stringify(data),
  });
  if (!res.ok) {
    throw new Error(`Не получается загрузить ${path}: ${res.statusText}`);
  }
}

export async function getSingleData<T>(path: string): Promise<T> {
  const res = await fetch(`${API_URL}/${path}`, {
    credentials: 'include',
  });
  if (!res.ok) {
    throw new Error(`Не получается загрузить ${path}: ${res.statusText}`);
  }
  const data = (await res.json()) as T;
  return data;
}

export async function postLoginData<T>(path: string, data: T): Promise<T> {
  const res = await fetch(`${API_URL}/${path}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify(data),
  });
  if (!res.ok) {
    throw new Error(`Не получается загрузить ${path}: ${await res.text()}`);
  }

  const userData = (await res.json()) as T;
  return userData;
}

export async function putData<T>(path: string, data: T) {
  const res = await fetch(`${API_URL}/${path}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify(data),
  });
  if (!res.ok) {
    throw new Error(`Не получается загрузить ${path}: ${res.statusText}`);
  }
}

// report api
export interface ReportParams {
  fromDate?: string; // Например, '2025-01-01'
  toDate?: string; // Например, '2025-05-21'
}

export type ReportType =
  | 'clientsByCreditProgram'
  | 'clientsByDeposit'
  | 'depositByCreditProgram'
  | 'depositAndCreditProgramByCurrency';
export type ReportFormat = 'word' | 'excel' | 'pdf';

export async function getReport(
  reportType: ReportType,
  format: ReportFormat,
  params?: ReportParams,
): Promise<{ blob: Blob; fileName: string; mimeType: string }> {
  const actionMap: Record<ReportType, Record<ReportFormat, string>> = {
    clientsByCreditProgram: {
      word: 'LoadClientsByCreditProgram',
      excel: 'LoadExcelClientByCreditProgram',
      pdf: 'LoadPdfClientsByCreditProgram',
    },
    clientsByDeposit: {
      word: 'LoadClientsByDeposit',
      excel: 'LoadExcelClientsByDeposit',
      pdf: 'LoadPdfClientsByDeposit',
    },
    depositByCreditProgram: {
      word: 'LoadDepositByCreditProgram',
      excel: 'LoadExcelDepositByCreditProgram',
      pdf: 'LoadPdfDepositByCreditProgram',
    },
    depositAndCreditProgramByCurrency: {
      word: 'LoadDepositAndCreditProgramByCurrency',
      excel: 'LoadExcelDepositAndCreditProgramByCurrency',
      pdf: 'LoadPdfDepositAndCreditProgramByCurrency',
    },
  };

  const action = actionMap[reportType][format];
  let query = '';
  if (params) {
    const paramParts: string[] = [];
    if (params.fromDate)
      paramParts.push(`fromDate=${encodeURIComponent(params.fromDate)}`);
    if (params.toDate)
      paramParts.push(`toDate=${encodeURIComponent(params.toDate)}`);
    if (paramParts.length > 0) query = `?${paramParts.join('&')}`;
  }

  const url = `${API_URL}/api/Reports/${action}${query}`;
  const res = await fetch(url, {
    credentials: 'include',
  });

  if (!res.ok) {
    throw new Error(
      `Не удалось загрузить отчет ${reportType} (${format}): ${res.statusText}`,
    );
  }

  const blob = await res.blob();
  const contentDisposition = res.headers.get('Content-Disposition');
  let fileName = `${reportType}.${format}`;

  if (contentDisposition && contentDisposition.includes('filename=')) {
    fileName = contentDisposition
      .split('filename=')[1]
      .replace(/"/g, '')
      .trim();
  }

  const mimeType =
    res.headers.get('Content-Type') ||
    {
      word: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
      excel:
        'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      pdf: 'application/pdf',
    }[format];

  return { blob, fileName, mimeType };
}

export async function sendReportByEmail(
  reportType: ReportType,
  format: ReportFormat,
  email: string,
  params?: ReportParams,
): Promise<void> {
  const actionMap: Record<ReportType, Record<ReportFormat, string>> = {
    clientsByCreditProgram: {
      word: 'SendReportByCreditProgram',
      excel: 'SendExcelReportByCreditProgram',
      pdf: 'SendPdfReportByCreditProgram',
    },
    clientsByDeposit: {
      word: 'SendReportByDeposit',
      excel: 'SendExcelReportByDeposit',
      pdf: 'SendPdfReportByDeposit',
    },
    depositByCreditProgram: {
      word: 'SendReportDepositByCreditProgram',
      excel: 'SendExcelReportDepositByCreditProgram',
      pdf: 'SendPdfReportDepositByCreditProgram',
    },
    depositAndCreditProgramByCurrency: {
      word: 'SendReportByCurrency',
      excel: 'SendExcelReportByCurrency',
      pdf: 'SendPdfReportByCurrency',
    },
  };

  const action = actionMap[reportType][format];
  const res = await fetch(`${API_URL}/api/Reports/${action}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify({ email, ...params }),
  });

  if (!res.ok) {
    throw new Error(
      `Не удалось отправить отчет ${reportType} (${format}): ${res.statusText}`,
    );
  }
}
