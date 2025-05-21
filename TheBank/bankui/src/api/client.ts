import { ConfigManager } from '@/lib/config';
import type { MailSendInfoBindingModel } from '@/types/types';
const API_URL = ConfigManager.loadUrl();
// Устанавливаем прямой URL к API серверу ASP.NET
// const API_URL = 'https://localhost:7224'; // URL API сервера ASP.NET

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
  mailInfo: MailSendInfoBindingModel,
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

  // Формируем тело запроса
  const requestBody = { ...mailInfo, ...params };

  const res = await fetch(`${API_URL}/api/Report/${action}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify(requestBody),
  });

  if (!res.ok) {
    throw new Error(
      `Не удалось отправить отчет ${reportType} (${format}): ${res.statusText}`,
    );
  }
}
