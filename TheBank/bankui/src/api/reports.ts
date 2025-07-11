import { ConfigManager } from '@/lib/config';
import type {
  CreditProgramReportMailSendInfoBindingModel,
  DepositReportMailSendInfoBindingModel,
} from '@/types/types';

const API_URL = ConfigManager.loadUrl();

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
    mailInfo: DepositReportMailSendInfoBindingModel,
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
        body: JSON.stringify(mailInfo),
      },
    );
    if (!res.ok) {
      throw new Error(`Не удалось отправить PDF отчет: ${res.statusText}`);
    }
  },

  // Word отчеты
  getWordReport: async (creditProgramIds: string[]) => {
    const idsParam = creditProgramIds.reduce((prev, curr, index) => {
      return (prev += `${index === 0 ? '' : '&'}creditProgramIds=${curr}`);
    }, '');
    console.log('idsParam', idsParam);
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

  sendWordReportByEmail: async (
    mailInfo: CreditProgramReportMailSendInfoBindingModel,
  ) => {
    const res = await fetch(
      `${API_URL}/api/Report/SendReportDepositByCreditProgram`,
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify(mailInfo),
      },
    );
    if (!res.ok) {
      throw new Error(`Не удалось отправить Word отчет: ${res.statusText}`);
    }
  },

  // Excel отчеты
  getExcelReport: async (creditProgramIds: string[]) => {
    const idsParam = creditProgramIds.reduce((prev, curr, index) => {
      return (prev += `${index === 0 ? '' : '&'}creditProgramIds=${curr}`);
    }, '');
    const res = await fetch(
      `${API_URL}/api/Report/LoadExcelDepositByCreditProgram?${idsParam}`,
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

  sendExcelReportByEmail: async (
    mailInfo: CreditProgramReportMailSendInfoBindingModel,
  ) => {
    const res = await fetch(
      `${API_URL}/api/Report/SendExcelReportDepositByCreditProgram`,
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify(mailInfo),
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
