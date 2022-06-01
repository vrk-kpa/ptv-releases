import React from 'react';
import { makeStyles } from '@mui/styles';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { PublishingStatus } from 'types/enumTypes';
import LoadingIndicator from 'components/LoadingIndicator';
import { useCanAddConnectionFromService } from 'hooks/security/useCanAddConnectionFromService';
import SearchResultItem from './SearchResultItem';

const useStyles = makeStyles({
  loadingIndicator: {
    marginTop: '20px',
    display: 'flex',
    justifyContent: 'center',
  },
});

type SearchResultsProps = {
  isLoading: boolean;
  items: ConnectableChannel[];
  selectedItems: ConnectableChannel[];
  suggestedChannels: string[];
  serviceResponsibleOrgId: string | null | undefined;
  serviceStatus: PublishingStatus;
  toggleSelectChannel: (channel: ConnectableChannel) => void;
};

export default function SearchResults(props: SearchResultsProps): React.ReactElement | null {
  const classes = useStyles();
  const canConnect = useCanAddConnectionFromService();

  if (props.isLoading) {
    return (
      <div className={classes.loadingIndicator}>
        <LoadingIndicator />
      </div>
    );
  }

  if (props.items.length === 0) {
    return null;
  }

  const expanders = props.items.map((x) => {
    const selected = props.selectedItems.findIndex((a) => a.id === x.id) !== -1;
    const suggested = props.suggestedChannels.includes(x.unificRootId);
    const canConnectFromService = canConnect(props.serviceResponsibleOrgId, props.serviceStatus, x);
    return (
      <SearchResultItem
        canConnect={canConnectFromService}
        suggested={suggested}
        selected={selected}
        key={x.id}
        toggleSelectChannel={props.toggleSelectChannel}
        channel={x}
      />
    );
  });

  return <div>{expanders}</div>;
}
