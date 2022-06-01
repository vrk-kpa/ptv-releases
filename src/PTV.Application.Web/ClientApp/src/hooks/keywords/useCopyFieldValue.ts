import { useContext, useEffect } from 'react';
import { DispatchContext } from 'features/service/contexts/ClassificationItems/DispatchContextProvider';
import { setItems, setOtherItems } from 'features/service/contexts/ClassificationItems/actions';

export const useCopyFieldValue = (fieldValue: string[], isOther = false): void => {
  const dispatch = useContext(DispatchContext);

  useEffect(() => {
    isOther ? setOtherItems(dispatch, fieldValue) : setItems(dispatch, fieldValue);
  }, [isOther, dispatch, fieldValue]);
};
