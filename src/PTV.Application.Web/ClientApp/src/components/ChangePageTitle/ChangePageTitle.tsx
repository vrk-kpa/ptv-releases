import React, { useEffect } from 'react';

export type ChangePageTitleProps = {
  pageTitle: string;
};

const ChangePageTitle = (props: ChangePageTitleProps): React.ReactElement => {
  useEffect(() => {
    const { title } = document;
    document.title = props.pageTitle;
    return () => {
      document.title = title;
    };
  }, [props.pageTitle]);

  return <></>;
};

export default ChangePageTitle;
