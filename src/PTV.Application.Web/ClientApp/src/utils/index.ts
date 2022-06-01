export function getValueOrEmpty<T>(value: T[] | undefined): T[] {
  if (!value) {
    return [];
  }

  return value;
}

export function valueOrDefault<T>(value: T | undefined | null, defaultValue: T): T {
  if (!value) {
    return defaultValue;
  }

  return value;
}

export function copyToClipboard(val: string): void {
  const selBox = document.createElement('textarea');
  selBox.style.position = 'fixed';
  selBox.style.left = '0';
  selBox.style.top = '0';
  selBox.style.opacity = '0';
  selBox.value = val;
  document.body.appendChild(selBox);
  selBox.focus();
  selBox.select();
  document.execCommand('copy');
  document.body.removeChild(selBox);
}

export const getKeys = Object.keys as <T extends Record<string, unknown>>(obj: T) => Array<keyof T>;

export function padStart(value: number | string, maxLength: number, fillString?: string | undefined): string {
  return value.toString().padStart(maxLength, fillString);
}
