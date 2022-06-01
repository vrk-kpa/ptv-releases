import React, { useState } from 'react';
import { Expander, ExpanderContent, ExpanderTitleButton } from 'suomifi-ui-components';
import { ConnectionApiModel } from 'types/api/connectionApiModel';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { ServiceFormValues } from 'types/forms/serviceFormTypes';
import { EditConnectionDetailsModal } from 'features/connection/components/EditConnectionDetailsModal';
import { ChannelItemContent } from './ChannelItemContent';
import { ChannelItemTitle } from './ChannelItemTitle';
import { RemoveConnectionModal } from './RemoveConnectionModal';

type ChannelItemProps = {
  channel: ConnectableChannel;
  canRemove: boolean;
  canEdit: boolean;
  getFormValues: () => ServiceFormValues;
  removeServiceChannel: (serviceChannelUnificRootId: string) => void;
};

export default function ChannelItem(props: ChannelItemProps): React.ReactElement {
  const [isRemoveModalOpen, setIsRemoveModalOpen] = useState<boolean>(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState<boolean>(false);
  const [isExpanderOpen, setIsExpanderOpen] = useState<boolean>(false);
  const [connection, setConnection] = useState<ConnectionApiModel | null>(null);

  function onEditConnection(connection: ConnectionApiModel) {
    setConnection(connection);
    setIsEditModalOpen(true);
  }

  function onCloseDiscardedChanges() {
    setIsEditModalOpen(false);
  }

  function onCloseSavedChanges() {
    setIsEditModalOpen(false);
    setConnection(connection);
  }

  return (
    <div>
      <Expander open={isExpanderOpen} onOpenChange={(openState) => setIsExpanderOpen(!openState)}>
        <ExpanderTitleButton>
          <ChannelItemTitle channel={props.channel} />
        </ExpanderTitleButton>
        <ExpanderContent>
          {isExpanderOpen && (
            <ChannelItemContent
              serviceChannelUnificRootId={props.channel.unificRootId}
              canRemove={props.canRemove}
              onRemoveConnection={() => setIsRemoveModalOpen(true)}
              canEdit={props.canEdit}
              onEditConnection={onEditConnection}
              getFormValues={props.getFormValues}
            />
          )}
        </ExpanderContent>
      </Expander>
      <RemoveConnectionModal
        channel={props.channel}
        visible={isRemoveModalOpen}
        onClose={() => setIsRemoveModalOpen(false)}
        getFormValues={props.getFormValues}
        removeServiceChannel={props.removeServiceChannel}
      />
      {connection && (
        <EditConnectionDetailsModal
          channel={props.channel}
          connection={connection}
          visible={isEditModalOpen}
          onCloseDiscardedChanges={onCloseDiscardedChanges}
          onCloseSavedChanges={onCloseSavedChanges}
          getFormValues={props.getFormValues}
        />
      )}
    </div>
  );
}
