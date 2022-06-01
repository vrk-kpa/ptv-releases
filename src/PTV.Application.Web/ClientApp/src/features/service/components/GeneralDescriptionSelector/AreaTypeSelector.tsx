import React from 'react';
import { useTranslation } from 'react-i18next';
import { Dropdown, DropdownItem } from 'suomifi-ui-components';
import { GeneralDescriptionType, generalDescriptionType } from 'types/enumTypes';
import { getKeyForGdType } from 'utils/translations';

type AreaTypeSelectorProps = {
  areaType: GeneralDescriptionType | undefined;
  onChange: (newValue: GeneralDescriptionType | undefined) => void;
};

export default function AreaTypeSelector(props: AreaTypeSelectorProps): React.ReactElement {
  const { t } = useTranslation();
  function onChange(newValue: string) {
    props.onChange(newValue ? (newValue as GeneralDescriptionType) : undefined);
  }

  const drowpDownItems = generalDescriptionType
    .map((x) => ({ key: x, text: t(getKeyForGdType(x)) }))
    .sort((left, right) => left.text.localeCompare(right.text));

  return (
    <Dropdown
      onChange={onChange}
      visualPlaceholder={t('Ptv.Service.Form.GdSearch.AreaType.Placeholder')}
      labelText={t('Ptv.Service.Form.GdSearch.AreaType.Label')}
      value={props.areaType}
    >
      {drowpDownItems.map((item) => (
        <DropdownItem key={item.key} value={item.key}>
          {item.text}
        </DropdownItem>
      ))}
    </Dropdown>
  );
}
