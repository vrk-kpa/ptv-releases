import React, { FunctionComponent } from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { Chip } from 'suomifi-ui-components';

const useStyles = makeStyles((theme) => ({
  chip: {
    '&.custom': {
      margin: '10px 10px 0 0',
    },
    '&.custom.custom': {
      backgroundColor: theme.suomifi.values.colors.alertBase.hsl,
    },
  },
}));

interface InvalidChipInterface {
  onClick: () => void;
}

export const InvalidChip: FunctionComponent<InvalidChipInterface> = ({ onClick, children }) => {
  const classes = useStyles();
  const { t } = useTranslation();
  const chipCSS = clsx(classes.chip, 'custom');

  return (
    <Chip className={chipCSS} actionLabel={t('Ptv.Form.Chip.ActionLabel.Remove')} removable onClick={onClick}>
      {children}
    </Chip>
  );
};
