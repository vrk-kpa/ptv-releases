import React, { FunctionComponent, useContext } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box, capitalize } from '@mui/material';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { HeadingWithTooltip } from 'components/HeadingWithTooltip';
import { useGdSpecificTranslationKey } from 'hooks/translations/useGdSpecificTranslationKey';
import { ClassificationItemsContext } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';
import { TreeViewList } from './TreeViewList';

type TreeViewProps = {
  fieldValue: string[];
  control: Control<ServiceModel>;
};

export const TreeView: FunctionComponent<TreeViewProps> = (props: TreeViewProps) => {
  const { t, i18n } = useTranslation();
  const { classification, namespace } = useContext(ClassificationItemsContext);

  const tooltipKey = useGdSpecificTranslationKey(
    props.control,
    `Ptv.Service.Form.Field.${capitalize(classification)}.Display.Tooltip`,
    `Ptv.Service.Form.Field.${capitalize(classification)}.Display.GdSelected.Tooltip`
  );

  return (
    <Box>
      <HeadingWithTooltip
        id={namespace}
        variant='h4'
        tooltipContent={i18n.exists(tooltipKey) ? t(tooltipKey) : ''}
        tooltipAriaLabel={t(`Ptv.Service.Form.Field.${capitalize(classification)}.Display.Label`)}
      >
        {t(`Ptv.Service.Form.Field.${capitalize(classification)}.Display.Label`)}
      </HeadingWithTooltip>
      <Box mt={2}>
        <TreeViewList
          labelText={t(`Ptv.Service.Form.Field.${capitalize(classification)}.Display.Selection.Label`)}
          fieldValue={props.fieldValue}
        />
      </Box>
    </Box>
  );
};
