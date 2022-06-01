import React from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { Icon } from 'suomifi-ui-components';
import { PublishingStatus } from 'types/enumTypes';

type CellWithStatusProps = {
  status: PublishingStatus;
  value: string;
  id?: string;
};

const useStyles = makeStyles((theme) => ({
  iconBase: {
    width: '10px',
    height: '10px',
    marginRight: '10px',
  },
  statusDraft: {
    verticalAlign: 'middle !important',
    color: theme.colors.draft,
  },
  statusPublished: {
    verticalAlign: 'middle !important',
    color: theme.colors.published,
  },
  statusArchived: {
    verticalAlign: 'middle !important',
    color: theme.colors.archived,
  },
}));

export default function CellWithStatus(props: CellWithStatusProps): React.ReactElement {
  const classes = useStyles();

  const getStatusClassName = (status: PublishingStatus): string => {
    switch (status) {
      case 'Deleted':
      case 'OldPublished':
        return classes.statusArchived;
      case 'Draft':
      case 'Modified':
        // All status except for Published should be gray?
        return classes.statusArchived;
      // return cellClasses.statusDraft;
      case 'Published':
        return classes.statusPublished;
      default:
        return '';
    }
  };

  return (
    <div id={props.id}>
      <Icon icon='radioButtonOn' className={clsx(getStatusClassName(props.status), classes.iconBase)} />
      <span>{props.value}</span>
    </div>
  );
}
