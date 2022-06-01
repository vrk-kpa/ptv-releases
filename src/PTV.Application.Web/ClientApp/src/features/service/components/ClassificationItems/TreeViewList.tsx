import React, { FunctionComponent, useContext } from 'react';
import { ViewValueList } from 'fields';
import { union } from 'lodash';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { translateToLang } from 'utils/translations';
import { ClassificationItemsContext } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';
import { getClassificationItems } from './utils';

interface TreeViewListInterface {
  labelText: string;
  fieldValue: string[];
}

export const TreeViewList: FunctionComponent<TreeViewListInterface> = ({ labelText, fieldValue }) => {
  const { classification, gdItems, namespace } = useContext(ClassificationItemsContext);
  const appContext = useAppContextOrThrow();
  const uiLang = useGetUiLanguage();

  const items = getClassificationItems(union(fieldValue, gdItems), appContext.staticData, classification);
  const values = items.map((x) => translateToLang(uiLang, x.names) ?? '');

  return <ViewValueList id={`${namespace}.${classification}`} labelText={labelText} values={values} />;
};
