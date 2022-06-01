import { Draft } from 'immer';
import { assertUnreachable } from 'utils/reducer';
import { IConnectionContext } from 'features/connection/context/ConnectionContextProvider';
import { Action } from 'features/connection/context/ConnectionDispatchContextProvider';

// TODO: Reducer will be implemented during connection details development

// eslint-disable-next-line @typescript-eslint/no-empty-function
export function connectionContextReducer(draft: Draft<IConnectionContext>, action: Action): void {
  assertUnreachable(action as never);
}
