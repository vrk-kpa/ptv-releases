import React, { FunctionComponent } from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { Text } from 'suomifi-ui-components';

interface NoValueLabelInterface {
  mandatory?: boolean;
}

const useStyles = makeStyles((theme) => ({
  optional: {
    '&.custom': {
      color: theme.suomifi.values.colors.depthBase.hsl,
    },
  },
  mandatory: {
    '&.custom': {
      color: theme.suomifi.values.colors.warningBase.hsl,
    },
  },
}));

export const NoValueLabel: FunctionComponent<NoValueLabelInterface> = ({ mandatory, children }) => {
  const { t } = useTranslation();
  const classes = useStyles();

  const className = clsx('custom', mandatory ? classes.mandatory : classes.optional);
  return (
    <Text variant='body' smallScreen className={className}>
      {t('Ptv.Form.Field.Optional.EmptyMessage')}
    </Text>
  );
};
