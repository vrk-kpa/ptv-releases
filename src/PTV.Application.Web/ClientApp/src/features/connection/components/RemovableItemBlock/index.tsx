import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Button } from 'suomifi-ui-components';

const useStyles = makeStyles((theme) => ({
  item: {
    display: 'flex',
    flexGrow: 1,
    border: `1px solid rgb(200, 205, 208)`,
    padding: '20px',
    marginBottom: '20px',
  },
  left: {
    display: 'flex',
    flexGrow: 1,
  },
  right: {
    display: 'flex',
    flexShrink: 0,
    marginLeft: 'auto',
    alignItems: 'flex-start',
  },
}));

type RemovableItemBlockProps = {
  children: React.ReactNode;
  onRemove: () => void;
};

export function RemovableItemBlock(props: RemovableItemBlockProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();

  return (
    <div className={classes.item}>
      <div className={classes.left}>{props.children}</div>
      <div className={classes.right}>
        <Button onClick={props.onRemove} icon='remove' variant='secondaryNoBorder'>
          {t('Ptv.Common.Remove')}
        </Button>
      </div>
    </div>
  );
}
