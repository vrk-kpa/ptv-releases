import React, { FunctionComponent, useContext } from 'react';
import { useTranslation } from 'react-i18next';
import { Box, capitalize } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Button } from 'suomifi-ui-components';
import { componentMode } from 'types/enumTypes';
import { Message } from 'components/Message';
import { ClassificationItemsContext } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';
import { DispatchContext } from 'features/service/contexts/ClassificationItems/DispatchContextProvider';
import { setItems, toggleMode } from 'features/service/contexts/ClassificationItems/actions';
import { getAreOnlyMainClassesSelected, getIsLimitReached } from 'features/service/contexts/ClassificationItems/selectors';

const useStyles = makeStyles(() => ({
  action: {
    display: 'inline-block',
    marginRight: '15px',
  },
  message: {
    marginBottom: '20px',
  },
}));

interface TreeActionsInterface {
  setFieldValue: (values: string[]) => void;
  fieldValue: string[];
  isModified?: boolean;
}

export const TreeActions: FunctionComponent<TreeActionsInterface> = ({ isModified, setFieldValue, fieldValue }) => {
  const classes = useStyles();
  const context = useContext(ClassificationItemsContext);
  const selectedItems = context.selectedItems;
  const { t } = useTranslation();
  const dispatch = useContext(DispatchContext);

  const modified = isModified || !(selectedItems.length === fieldValue.length && selectedItems.every((item) => fieldValue.includes(item)));

  const handleCancelSelection = () => {
    setItems(dispatch, fieldValue);
    toggleMode(dispatch, componentMode.DISPLAY);
  };

  const handleConfirmSelection = () => {
    setFieldValue(selectedItems);
    toggleMode(dispatch, componentMode.DISPLAY);
  };

  const onlyMainSelected = getAreOnlyMainClassesSelected(context);
  const limitReached = getIsLimitReached(context);
  const showMessage = onlyMainSelected || limitReached;
  const cancelButtonId = `${context.namespace}.${context.classification}-cancel`;
  const confirmButtonId = `${context.namespace}.${context.classification}-confirm`;
  return (
    <Box>
      {showMessage && (
        <div className={classes.message}>
          {onlyMainSelected && <Message type='error'>{t('Ptv.Service.Form.Field.ServiceClasses.Message.MainClassesOnly')}</Message>}
          {limitReached && (
            <Message type='error'>{t(`Ptv.Service.Form.Field.${capitalize(context.classification)}.Message.LimitReached`)}</Message>
          )}
        </div>
      )}
      <Box className={classes.action}>
        <Button key='confirm' disabled={!modified} onClick={handleConfirmSelection} id={confirmButtonId}>
          {t('Ptv.Action.ConfirmSelection.Label')}
        </Button>
      </Box>
      <Box className={classes.action}>
        <Button key='cancel' variant='secondary' onClick={handleCancelSelection} id={cancelButtonId}>
          {t('Ptv.Action.Cancel.Label')}
        </Button>
      </Box>
    </Box>
  );
};
