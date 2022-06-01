import React from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { Text } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';

const useStyles = makeStyles((theme) => ({
  tabHeader: {
    display: 'flex',
    userSelect: 'none',
    alignItems: 'center',
    cursor: 'pointer',
    paddingBottom: '14px',
    marginRight: '30px',
    borderBottom: '5px',
    fontSize: '16px',
    textTransform: 'uppercase',
  },
  selectedTabHeader: {
    borderBottom: '5px solid rgb(42, 110, 187)',
    fontWeight: 600,
  },
  languageName: {
    display: 'flex',
  },
}));

type TabHeaderProps = {
  tabId: Language | undefined;
  selected: boolean;
  title: string;
  onClick?: (tabId: Language) => void;
};

export function TabHeader(props: TabHeaderProps): React.ReactElement {
  const classes = useStyles();

  const tabId = props.tabId;
  const headerClassName = clsx(classes.tabHeader, {
    [classes.selectedTabHeader]: props.selected,
  });

  return (
    <label className={headerClassName} role='tab' key={tabId} onClick={() => !!tabId && props.onClick?.(tabId)}>
      <div className={classes.languageName}>
        <Text>{props.title}</Text>
      </div>
    </label>
  );
}
