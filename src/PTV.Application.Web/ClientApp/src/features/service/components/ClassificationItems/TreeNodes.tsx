import React, { FunctionComponent, useContext } from 'react';
import { useTranslation } from 'react-i18next';
import { AppContext } from 'context/AppContextProvider';
import { usePrepareClassificationItems } from 'hooks/classificationItems/usePrepareClassificationItems';
import { useSortClassificationItems } from 'hooks/useSortClassificationItems';
import { TreeSettings, createTree } from 'utils/dataHelpers';
import { ClassificationItemsContext } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';
import { TreeNodeMain } from './TreeNodeMain';

export const TreeNodes: FunctionComponent = () => {
  const appContext = useContext(AppContext);
  const { classification } = useContext(ClassificationItemsContext);
  const { t } = useTranslation();

  const settings: TreeSettings = {
    mainCategoryNames: {
      en: t('Ptv.Service.Form.Field.ServiceClasses.MainCategory.Title', { lng: 'en' }),
      fi: t('Ptv.Service.Form.Field.ServiceClasses.MainCategory.Title', { lng: 'fi' }),
      sv: t('Ptv.Service.Form.Field.ServiceClasses.MainCategory.Title', { lng: 'sv' }),
    },
    subCategoryNames: {
      en: t('Ptv.Service.Form.Field.ServiceClasses.SubCategory.Title', { lng: 'en' }),
      fi: t('Ptv.Service.Form.Field.ServiceClasses.SubCategory.Title', { lng: 'fi' }),
      sv: t('Ptv.Service.Form.Field.ServiceClasses.SubCategory.Title', { lng: 'sv' }),
    },
  };

  const data = createTree(classification, appContext.staticData[classification], settings);

  const filteredItems = usePrepareClassificationItems(data);
  const sortedFilteredItems = useSortClassificationItems(filteredItems);

  return (
    <div>
      {sortedFilteredItems.map((item) => (
        <TreeNodeMain key={item.id} item={item} />
      ))}
    </div>
  );
};
