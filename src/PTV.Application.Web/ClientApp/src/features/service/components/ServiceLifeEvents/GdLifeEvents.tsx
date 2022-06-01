import React from 'react';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { ViewValueList } from 'fields';
import { MultiSelectData, Text } from 'suomifi-ui-components';
import { GDChip } from 'components/GDChip';
import { useFormMetaContext } from 'context/formMeta';

type GdLifeEventsProps = {
  items: MultiSelectData[];
};

export function GdLifeEvents(props: GdLifeEventsProps): React.ReactElement {
  const { mode } = useFormMetaContext();
  const { t } = useTranslation();

  if (mode === 'view') {
    const values = props.items.map((value) => value.labelText);
    return (
      <ViewValueList id={'gd-lifeevents.list-view'} labelText={t('Ptv.Service.Form.Field.LifeEvents.SelectionGD.Label')} values={values} />
    );
  }

  return (
    <Box mt={2}>
      <Text variant='bold' smallScreen>
        {t('Ptv.Service.Form.Field.LifeEvents.SelectionGD.Label')}
      </Text>
      <Box>
        {props.items.map((item) => {
          return <GDChip key={item.uniqueItemId}>{item.labelText}</GDChip>;
        })}
      </Box>
    </Box>
  );
}
