import React, { useState } from 'react';
import { makeStyles } from '@mui/styles';
import { ServiceChannel } from 'types/api/serviceChannelModel';
import Paginator from 'components/Paginator';
import { getItemsForPage, getPageCount } from 'utils/pagination';
import { ConnectedServicesTable } from './ConnectedServicesTable';

const useStyles = makeStyles((theme) => ({
  paginator: {
    display: 'flex',
    justifyContent: 'center',
    marginTop: '20px',
  },
}));

type ConnectedServicesProps = {
  channel: ServiceChannel;
};

const RowsPerPage = 10;

export function ConnectedServices(props: ConnectedServicesProps): React.ReactElement | null {
  const classes = useStyles();
  const [pageNumber, setPageNumber] = useState<number>(1);
  const connections = props.channel.connections;

  if (connections.length === 0) {
    return null;
  }

  const pageCount = getPageCount(connections.length, RowsPerPage);
  const rows = getItemsForPage(connections, pageNumber, RowsPerPage);
  const hasMultiplePages = connections.length > RowsPerPage;

  return (
    <div>
      <ConnectedServicesTable connections={rows} />
      {hasMultiplePages && (
        <div className={classes.paginator}>
          <Paginator currentPage={pageNumber} pageCount={pageCount} onChange={(page) => setPageNumber(page)} />
        </div>
      )}
    </div>
  );
}
