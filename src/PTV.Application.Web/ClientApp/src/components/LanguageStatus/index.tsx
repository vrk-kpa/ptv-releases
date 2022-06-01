import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { Text } from 'suomifi-ui-components';
import { Language, PublishingStatus } from 'types/enumTypes';
import { getKeyForLanguage, getKeysForStatusType } from 'utils/translations';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    alignItems: 'center',
  },
  status: {
    display: 'flex',
    marginRight: '5px',
    backgroundColor: 'rgba(237, 238, 239, 0.8)',
    justifyContent: 'center',
  },
  statusScheduledDraft: {
    borderStyle: 'solid',
    borderImage: 'linear-gradient(to right, #EA7125 45%, rgba(237, 238, 239, 0.8) 45% 55%, #00B38A 55%)',
    borderImageSlice: 6,
    borderWidth: '0 0 3px 0',
  },
  statusScheduledPublish: {
    borderStyle: 'solid',
    borderImage: 'linear-gradient(to right, #00B38A 45%, rgba(237, 238, 239, 0.8) 45% 55%, #00B38A 55%)',
    borderImageSlice: 6,
    borderWidth: '0 0 3px 0',
  },
  statusDraft: {
    borderBottom: '3px solid',
    borderRadius: '2px',
    borderBlockColor: theme.colors.draft,
  },
  statusPublished: {
    borderBottom: '3px solid',
    borderRadius: '2px',
    borderBlockColor: theme.colors.published,
  },
  statusArchived: {
    borderBottom: '3px solid',
    borderRadius: '2px',
    borderBlockColor: theme.colors.archived,
  },
  languageCode: {
    display: 'flex',
    justifyContent: 'center',
    textTransform: 'uppercase',
    paddingTop: '1px',
    paddingBottom: '1px',
    minWidth: '30px',
  },
  statusText: {
    textTransform: 'none',
  },
}));

type LanguageStatusProps = {
  language: Language;
  status: PublishingStatus;
  isScheduled: boolean;
  excludeText?: boolean | undefined;
  className?: string | undefined;
};

export function LanguageStatus(props: LanguageStatusProps): React.ReactElement {
  const classes = useStyles();
  const { t } = useTranslation();

  const statusClassName = clsx(classes.status, {
    [classes.statusScheduledDraft]: props.isScheduled && (props.status === 'Draft' || props.status === 'Modified'),
    [classes.statusScheduledPublish]: props.isScheduled && props.status === 'Published',
    [classes.statusPublished]: !props.isScheduled && props.status === 'Published',
    [classes.statusDraft]: !props.isScheduled && (props.status === 'Draft' || props.status === 'Modified'),
    [classes.statusArchived]: props.status === 'Removed' || props.status === 'OldPublished' || props.status === 'Deleted',
  });

  const langName = t(getKeyForLanguage(props.language));
  const statusText = t(getKeysForStatusType(props.status)).toLowerCase();
  const rootClass = clsx(classes.root, props.className);

  return (
    <span className={rootClass}>
      <span className={statusClassName}>
        <span className={classes.languageCode}>
          <Text variant='bold'>{props.language}</Text>
        </span>
      </span>
      {!props.excludeText && (
        <span className={classes.statusText}>
          <Text>{`${langName}, ${statusText}`}</Text>
        </span>
      )}
    </span>
  );
}
