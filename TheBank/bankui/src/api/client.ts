import { ConfigManager } from '@/lib/config';

const API_URL = ConfigManager.loadUrl();

export async function getData<T>(path: string): Promise<T[]> {
  const res = await fetch(`${API_URL}/${path}`);
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
    },
    body: JSON.stringify(data),
  });
  if (!res.ok) {
    throw new Error(`Не получается загрузить ${path}: ${res.statusText}`);
  }
}

export async function putData<T>(path: string, data: T) {
  const res = await fetch(`${API_URL}/${path}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(data),
  });
  if (!res.ok) {
    throw new Error(`Не получается загрузить ${path}: ${res.statusText}`);
  }
}
