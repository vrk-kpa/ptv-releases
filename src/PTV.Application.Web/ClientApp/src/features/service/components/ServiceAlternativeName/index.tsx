import React, { Fragment, FunctionComponent } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { RhfCheckbox } from 'fields';
import { Language, Mode } from 'types/enumTypes';
import { ServiceModel, cLv } from 'types/forms/serviceFormTypes';
import { ComparisonView } from 'components/ComparisonView';
import { FormBlock } from 'components/formLayout/FormBlock';
import { useFormMetaContext, useGetCompareFieldId, useGetCompareFieldName, useGetFieldId, useGetFieldName } from 'context/formMeta';
import { AlternativeName } from './AlternativeName';

type ServiceAlternativeNameProps = {
  control: Control<ServiceModel>;
};

export const ServiceAlternativeName: FunctionComponent<ServiceAlternativeNameProps> = (props: ServiceAlternativeNameProps) => {
  const meta = useFormMetaContext();
  const name = useGetFieldName();
  const compareName = useGetCompareFieldName();
  const id = useGetFieldId();
  const compareId = useGetCompareFieldId();

  function renderRight() {
    if (!meta.compareLanguageCode) return null;

    return (
      <NameAndToggle
        name={compareName(cLv.alternativeName, meta.compareLanguageCode)}
        id={compareId(cLv.alternativeName, meta.compareLanguageCode)}
        toggleFieldName={compareName(cLv.hasAlternativeName, meta.compareLanguageCode)}
        toggleFieldId={compareId(cLv.hasAlternativeName, meta.compareLanguageCode)}
        mode={meta.mode}
        control={props.control}
        language={meta.compareLanguageCode}
      />
    );
  }

  return (
    <ComparisonView
      left={
        <NameAndToggle
          name={name(cLv.alternativeName)}
          id={id(cLv.alternativeName)}
          toggleFieldName={name(cLv.hasAlternativeName)}
          toggleFieldId={id(cLv.hasAlternativeName)}
          mode={meta.mode}
          control={props.control}
          language={meta.selectedLanguageCode}
        />
      }
      right={renderRight()}
    />
  );
};

type NameAndToggleProps = {
  name: string;
  id: string;
  mode: Mode;
  toggleFieldName: string;
  toggleFieldId: string;
  control: Control<ServiceModel>;
  language: Language;
};

const NameAndToggle: FunctionComponent<NameAndToggleProps> = (props: NameAndToggleProps) => {
  const { t } = useTranslation();

  return (
    <Fragment>
      <FormBlock>
        <RhfCheckbox name={props.toggleFieldName} id={props.toggleFieldId} hideInViewMode mode={props.mode} control={props.control}>
          {t('Ptv.Service.Form.Field.AddAlternativeName.Label')}
        </RhfCheckbox>
      </FormBlock>
      <AlternativeName name={props.name} id={props.id} mode={props.mode} language={props.language} control={props.control} />
    </Fragment>
  );
};
