import React from 'react';
import { Control, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfTextInput } from 'fields';
import { Language, Mode } from 'types/enumTypes';
import { ServiceModel, cLv, cService } from 'types/forms/serviceFormTypes';
import { FormBlock } from 'components/formLayout/FormBlock';

type AlternativeNameProps = {
  name: string;
  language: Language;
  id: string;
  mode: Mode;
  control: Control<ServiceModel>;
};

export function AlternativeName(props: AlternativeNameProps): React.ReactElement | null {
  const { t } = useTranslation();

  const hasAlternativeName = useWatch({
    name: `${cService.languageVersions}.${props.language}.${cLv.hasAlternativeName}`,
    control: props.control,
  });

  if (!hasAlternativeName) return null;

  return (
    <FormBlock>
      <RhfTextInput
        name={props.name}
        id={props.id}
        control={props.control}
        mode={props.mode}
        visualPlaceholder={t('Ptv.Service.Form.Field.AlternativeName.Placeholder')}
        labelText={t('Ptv.Service.Form.Field.AlternativeName.Label')}
        optionalText={t('Ptv.Common.Optional')}
      />
    </FormBlock>
  );
}
