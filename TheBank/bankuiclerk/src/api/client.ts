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

// Функция для получения файлов отчетов
export async function getFileData(path: string): Promise<{
  blob: Blob;
  fileName: string;
  mimeType: string;
}> {
  const res = await fetch(`${API_URL}/${path}`, {
    credentials: 'include',
    // Убираем заголовок Content-Type для GET запросов файлов
  });

  if (!res.ok) {
    throw new Error(`Не получается загрузить файл ${path}: ${res.statusText}`);
  }

  const blob = await res.blob();
  const contentDisposition = res.headers.get('Content-Disposition');
  const contentType =
    res.headers.get('Content-Type') || 'application/octet-stream';

  let fileName = 'report';
  if (contentDisposition) {
    const fileNameMatch = contentDisposition.match(
      /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/,
    );
    if (fileNameMatch) {
      fileName = fileNameMatch[1].replace(/['"]/g, '');
    }
  }

  // Если имя файла не извлечено из заголовка, пытаемся определить по URL
  if (fileName === 'report') {
    if (path.includes('LoadClientsByCreditProgram')) {
      fileName = 'clientsbycreditprogram.docx';
    } else if (path.includes('LoadExcelClientByCreditProgram')) {
      fileName = 'clientsbycreditprogram.xlsx';
    } else if (path.includes('LoadClientsByDeposit')) {
      fileName = 'clientbydeposit.pdf';
    }
  }

  return {
    blob,
    fileName,
    mimeType: contentType,
  };
}

// Функция для отправки email с отчетами
export async function postEmailData<T>(path: string, data: T) {
  const res = await fetch(`${API_URL}/${path}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify(data),
  });
  if (!res.ok) {
    throw new Error(`Не получается отправить email ${path}: ${res.statusText}`);
  }
}
