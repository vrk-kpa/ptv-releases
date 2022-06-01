import { ServiceApiModel } from 'types/api/serviceApiModel';
import { ValidationModel } from 'types/api/validationModel';
import { Dispatch } from './DispatchContextProvider';

export function ValidateService(dispatch: Dispatch, payload: ValidationModel<ServiceApiModel> | null): void {
  dispatch({
    type: 'ValidateService',
    payload: payload,
  });
}
