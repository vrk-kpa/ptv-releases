import { AppSettings } from 'types/settingTypes';

let settings: AppSettings | null = null;

export function setSettings(value: AppSettings): void {
  settings = value;
}

export function getSettings(): AppSettings {
  if (!settings) {
    throw new Error('Settings are null');
  }

  return settings;
}

export function getApiUrl(path: string): string {
  const settings = getSettings();
  if (path.startsWith('/')) {
    return `${settings.uiApiUrl}${path.substring(1)}`;
  }

  return `${settings.uiApiUrl}${path}`;
}
