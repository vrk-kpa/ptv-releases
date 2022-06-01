import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Checkbox, Expander, ExpanderContent, ExpanderTitle } from 'suomifi-ui-components';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import useTranslateLocalizedText from 'hooks/useTranslateLocalizedText';
import { getConnectableChannelValue } from 'utils/serviceChannel';
import { getKeyForChannelType } from 'utils/translations';
import SearchResultContent from './SearchResultContent';
import { SearchResultTitle } from './SearchResultTitle';

const useStyles = makeStyles((theme) => ({
  expanderTitle: {
    '& span': {
      width: '100%',
    },
  },
}));

type SearchResultItemProps = {
  suggested: boolean;
  selected: boolean;
  channel: ConnectableChannel;
  canConnect: boolean;
  toggleSelectChannel: (channel: ConnectableChannel) => void;
};

export default function SearchResultItem(props: SearchResultItemProps): React.ReactElement {
  const translate = useTranslateLocalizedText();
  const classes = useStyles();
  const { t } = useTranslation();
  const [isOpen, setIsOpen] = useState<boolean>(false);

  const language = useGetUiLanguage();
  const channel = props.channel;

  function onOpenChange(currentOpenState: boolean) {
    setIsOpen(!currentOpenState);
  }

  function getContent(): React.ReactElement | null {
    if (isOpen) {
      return <SearchResultContent unificRootId={channel.unificRootId} showConnectedServices={true} />;
    }

    return null;
  }

  function getHintText(channel: ConnectableChannel) {
    const org = channel.organization;
    const name = org ? translate(org.texts) : '';
    const type = channel.channelType ? t(getKeyForChannelType(channel.channelType)) : '';
    return name ? `${type} - ${name}` : type;
  }

  const title = getConnectableChannelValue(channel.languageVersions, language, (lv) => lv.name, '');

  return (
    <Expander open={isOpen} onOpenChange={onOpenChange} key={channel.id}>
      <ExpanderTitle
        className={classes.expanderTitle}
        ariaOpenText={t('Ptv.Common.OpenExpander')}
        ariaCloseText={t('Ptv.Common.CloseExpander')}
        toggleButtonAriaDescribedBy={''}
      >
        <Checkbox
          disabled={!props.canConnect}
          checked={props.selected}
          onClick={() => props.toggleSelectChannel(channel)}
          hintText={getHintText(channel)}
        >
          <SearchResultTitle value={title} suggested={props.suggested} />
        </Checkbox>
      </ExpanderTitle>
      <ExpanderContent>{getContent()}</ExpanderContent>
    </Expander>
  );
}
