import { createContext } from 'react';

// TODO: These will be filled once we have the functionality

export type Payload = unknown | null;
export type Action = unknown;
export type Dispatch = (action: Action) => void;

// eslint-disable-next-line @typescript-eslint/no-empty-function
export const ConnectionDispatchContext = createContext<Dispatch>(() => {});
