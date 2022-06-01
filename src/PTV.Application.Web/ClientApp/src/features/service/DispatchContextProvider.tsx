import { createContext } from 'react';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { ValidationModel } from 'types/api/validationModel';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type Payload = any | null;

export type Action = { type: 'ValidateService'; payload: ValidationModel<ServiceApiModel> | null };

export type Dispatch = (action: Action) => void;

// eslint-disable-next-line @typescript-eslint/no-empty-function
export const DispatchContext = createContext<Dispatch>(() => {});
