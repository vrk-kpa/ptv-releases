import React from 'react';
import { Control, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RadioButton, RadioButtonGroup } from 'suomifi-ui-components';
import { FakeLoginModel, UserAccessRightsGroup } from 'types/loginTypes';
import { useTranslateText } from 'hooks/useTranslateText';

type AcccessGroupsProps = {
  control: Control<FakeLoginModel>;
  groups: UserAccessRightsGroup[];
};

export default function AccessGroups(props: AcccessGroupsProps): React.ReactElement | null {
  const { field } = useController({ control: props.control, name: 'userAccessRightsGroup' });

  const translate = useTranslateText();
  const { t } = useTranslation();

  if (props.groups.length === 0) {
    return null;
  }

  const buttons: React.ReactElement[] = props.groups.map((group) => {
    return (
      <RadioButton key={group.id} value={group.id}>
        {translate(group.translation)}
      </RadioButton>
    );
  });

  const value: string = field.value ? field.value : '';

  return (
    <RadioButtonGroup
      onChange={(value) => field.onChange(value)}
      value={value}
      name='access-group'
      labelText={t('Ptv.Login.Form.Field.AccessGroup.Label')}
    >
      {buttons}
    </RadioButtonGroup>
  );
}
