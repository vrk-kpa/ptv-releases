import { ServiceFormValues } from 'types/forms/serviceFormTypes';
import { HttpError } from 'types/miscellaneousTypes';
import { useGetService } from 'hooks/service/useGetService';
import { toServiceUiModel } from 'mappers/serviceMapper';

type GetServiceResult = {
  isLoading: boolean;
  error: HttpError | null;
  service: ServiceFormValues | null | undefined;
};

export default function useGetServiceUiModel(serviceId: string): GetServiceResult {
  const query = useGetService(serviceId);

  let uiModel: ServiceFormValues | null = null;
  if (!query.isLoading && !query.error && query.data) {
    uiModel = toServiceUiModel(query.data);
  }

  return {
    isLoading: query.isLoading,
    error: query.error,
    service: uiModel,
  };
}
