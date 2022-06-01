import React, { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Dropdown, DropdownItem } from 'suomifi-ui-components';
import { ChannelType, channelType } from 'types/enumTypes';
import { KeyAndText } from 'types/miscellaneousTypes';
import { getKeyForChannelType } from 'utils/translations';

type ServiceTypeSelectorProps = {
  channelType: ChannelType | undefined;
  onChange: (newValue: ChannelType | undefined) => void;
};

const useStyles = makeStyles((theme) => ({
  dropdown: {
    width: '100%',
    '& .fi-dropdown_button': {
      width: '100%',
    },
  },
}));

export default function ServiceChannelTypeSelector(props: ServiceTypeSelectorProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  function onChange(newValue: string) {
    props.onChange(newValue ? (newValue as ChannelType) : undefined);
  }

  const drowpDownItems = useMemo(() => {
    const items: KeyAndText<ChannelType>[] = channelType
      .map((x) => ({ key: x, text: t(getKeyForChannelType(x)) }))
      .sort((left, right) => left.text.localeCompare(right.text));
    return items.map((item) => (
      <DropdownItem key={item.key} value={item.key}>
        {item.text}
      </DropdownItem>
    ));
  }, [t]);

  return (
    <Dropdown
      className={classes.dropdown}
      onChange={onChange}
      visualPlaceholder={t('Ptv.Service.Form.ServiceChannelSearch.ChannelType.Placeholder')}
      labelText={t('Ptv.Service.Form.ServiceChannelSearch.ChannelType.Label')}
      value={props.channelType}
    >
      {drowpDownItems}
    </Dropdown>
  );
}
