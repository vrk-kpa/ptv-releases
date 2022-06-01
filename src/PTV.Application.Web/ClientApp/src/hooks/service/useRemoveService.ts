import { UseMutationResult, useMutation } from 'react-query';
import { useNavigate } from 'react-router';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { HttpError } from 'types/miscellaneousTypes';
import { post } from 'utils/request';

export const useRemoveService = (): UseMutationResult<ServiceApiModel, HttpError, string, unknown> => {
  const navigate = useNavigate();

  const mutation = useMutation<ServiceApiModel, HttpError, string, unknown>(
    (id: string) => {
      return post<ServiceApiModel>(`next/service/remove/${id}`);
    },
    {
      onSuccess: () => navigate('/frontpage/search'),
    }
  );

  return mutation;
};
