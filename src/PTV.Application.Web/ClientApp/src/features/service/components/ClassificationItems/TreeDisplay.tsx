import React, { FunctionComponent, useContext } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box, capitalize } from '@mui/material';
import { Paragraph } from 'suomifi-ui-components';
import { componentMode } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { HeadingWithTooltip } from 'components/HeadingWithTooltip';
import { useGdSpecificTranslationKey } from 'hooks/translations/useGdSpecificTranslationKey';
import { ClassificationItemsContext } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';
import { ModeSwitch } from './ModeSwitch';
import { TreeSelectedNodes } from './TreeSelectedNodes';

type TreeDisplayProps = {
  fieldValue: string[];
  setFieldValue: (values: string[]) => void;
  control: Control<ServiceModel>;
};

export const TreeDisplay: FunctionComponent<TreeDisplayProps> = (props: TreeDisplayProps) => {
  const { t, i18n } = useTranslation();

  const { classification, namespace } = useContext(ClassificationItemsContext);

  const descriptionKey = useGdSpecificTranslationKey(
    props.control,
    `Ptv.Service.Form.Field.${capitalize(classification)}.Display.Description`,
    `Ptv.Service.Form.Field.${capitalize(classification)}.Display.GdSelected.Description`
  );
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
        tabIndex={0}
      >
        {t(`Ptv.Service.Form.Field.${capitalize(classification)}.Display.Label`)}
      </HeadingWithTooltip>
      <Box mt={2}>
        <Paragraph>{t(descriptionKey)}</Paragraph>
      </Box>
      <Box mt={2}>
        <ModeSwitch
          label={t(`Ptv.Service.Form.Field.${capitalize(classification)}.Display.ToggleSelectMode.Label`)}
          mode={componentMode.SELECT}
          buttonKey='switch'
          id={`${namespace}.open`}
        />
      </Box>
      <TreeSelectedNodes fieldValue={props.fieldValue} setFieldValue={props.setFieldValue} />
    </Box>
  );
};
