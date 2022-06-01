import React from 'react';
import { useTranslation } from 'react-i18next';
import { Divider } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Button } from 'suomifi-ui-components';
import { ConnectionApiModel } from 'types/api/connectionApiModel';
import { ServiceFormValues } from 'types/forms/serviceFormTypes';
import { ServerErrorBox } from 'components/ErrorBox/ServerErrorBox';
import LoadingIndicator from 'components/LoadingIndicator';
import { useFormMetaContext } from 'context/formMeta';
import { useGetConnectionForService } from 'hooks/queries/useGetConnectionForService';
import SearchResultContent from 'features/service/components/ServiceChannelSearch/SearchResultContent';

const useStyles = makeStyles(() => ({
  edit: {
    display: 'inline',
    marginRight: '20px',
  },
  loadingIndicator: {
    marginTop: '20px',
    display: 'flex',
    justifyContent: 'center',
  },
  divider: {
    marginTop: '20px !important',
    marginBottom: '20px !important',
  },
}));

type ChannelItemContentProps = {
  serviceChannelUnificRootId: string;
  canRemove: boolean;
  canEdit: boolean;
  onRemoveConnection: () => void;
  onEditConnection: (connection: ConnectionApiModel) => void;
  getFormValues: () => ServiceFormValues;
};

export function ChannelItemContent(props: ChannelItemContentProps): React.ReactElement {
  const { t } = useTranslation();
  const { mode } = useFormMetaContext();
  const classes = useStyles();
  const service = props.getFormValues();

  const query = useGetConnectionForService(service.id || '', props.serviceChannelUnificRootId);
  const canEditDetails = props.canEdit && query.data && !query.isLoading;
  const editDisabled = mode !== 'view' || !canEditDetails;
  const removeDisabled = mode !== 'view' || !props.canRemove;

  if (query.isLoading || !query.data) {
    return (
      <div className={classes.loadingIndicator}>
        <LoadingIndicator />
      </div>
    );
  }

  if (query.error) {
    return <ServerErrorBox httpError={query.error} />;
  }

  return (
    <div>
      <SearchResultContent unificRootId={props.serviceChannelUnificRootId} showConnectedServices={false} channelConnection={query.data} />
      <Divider className={classes.divider} />
      <div>
        <div className={classes.edit}>
          <Button disabled={editDisabled} onClick={() => props.onEditConnection(query.data)} variant='secondary'>
            {t('Ptv.ConnectionDetails.OpenEditor.Button.Text')}
          </Button>
        </div>
        <Button disabled={removeDisabled} onClick={props.onRemoveConnection} icon='remove' variant='secondaryNoBorder'>
          {t('Ptv.Service.Form.ConnectedServiceChannels.Item.RemoveConnection.Button.Label')}
        </Button>
      </div>
    </div>
  );
}
