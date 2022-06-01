import { DispatchContext } from './DispatchFormMetaContext';
import { FormMetaContextProvider, createInitialState } from './FormMetaContext';
import {
  cancelModification,
  changeCompareLanguage,
  selectLanguage,
  setFirstTabAsSelected,
  setServerError,
  switchCompareMode,
  switchFormMode,
  switchFormModeAndSelectLanguage,
} from './actions';
import { useFormMetaContext } from './useFormMetaContext';
import { useGetCompareFieldId } from './useGetCompareFieldId';
import { useGetCompareFieldName } from './useGetCompareFieldName';
import { useGetFieldId } from './useGetFieldId';
import { useGetFieldName } from './useGetFieldName';
import { useGetSelectedLanguage } from './useGetSelectedLanguage';
import { useGetServerError } from './useGetServerError';

export {
  useFormMetaContext,
  useGetFieldId,
  useGetFieldName,
  useGetSelectedLanguage,
  DispatchContext,
  FormMetaContextProvider,
  useGetCompareFieldId,
  useGetCompareFieldName,
  createInitialState,
  switchFormMode,
  switchFormModeAndSelectLanguage,
  changeCompareLanguage,
  setFirstTabAsSelected,
  selectLanguage,
  switchCompareMode,
  useGetServerError,
  setServerError,
  cancelModification,
};
