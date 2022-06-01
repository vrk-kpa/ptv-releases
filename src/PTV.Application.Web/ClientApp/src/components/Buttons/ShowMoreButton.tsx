import React from 'react';
import { useTranslation } from 'react-i18next';
import { FetchNextPageOptions, InfiniteQueryObserverResult } from 'react-query';
import { makeStyles } from '@mui/styles';
import { Button } from 'suomifi-ui-components';
import { InfiniteModel } from 'types/api/infiniteModel';
import { HttpError } from 'types/miscellaneousTypes';
import LoadingIndicator from 'components/LoadingIndicator';

type ShowMoreButtonProps<T> = {
  isLoading: boolean;
  fetchNextPage: (options?: FetchNextPageOptions) => Promise<InfiniteQueryObserverResult<InfiniteModel<T>, HttpError>>;
};

const useStyles = makeStyles(() => ({
  showMoreWrapper: {
    display: 'flex',
    justifyContent: 'center',
    padding: '10px',
  },
}));

export default function ShowMoreButton<T>(props: ShowMoreButtonProps<T>): React.ReactElement {
  const classes = useStyles();
  const { t } = useTranslation();

  if (props.isLoading) {
    return <LoadingIndicator />;
  }

  return (
    <div className={classes.showMoreWrapper}>
      <Button variant='secondary' onClick={() => props.fetchNextPage()}>
        {t('Ptv.Form.Header.EditHistory.Table.LoadMore')}
      </Button>
    </div>
  );
}
