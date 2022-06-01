import React, { FunctionComponent } from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { RhfReadOnlyField } from 'fields';
import { Block } from 'suomifi-ui-components';
import { LinkModel, cLink } from 'types/link';

const useStyles = makeStyles(() => ({
  field: {
    marginTop: '20px',
  },
  name: {
    fontWeight: 600,
  },
  preview: {
    marginTop: 0,
  },
}));

interface ServiceLawViewInterface {
  index: number;
  value: LinkModel;
  id: string;
  concise?: boolean;
}

export const ServiceLawView: FunctionComponent<ServiceLawViewInterface> = ({ value, index, id, concise }) => {
  const { t } = useTranslation();
  const classes = useStyles();
  const fieldClass = clsx(classes.field, { [classes.preview]: concise });

  return (
    <Block>
      <div className={fieldClass}>
        <RhfReadOnlyField
          value={value.name}
          id={`${id}.${index}.${cLink.name}`}
          labelText={concise ? undefined : t('Ptv.Service.Form.Field.Laws.Name.Label')}
        />
      </div>
      <div className={fieldClass}>
        <RhfReadOnlyField
          value={value.url}
          id={`${id}.${index}.${cLink.url}`}
          labelText={concise ? undefined : t('Ptv.Service.Form.Field.Laws.Url.Label')}
          asLink
        />
      </div>
    </Block>
  );
};
