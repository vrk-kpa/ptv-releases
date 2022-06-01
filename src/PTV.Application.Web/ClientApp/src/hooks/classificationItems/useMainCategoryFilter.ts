import { useContext, useEffect, useState } from 'react';
import { ClassificationItem } from 'types/classificationItemsTypes';
import { ClassificationItemTypes } from 'types/enumTypes';
import { cService } from 'types/forms/serviceFormTypes';
import { AppContext } from 'context/AppContextProvider';

export const useMainCategoryFilter = (classification: ClassificationItemTypes): string[] => {
  const appContext = useContext(AppContext);
  const [result, setResult] = useState<string[]>([]);
  const data = appContext.staticData[classification];

  useEffect(() => {
    if (classification === cService.serviceClasses) {
      const mainCategoryIds = (data || []).reduce((mainCategoryIds: string[], item: ClassificationItem) => {
        return item.parentId ? mainCategoryIds : [...mainCategoryIds, item.id];
      }, []);

      setResult(mainCategoryIds);
    }
  }, [data, classification]);
  return result;
};
