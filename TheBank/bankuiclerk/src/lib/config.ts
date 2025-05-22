export class ConfigManager {
  static loadUrl(): string {
    const apiUrl = import.meta.env.VITE_API_URL;
    if (!apiUrl) {
      throw new Error('Не получается загрузить url сервера');
    }
    return apiUrl;
  }
}
