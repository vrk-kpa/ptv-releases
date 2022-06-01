import React, { ReactNode } from 'react';

export type TabPanelProps<T> = {
  tab: T;
  selectedTab: T;
  children: ReactNode;
};

export function TabPanel<T>(props: TabPanelProps<T>): React.ReactElement {
  const { children, tab, selectedTab } = props;

  // See https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/hidden
  // it says hidden should not be used "hiding panels in a tabbed dialog box".
  // If returning empty fragment becomes an issue we can try the hidden attribute
  if (tab !== selectedTab) return <></>;

  return <div>{children}</div>;
}
