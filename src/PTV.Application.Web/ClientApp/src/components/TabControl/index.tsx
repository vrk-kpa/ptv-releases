import React, { Children, ReactNode } from 'react';
import { useTranslation } from 'react-i18next';
import Grid from '@mui/material/Grid';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { Checkbox } from 'suomifi-ui-components';
import { Language, PublishingStatus } from 'types/enumTypes';
import { FormDivider } from 'components/formLayout/FormDivider';
import { getKeyForLanguage } from 'utils/translations';
import { TabHeader } from './TabHeader';

const useStyles = makeStyles(() => ({
  flex: {
    /*display: 'flex',
    flex: 1,*/
  },
  tabControlRoot: {
    display: 'flex',
    flexWrap: 'wrap',
    flex: 1,
    flexDirection: 'column',
  },
  visibleTab: {
    /*display: 'flex',
    flex: 1,*/
  },
  hiddenTab: {
    display: 'none',
  },
  tabHeaderList: {
    display: 'flex',
    flexDirection: 'row',
    flexWrap: 'wrap',
  },
  tabHeaderButtonArea: {
    display: 'flex',
  },
  tabButtonContainer: {
    display: 'flex',
    marginLeft: '20px',
  },
  tabButtonList: {
    marginLeft: 'auto',
  },
  abort: {
    display: 'flex',
    userSelect: 'none',
    cursor: 'pointer',
  },
}));

export type TabItemProps = {
  tabId: Language | undefined;
  status?: PublishingStatus | null;
  language: Language;
  children: React.ReactNode;
  isScheduled: boolean;
};

export function TabItem(props: TabItemProps): React.ReactElement {
  const classes = useStyles();
  return <div className={classes.flex}>{props.children}</div>;
}

type TabControlProps = {
  selectedLanguage: Language;
  tabSelected?: (tabId: string) => void;
  children: React.ReactElement<TabItemProps> | React.ReactElement<TabItemProps>[];
  isComparing: boolean;
  toggleCompare: () => void;
  languageVersionsCount: number;
};

export function TabControl(props: TabControlProps): React.ReactElement {
  const classes = useStyles();
  const { t } = useTranslation();

  let tabs: React.ReactElement<TabItemProps>[] = [];
  tabs = tabs.concat(props.children);

  const selectedLanguage = props.selectedLanguage;

  function tabHeaderClick(tabId: string) {
    props.tabSelected?.(tabId);
  }

  function renderTabs(children: React.ReactElement<TabItemProps>[]) {
    const tabs: ReactNode[] = [];

    children.forEach((item) => {
      const isTabSelected = item.props.tabId === selectedLanguage;
      const className = clsx(classes.visibleTab, { [classes.hiddenTab]: !isTabSelected });
      tabs.push(
        <div role='tab' key={item.props.tabId} className={className}>
          {item}
        </div>
      );
    });

    return tabs;
  }

  function renderTabHeaders(children: React.ReactElement<TabItemProps>[]) {
    const headerList: ReactNode[] = [];

    Children.map(children, (item) => {
      const tabId = item.props.tabId;
      const language = item.props.language;

      headerList.push(
        <Grid item key={item.props.tabId}>
          <TabHeader onClick={tabHeaderClick} tabId={tabId} selected={tabId === selectedLanguage} title={t(getKeyForLanguage(language))} />
        </Grid>
      );
    });

    return headerList;
  }

  const tabHeaders = renderTabHeaders(tabs);
  const tabItems = renderTabs(tabs);

  return (
    <div className={classes.tabControlRoot}>
      <Grid container wrap='wrap' direction='row' alignItems='center' justifyContent='space-between'>
        <div className={classes.tabHeaderList}>{tabHeaders}</div>
        <Grid item>
          <Checkbox onClick={props.toggleCompare} checked={props.isComparing} disabled={props.languageVersionsCount <= 1}>
            {t('Ptv.TabControl.ShowComparison')}
          </Checkbox>
        </Grid>
      </Grid>
      <FormDivider mt={0} />
      <div className={classes.flex}>{tabItems}</div>
    </div>
  );
}
