import React, { FunctionComponent, useContext } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box, capitalize } from '@mui/material';
import { DefaultTheme, makeStyles } from '@mui/styles';
import { Block, Modal, ModalFooter, ModalTitle, Paragraph } from 'suomifi-ui-components';
import { componentMode } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { useCopyFieldValue } from 'hooks/keywords/useCopyFieldValue';
import { useGdSpecificTranslationKey } from 'hooks/translations/useGdSpecificTranslationKey';
import { SearchFilter } from 'features/service/components/SearchFilter';
import { ClassificationItemsContext } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';
import { DispatchContext } from 'features/service/contexts/ClassificationItems/DispatchContextProvider';
import { toggleMode } from 'features/service/contexts/ClassificationItems/actions';
import { getSelectedItemsUnion } from 'features/service/contexts/ClassificationItems/selectors';
import { ModeSwitch } from './ModeSwitch';
import { TreeActions } from './TreeActions';
import { TreeNodes } from './TreeNodes';
import { TreeSelectedNodes } from './TreeSelectedNodes';

const useStyles = makeStyles<DefaultTheme, StyleProps>((theme) => ({
  scrollableArea: {
    height: ({ height }) => height,
    overflow: 'auto',
  },
  searchFilter: {
    marginBottom: '20px',
    paddingBottom: '20px',
    borderBottom: `0.5px solid ${theme.suomifi.values.colors.blackLight1.hsl}`,
  },
  modalHead: {
    padding: '24px 30px 0',
  },
}));

export interface StyleProps {
  height: number;
}

type TreeSelectProps = {
  setFieldValue: (values: string[]) => void;
  fieldValue: string[];
  control: Control<ServiceModel>;
};

export const TreeSelect: FunctionComponent<TreeSelectProps> = (props: TreeSelectProps) => {
  const classes = useStyles({ height: 740 });
  const context = useContext(ClassificationItemsContext);
  const { elementMode, classification, namespace } = context;
  const { t } = useTranslation();
  const dispatch = useContext(DispatchContext);

  useCopyFieldValue(props.fieldValue);

  const descriptionKey = useGdSpecificTranslationKey(
    props.control,
    `Ptv.Service.Form.Field.${capitalize(classification)}.Select.Description`,
    `Ptv.Service.Form.Field.${capitalize(classification)}.Select.GdSelected.Description`
  );

  const handleClose = () => {
    toggleMode(dispatch, componentMode.DISPLAY);
  };

  const isOpen = elementMode === componentMode.SELECT || elementMode === componentMode.SUMMARY;
  const allItems = getSelectedItemsUnion(context);

  return (
    <Modal appElementId={'root'} visible={isOpen} onEscKeyDown={handleClose}>
      <Box className={classes.scrollableArea}>
        {(elementMode === componentMode.SELECT && (
          <Block>
            <Box className={classes.modalHead}>
              <ModalTitle>{t(`Ptv.Service.Form.Field.${capitalize(classification)}.Select.Label`)}</ModalTitle>
              {classification === cService.serviceClasses && (
                <Box mb={2}>
                  <Paragraph>{t(descriptionKey)}</Paragraph>
                </Box>
              )}
            </Box>
            <Box pb={5} px={3}>
              <Box className={classes.searchFilter}>
                <SearchFilter
                  labelText={t(`Ptv.Service.Form.Field.${capitalize(classification)}.Select.Label`)}
                  visualPlaceholder={t('Ptv.SearchFilter.Placeholder')}
                  clearButtonLabel={t('Ptv.SearchFilter.Clear')}
                  searchButtonLabel={t('Ptv.SearchFilter.Confirm')}
                />
              </Box>
              <TreeNodes />
            </Box>
          </Block>
        )) || (
          <Box pt={2} px={3} className={classes.scrollableArea}>
            <ModeSwitch
              label={t(`Ptv.Service.Form.Field.${capitalize(classification)}.Summary.GoToSelect.Label`)}
              mode={componentMode.SELECT}
              buttonKey='backward'
              variant='secondaryNoBorder'
              icon='arrowLeft'
              id={`${namespace}.${classification}-selection`}
            />
            <TreeSelectedNodes fieldValue={props.fieldValue} setFieldValue={props.setFieldValue} />
          </Box>
        )}
      </Box>
      <ModalFooter>
        {elementMode === componentMode.SELECT && (
          <ModeSwitch
            label={t(`Ptv.Service.Form.Field.${capitalize(classification)}.Select.GoToSummary.Label`, {
              count: allItems.length,
            })}
            mode={componentMode.SUMMARY}
            buttonKey='forward'
            variant='secondaryNoBorder'
            iconRight='arrowRight'
            id={`${namespace}.${classification}-summary`}
          />
        )}
        <TreeActions setFieldValue={props.setFieldValue} fieldValue={props.fieldValue} />
      </ModalFooter>
    </Modal>
  );
};
