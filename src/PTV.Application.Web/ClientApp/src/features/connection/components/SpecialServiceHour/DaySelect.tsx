import React, { useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { Dropdown, DropdownItem } from 'suomifi-ui-components';
import * as enumTypes from 'types/enumTypes';
import { getKeyForWeekday } from 'utils/translations';
import { toFieldId } from 'features/connection/utils/fieldid';
import './styles.css';

type DaySelectProps = {
  onChange: (newValue: string) => void;
  name: string;
  value: enumTypes.Weekday;
  label: string;
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function DaySelect(props: DaySelectProps): React.ReactElement {
  const { t } = useTranslation();

  const renderItems = useCallback(() => {
    const elements: React.ReactElement[] = [];
    for (const day of enumTypes.weekDayType) {
      elements.push(
        <DropdownItem key={day} value={day}>
          {t(getKeyForWeekday(day))}
        </DropdownItem>
      );
    }

    return elements;
  }, [t]);

  function onDayChange(newValue: string) {
    props.onChange(newValue);
  }

  const items = renderItems();

  return (
    <Dropdown
      visualPlaceholder={t('Ptv.Common.Choose')}
      className='dropdown-custom'
      onChange={onDayChange}
      value={props.value}
      id={toFieldId(props.name)}
      labelText={props.label}
    >
      {items}
    </Dropdown>
  );
}
