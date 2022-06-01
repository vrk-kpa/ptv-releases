import React, { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { Dropdown, DropdownItem } from 'suomifi-ui-components';
import { ServiceType, servicetype } from 'types/enumTypes';
import { KeyAndText } from 'types/miscellaneousTypes';
import { getKeyForServiceType } from 'utils/translations';

type ServiceTypeSelectorProps = {
  serviceType: ServiceType | undefined;
  onChange: (newValue: ServiceType | undefined) => void;
};

export default function ServiceTypeSelector(props: ServiceTypeSelectorProps): React.ReactElement {
  const { t } = useTranslation();
  function onChange(newValue: string) {
    props.onChange(newValue ? (newValue as ServiceType) : undefined);
  }

  const drowpDownItems = useMemo(() => {
    const items: KeyAndText<ServiceType>[] = servicetype
      .map((x) => ({ key: x, text: t(getKeyForServiceType(x)) }))
      .sort((left, right) => left.text.localeCompare(right.text));
    return items.map((item) => (
      <DropdownItem key={item.key} value={item.key}>
        {item.text}
      </DropdownItem>
    ));
  }, [t]);

  return (
    <Dropdown
      onChange={onChange}
      visualPlaceholder={t('Ptv.Service.Form.GdSearch.ServiceType.Placeholder')}
      labelText={t('Ptv.Service.Form.GdSearch.ServiceType.Label')}
      value={props.serviceType}
    >
      {drowpDownItems}
    </Dropdown>
  );
}
