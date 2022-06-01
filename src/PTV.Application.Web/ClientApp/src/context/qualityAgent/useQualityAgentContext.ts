import { useContext } from 'react';
import { IQualityAgentContext, QualityAgentContext } from './qualityAgentContext';

export function useQualityAgentContext(): IQualityAgentContext {
  const ctx = useContext(QualityAgentContext);
  if (!ctx) {
    throw Error('QualityAgentContext has not been provided.');
  }
  return ctx;
}
