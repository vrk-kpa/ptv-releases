import React, { FunctionComponent, memo, useContext } from 'react';
import { Checkbox } from 'suomifi-ui-components';
import { ClassificationItem } from 'types/classificationItemsTypes';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { translateToLang } from 'utils/translations';
import { ClassificationItemsContext } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';
import { DispatchContext } from 'features/service/contexts/ClassificationItems/DispatchContextProvider';
import { addItem, removeItem } from 'features/service/contexts/ClassificationItems/actions';
import { getAreMainClassesDisabled, getIsLimitReached } from 'features/service/contexts/ClassificationItems/selectors';
import { NodeLabel } from './NodeLabel';

interface TreeNodeInterface {
  item: ClassificationItem;
  onClick?: (id: string) => void;
}

const TreeNode: FunctionComponent<TreeNodeInterface> = ({ item, onClick }) => {
  const context = useContext(ClassificationItemsContext);
  const uiLang = useGetUiLanguage();
  const dispatch = useContext(DispatchContext);
  const isMain = item.code.indexOf('.') === -1;
  const isChecked = context.selectedItems.includes(item.id) || context.gdItems.includes(item.id);
  const isDisabled =
    context.gdItems.includes(item.id) || (!isChecked && ((isMain && getAreMainClassesDisabled(context)) || getIsLimitReached(context)));
  const languageCode = useGetUiLanguage();
  const info = item?.descriptions && item?.descriptions[languageCode];

  const handleOnClick = (value: boolean, id: string) => {
    onClick && onClick(id);
    if (value) {
      addItem(dispatch, id);
    } else {
      removeItem(dispatch, id);
    }
  };

  const label = translateToLang(uiLang, item.names) ?? '';
  return (
    <Checkbox disabled={isDisabled} checked={isChecked} onClick={(e) => handleOnClick(e.checkboxState, item.id)}>
      <NodeLabel label={label} info={info} />
    </Checkbox>
  );
};

export default memo(TreeNode);
