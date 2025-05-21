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
  | 'depositByCreditProgram'
  | 'depositAndCreditProgramByCurrency';
export type ReportFormat = 'word' | 'excel' | 'pdf';

export async function sendReportByEmail(
  reportType: ReportType,
  format: ReportFormat,
  email: string,
  params?: ReportParams,
): Promise<void> {
  const actionMap: Record<ReportType, Record<ReportFormat, string>> = {
    depositByCreditProgram: {
      word: 'SendReportDepositByCreditProgram',
      excel: 'SendExcelReportDepositByCreditProgram',
      pdf: 'SendReportDepositByCreditProgram',
    },
    depositAndCreditProgramByCurrency: {
      word: 'SendReportByCurrency',
      excel: 'SendReportByCurrency',
      pdf: 'SendReportByCurrency',
    },
  };

  const action = actionMap[reportType][format];
  const res = await fetch(`${API_URL}/api/Report/${action}`, {
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
