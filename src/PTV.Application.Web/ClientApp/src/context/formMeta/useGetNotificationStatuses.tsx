import { NotificationStatuses } from 'types/notificationStatus';
import { useFormMetaContext } from './';

export function useGetNotificationStatuses(): NotificationStatuses {
  const ctx = useFormMetaContext();
  return ctx.notificationStatuses;
}
