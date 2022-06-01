import React from 'react';
import { Control, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Dropdown, DropdownItem } from 'suomifi-ui-components';
import { ConnectionFormModel, cC, cHour, openingHourType } from 'types/forms/connectionFormTypes';
import ValidationMessage from 'components/ValidationMessage';
import { toFieldStatus } from 'utils/rhf';
import { getKeyForStandardServiceHourOpeningType } from 'utils/translations';
import { toFieldId } from 'features/connection/utils/fieldid';

type OpeningTypeProps = {
  hourIndex: number;
  control: Control<ConnectionFormModel>;
};

export function OpeningType(props: OpeningTypeProps): React.ReactElement | null {
  const { t } = useTranslation();

  const { field, fieldState } = useController({
    name: `${cC.standardOpeningHours}.${props.hourIndex}.${cHour.openingType}` as const,
    control: props.control,
  });

  const { status, statusText } = toFieldStatus(fieldState);

  const drowpDownItems = openingHourType
    .map((x) => ({ key: x, text: t(getKeyForStandardServiceHourOpeningType(x)) }))
    .sort((left, right) => left.text.localeCompare(right.text));

  function onChange(value: string) {
    field.onChange(value);
  }

  return (
    <>
      <Dropdown
        id={toFieldId(field.name)}
        labelText={t('Ptv.ConnectionDetails.StandardServiceHour.OpeningType.Label')}
        optionalText={t('Ptv.Common.Optional')}
        value={field.value || undefined}
        onChange={onChange}
        visualPlaceholder={t('Ptv.ConnectionDetails.StandardServiceHour.OpeningType.Placeholder.Label')}
      >
        {drowpDownItems.map((item) => (
          <DropdownItem key={item.key} value={item.key}>
            {item.text}
          </DropdownItem>
        ))}
      </Dropdown>
      {status === 'error' && (
        <div>
          <ValidationMessage message={statusText} />
        </div>
      )}
    </>
  );
}
