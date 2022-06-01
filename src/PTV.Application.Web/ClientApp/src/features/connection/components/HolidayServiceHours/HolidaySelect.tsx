import React, { useMemo } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { MultiSelect, MultiSelectData } from 'suomifi-ui-components';
import * as enumTypes from 'types/enumTypes';
import { ConnectionFormModel, cC } from 'types/forms/connectionFormTypes';
import { getKeyForHoliday } from 'utils/translations';
import { toFieldId } from 'features/connection/utils/fieldid';

type HolidaySelectProps = {
  control: Control<ConnectionFormModel>;
  selectedHolidays: enumTypes.HolidayEnum[];
  add: (holiday: enumTypes.HolidayEnum) => void;
  remove: (holiday: enumTypes.HolidayEnum) => void;
};

type HolidayData = MultiSelectData & {
  holiday: enumTypes.HolidayEnum;
};

export function HolidaySelect(props: HolidaySelectProps): React.ReactElement {
  const { t } = useTranslation();

  function toHolidayData(holiday: enumTypes.HolidayEnum): HolidayData {
    return {
      uniqueItemId: holiday,
      holiday: holiday,
      labelText: t(getKeyForHoliday(holiday)),
    };
  }

  const selected = props.selectedHolidays.map((x) => toHolidayData(x));
  const allHolidays = useMemo(() => {
    return enumTypes.holidayType.map((x: enumTypes.HolidayEnum): HolidayData => {
      return {
        holiday: x,
        uniqueItemId: x,
        labelText: t(getKeyForHoliday(x)),
      };
    });
  }, [t]);

  function onItemSelect(uniqueItemId: string) {
    const item = allHolidays.find((x) => x.uniqueItemId === uniqueItemId);
    if (!item) return;

    const index = props.selectedHolidays.findIndex((x) => x === item.holiday);
    if (index === -1) {
      props.add(item.holiday);
    } else {
      props.remove(item.holiday);
    }
  }

  return (
    <MultiSelect
      id={toFieldId(cC.holidayOpeningHours + '.select-holiday')}
      labelText={t('Ptv.ConnectionDetails.HolidayServiceHours.HolidaySelect.Label')}
      visualPlaceholder={t('Ptv.ConnectionDetails.HolidayServiceHours.HolidaySelect.Placeholder')}
      noItemsText=''
      onItemSelect={onItemSelect}
      selectedItems={selected}
      items={allHolidays}
      ariaSelectedAmountText=''
      ariaOptionsAvailableText=''
      ariaOptionChipRemovedText=''
    />
  );
}
