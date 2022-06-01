type ComponentStatusText = string | undefined;
type ComponentStatus = 'default' | 'error';

export function getStatus(error: string | undefined | null, touched: boolean): ComponentStatus {
  return error && touched ? 'error' : 'default';
}

export function getStatusText(status: ComponentStatus, statusText: ComponentStatusText): ComponentStatusText {
  return status === 'error' ? statusText : undefined;
}
