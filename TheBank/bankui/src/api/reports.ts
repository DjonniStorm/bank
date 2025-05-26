import { ConfigManager } from '@/lib/config';

const API_URL = ConfigManager.loadUrl();

interface SendEmailRequest {
  toEmail: string;
  creditProgramIds?: string[];
}

export const reportsApi = {
  // PDF отчеты
  getPdfReport: async (fromDate: string, toDate: string) => {
    const res = await fetch(
      `${API_URL}/api/Report/LoadDepositAndCreditProgramByCurrency?fromDate=${fromDate}&toDate=${toDate}`,
      {
        credentials: 'include',
        headers: {
          'Content-Type': 'application/octet-stream',
        },
      },
    );
    if (!res.ok) {
      throw new Error(`Не удалось загрузить PDF отчет: ${res.statusText}`);
    }
    return res.blob();
  },

  sendPdfReportByEmail: async (
    toEmail: string,
    fromDate: string,
    toDate: string,
  ) => {
    const res = await fetch(
      `${API_URL}/api/Report/SendReportByCurrency?fromDate=${fromDate}&toDate=${toDate}`,
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({
          email: toEmail,
        }),
      },
    );
    if (!res.ok) {
      throw new Error(`Не удалось отправить PDF отчет: ${res.statusText}`);
    }
  },

  // Word отчеты
  getWordReport: async (creditProgramIds: string[]) => {
    const idsParam = creditProgramIds.join(',');
    const res = await fetch(
      `${API_URL}/api/Report/LoadDepositByCreditProgram?creditProgramIds=${idsParam}`,
      {
        credentials: 'include',
        headers: {
          'Content-Type': 'application/octet-stream',
        },
      },
    );
    if (!res.ok) {
      throw new Error(`Не удалось загрузить Word отчет: ${res.statusText}`);
    }
    return res.blob();
  },

  sendWordReportByEmail: async (request: SendEmailRequest) => {
    const res = await fetch(
      `${API_URL}/api/Report/SendReportDepositByCreditProgram`,
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({
          email: request.toEmail,
          creditProgramIds: request.creditProgramIds,
        }),
      },
    );
    if (!res.ok) {
      throw new Error(`Не удалось отправить Word отчет: ${res.statusText}`);
    }
  },

  // Excel отчеты
  getExcelReport: async (creditProgramIds: string[]) => {
    const idsParam = creditProgramIds.join(',');
    const res = await fetch(
      `${API_URL}/api/Report/LoadExcelDepositByCreditProgram?creditProgramIds=${idsParam}`,
      {
        credentials: 'include',
        headers: {
          'Content-Type': 'application/octet-stream',
        },
      },
    );
    if (!res.ok) {
      throw new Error(`Не удалось загрузить Excel отчет: ${res.statusText}`);
    }
    return res.blob();
  },

  sendExcelReportByEmail: async (request: SendEmailRequest) => {
    const res = await fetch(
      `${API_URL}/api/Report/SendExcelReportDepositByCreditProgram`,
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({
          email: request.toEmail,
          creditProgramIds: request.creditProgramIds,
        }),
      },
    );
    if (!res.ok) {
      throw new Error(`Не удалось отправить Excel отчет: ${res.statusText}`);
    }
  },

  // Получение данных для предпросмотра
  getReportData: async (creditProgramIds: string[]) => {
    const idsParam = creditProgramIds.join(',');
    const res = await fetch(
      `${API_URL}/api/Report/GetDepositByCreditProgram?creditProgramIds=${idsParam}`,
      {
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json',
        },
      },
    );
    if (!res.ok) {
      throw new Error(`Не удалось загрузить данные отчета: ${res.statusText}`);
    }
    return res.json();
  },

  getReportDataByCurrency: async (fromDate: string, toDate: string) => {
    const res = await fetch(
      `${API_URL}/api/Report/GetDepositAndCreditProgramByCurrency?fromDate=${fromDate}&toDate=${toDate}`,
      {
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json',
        },
      },
    );
    if (!res.ok) {
      throw new Error(
        `Не удалось загрузить данные отчета по валюте: ${res.statusText}`,
      );
    }
    return res.json();
  },
};
