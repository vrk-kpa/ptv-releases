import React from 'react';
import { Control, UseFormSetValue, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { Heading } from 'suomifi-ui-components';
import { Language, ServiceType } from 'types/enumTypes';
import { ServiceFormValues, ServiceModel, cLv, cService } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { OptionalFieldByWatch } from 'components/OptionalFieldByWatch';
import { FormBlock } from 'components/formLayout/FormBlock';
import { FormDivider } from 'components/formLayout/FormDivider';
import { useFormMetaContext } from 'context/formMeta';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { getDescriptionSelector } from 'features/qualityAgent';
import { ConditionsAndCriteria } from 'features/service/components/ConditionsAndCriteria';
import { Deadline } from 'features/service/components/Deadline';
import { LanguageSelector } from 'features/service/components/LanguageSelector';
import { PeriodOfValidity } from 'features/service/components/PeriodOfValidity';
import { ProcessingTime } from 'features/service/components/ProcessingTime';
import { ServiceAlternativeName } from 'features/service/components/ServiceAlternativeName';
import { ServiceAreaInformation } from 'features/service/components/ServiceAreaInformation';
import { ServiceBackgroundDescription } from 'features/service/components/ServiceBackgroundDescription';
import ServiceCharge from 'features/service/components/ServiceCharge';
import { ServiceDescription } from 'features/service/components/ServiceDescription';
import { ServiceLawsCompare } from 'features/service/components/ServiceLaws';
import { ServiceName } from 'features/service/components/ServiceName';
import { ServiceSummary } from 'features/service/components/ServiceSummary';
import { ServiceTypeSelector } from 'features/service/components/ServiceTypeSelector';
import { UserInstructions } from 'features/service/components/UserInstructions';
import { isGeneralDescriptionSelected, isPermissionOrQualification } from 'features/service/utils';

type LanguageVersionFormProps = {
  language: Language;
  enabledLanguages: Language[];
  control: Control<ServiceModel>;
  generalDescription: GeneralDescriptionModel | null | undefined;
  setValue: UseFormSetValue<ServiceModel>;
  trigger: UseFormTrigger<ServiceModel>;
  getFormValues: () => ServiceFormValues;
};

export function LanguageVersionForm(props: LanguageVersionFormProps): React.ReactElement {
  const { t } = useTranslation();
  const appContext = useAppContextOrThrow();
  const meta = useFormMetaContext();
  const mode = meta.mode;
  const { language } = props;

  return (
    <Box>
      <FormBlock>
        <ServiceName control={props.control} setValue={props.setValue} />
      </FormBlock>
      <FormBlock>
        <ServiceAlternativeName control={props.control} />
      </FormBlock>
      <FormBlock>
        <ServiceTypeSelector
          control={props.control}
          name={cService.serviceType}
          tabLanguage={language}
          mode={mode}
          disabled={props.generalDescription?.id !== undefined}
        />
      </FormBlock>
      <FormDivider my={3} />
      <FormBlock>
        <Heading variant='h4'>{t('Ptv.Service.Form.ServiceDescription.Title.Text')}</Heading>
      </FormBlock>
      <FormBlock>
        <ServiceSummary control={props.control} language={language} setValue={props.setValue} />
      </FormBlock>
      <FormBlock>
        <ServiceDescription
          gd={props.generalDescription}
          qualitySelector={getDescriptionSelector('Services', 'Description', language)}
          control={props.control}
          setValue={props.setValue}
        />
      </FormBlock>
      <FormBlock>
        <UserInstructions
          gd={props.generalDescription}
          qualitySelector={getDescriptionSelector('Services', 'UserInstruction', language)}
          control={props.control}
          setValue={props.setValue}
        />
      </FormBlock>
      <FormBlock>
        <ConditionsAndCriteria
          gd={props.generalDescription}
          qualitySelector={`requirements.${language}`}
          control={props.control}
          setValue={props.setValue}
        />
      </FormBlock>
      <FormDivider my={3} />
      <OptionalFieldByWatch<GeneralDescriptionModel>
        fieldName={cService.generalDescription}
        shouldRender={isGeneralDescriptionSelected}
        control={props.control}
      >
        <>
          <FormBlock my={3}>
            <ServiceBackgroundDescription gd={props.generalDescription} />
          </FormBlock>
          <FormDivider my={3} />
        </>
      </OptionalFieldByWatch>
      <OptionalFieldByWatch<ServiceType>
        fieldName={cService.serviceType}
        shouldRender={isPermissionOrQualification}
        control={props.control}
      >
        <>
          <FormBlock>
            <Deadline
              gd={props.generalDescription}
              qualitySelector={getDescriptionSelector('Services', 'DeadLine', language)}
              control={props.control}
              setValue={props.setValue}
            />
          </FormBlock>
          <FormDivider my={3} />
        </>
      </OptionalFieldByWatch>
      <OptionalFieldByWatch<ServiceType>
        fieldName={cService.serviceType}
        shouldRender={isPermissionOrQualification}
        control={props.control}
      >
        <>
          <FormBlock>
            <ProcessingTime
              gd={props.generalDescription}
              qualitySelector={getDescriptionSelector('Services', 'ProcessingTime', language)}
              control={props.control}
              setValue={props.setValue}
            />
          </FormBlock>
          <FormDivider my={3} />
        </>
      </OptionalFieldByWatch>
      <OptionalFieldByWatch<ServiceType>
        fieldName={cService.serviceType}
        shouldRender={isPermissionOrQualification}
        control={props.control}
      >
        <>
          <FormBlock>
            <PeriodOfValidity
              gd={props.generalDescription}
              qualitySelector={getDescriptionSelector('Services', 'ValidityTime', language)}
              control={props.control}
              setValue={props.setValue}
            />
          </FormBlock>
          <FormDivider my={3} />
        </>
      </OptionalFieldByWatch>
      <FormBlock>
        <ServiceLawsCompare gd={props.generalDescription} control={props.control} trigger={props.trigger} />
      </FormBlock>
      <FormDivider my={3} />
      <FormBlock>
        <Heading variant='h4'>{t('Ptv.Service.Form.LanguageAndAreaInformation.Title.Text')}</Heading>
      </FormBlock>
      <FormBlock>
        <LanguageSelector
          control={props.control}
          tabLanguage={language}
          name={cService.languages}
          allLanguages={appContext.staticData.languages}
          mode={mode}
        />
      </FormBlock>
      <FormBlock>
        <ServiceAreaInformation
          tabLanguage={language}
          name={cService.areaInformation}
          allMunicipalities={appContext.staticData.municipalities}
          allProvinces={appContext.staticData.provinces}
          allBusinessRegions={appContext.staticData.businessRegions}
          allHospitalRegions={appContext.staticData.hospitalRegions}
          mode={mode}
          control={props.control}
        />
      </FormBlock>
      <FormDivider my={3} />
      <FormBlock>
        <ServiceCharge tabLanguage={language} name={cLv.charge} control={props.control} setValue={props.setValue} />
      </FormBlock>
    </Box>
  );
}
