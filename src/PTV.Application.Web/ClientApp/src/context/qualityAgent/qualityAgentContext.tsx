import React, { createContext } from 'react';
import { useImmerReducer } from 'use-immer';
import { QualityResult, SkippedIssue } from 'types/qualityAgentResponses';
import { Action, DispatchQualityAgentContext } from './dispatchQualityAgentContext';
import { qualityAgentContextReducer } from './reducers';

export interface IQualityAgentContext {
  qualityIssues: QualityResult[];
  loading: boolean;
  skippedIssues: SkippedIssue[];
}

export function createInitialState(): IQualityAgentContext {
  return {
    qualityIssues: [],
    loading: false,
    skippedIssues: [],
  };
}

export const QualityAgentContext = createContext<IQualityAgentContext | null>(null);

export const useQualityAgentContextReducer = (initialState: IQualityAgentContext): [IQualityAgentContext, React.Dispatch<Action>] =>
  useImmerReducer(qualityAgentContextReducer, initialState);

type QualityAgentContextProviderProps = {
  children: React.ReactNode;
  initialState: IQualityAgentContext;
};

export function QualityAgentContextProvider(props: QualityAgentContextProviderProps): React.ReactElement {
  const [state, dispatch] = useQualityAgentContextReducer(props.initialState);
  return (
    <QualityAgentContext.Provider value={state}>
      <DispatchQualityAgentContext.Provider value={dispatch}>{props.children}</DispatchQualityAgentContext.Provider>
    </QualityAgentContext.Provider>
  );
}
