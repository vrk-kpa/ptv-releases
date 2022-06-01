import React from 'react';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { ViewValueList } from 'fields';
import { MultiSelectData, Text } from 'suomifi-ui-components';
import { CustomChip } from 'components/CustomChip';
import { useFormMetaContext } from 'context/formMeta';

type LifeEventsListProps = {
  items: MultiSelectData[];
  remove: (id: string) => void;
};

export function LifeEventsList(props: LifeEventsListProps): React.ReactElement {
  const { mode } = useFormMetaContext();
  const { t } = useTranslation();

  if (mode === 'view') {
    const values = props.items.map((value) => value.labelText);
    return (
      <ViewValueList
        id={`service-lifeevents.list-view`}
        labelText={t('Ptv.Service.Form.Field.LifeEvents.Selection.Label')}
        values={values}
      />
    );
  }

  return (
    <Box mt={2}>
      <Text variant='bold' smallScreen>
        {t('Ptv.Service.Form.Field.LifeEvents.Selection.Label')}
      </Text>
      <Box>
        {props.items.length === 0 ? (
          <Text smallScreen>{t('Ptv.Service.Form.Field.LifeEvents.Selection.Empty')}</Text>
        ) : (
          props.items.map((item: MultiSelectData) => (
            <CustomChip key={item.uniqueItemId} onClick={() => props.remove(item.uniqueItemId)}>
              {item.labelText}
            </CustomChip>
          ))
        )}
      </Box>
    </Box>
  );
}
