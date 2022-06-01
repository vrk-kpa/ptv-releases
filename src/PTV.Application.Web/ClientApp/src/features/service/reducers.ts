import { Draft } from 'immer';
import { Action } from './DispatchContextProvider';
import { IServiceContext } from './ServiceContextProvider';

export function serviceContextReducer(draft: Draft<IServiceContext>, action: Action): void {
  switch (action.type) {
    case 'ValidateService':
      draft.validationResult = action.payload;
      break;
    default:
      throw Error(`Unsupported action: ${action.type}`);
  }
}
