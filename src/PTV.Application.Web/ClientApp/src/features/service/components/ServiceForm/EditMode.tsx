import React from 'react';
import { Control, UseFormSetValue, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box, Grid } from '@mui/material';
import { Heading, Paragraph, Text } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceFormValues, ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { TabPanel } from 'components/TabPanel';
import { VisualHeading } from 'components/VisualHeading';
import { FormBlock } from 'components/formLayout/FormBlock';
import { FormDivider } from 'components/formLayout/FormDivider';
import { FormFieldArea } from 'components/formLayout/FormFieldArea';
import { useFormMetaContext } from 'context/formMeta';
import { useGdSpecificTranslationKey } from 'hooks/translations/useGdSpecificTranslationKey';
import { LanguagePriorities } from 'utils/languages';
import { ServiceQualityChecker } from 'features/qualityAgent';
import { ClassificationAndKeywordsForm } from 'features/service/components/ClassificationAndKeywords/ClassificationAndKeywordsForm';
import { CompareLanguageSelection } from 'features/service/components/CompareLanguageSelection';
import { GeneralDescriptionSelector } from 'features/service/components/GeneralDescriptionSelector';
import { LanguageVersionForm } from 'features/service/components/LanguageVersionForm';
import { ServiceFormEditFunctions } from 'features/service/components/ServiceFormEditFunctions';
import { ServiceFormErrorBox } from 'features/service/components/ServiceFormErrorBox';
import { ServiceFormTitle } from 'features/service/components/ServiceFormTitle';
import { ServiceOrganizationForm } from 'features/service/components/ServiceOrganizationForm';
import ServiceProviders from 'features/service/components/ServiceProviders';
import ServiceVouchersEdit from 'features/service/components/ServiceVouchers/ServiceVouchersEdit';
import { DetailedTabErrors } from 'features/service/components/ValidationErrors/DetailedTabErrors';
import { LanguageVersionTabsErrors } from 'features/service/components/ValidationErrors/LanguageVersionTabsErrors';
import { MissingOrganizationErrors } from 'features/service/components/ValidationErrors/MissingOrganizationErrors';
import { LanguageTabs } from './LanguageTabs';
import { useTriggerFormValidation } from './useTriggerFormValidation';

type ServiceFormEditModeProps = {
  service: ServiceFormValues;
  enabledLanguages: Language[];
  control: Control<ServiceModel>;
  responsibleOrganization: OrganizationModel | null | undefined;
  generalDescription: GeneralDescriptionModel | null | undefined;
  saveInProgress: boolean;
  trigger: UseFormTrigger<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
  saveDraft: () => void;
  resetForm: () => void;
  onTabChange: (language: Language) => void;
  onCompareModeChanged: () => void;
  getFormValues: () => ServiceFormValues;
};

const LongFieldMaxWidth = '950px';

export function ServiceFormEditMode(props: ServiceFormEditModeProps): React.ReactElement {
  const { t } = useTranslation();
  const { displayComparison, selectedLanguageCode } = useFormMetaContext();
  const meta = useFormMetaContext();

  useTriggerFormValidation({ trigger: props.trigger });

  const classificationAndKeywordsDescKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.ClassificationAndKeywords.Title.Description',
    'Ptv.Service.Form.ClassificationAndKeywords.Title.GdSelected.Description'
  );

  const serviceOrganizationNamespace = `${cService.languageVersions}.${selectedLanguageCode}`;

  return (
    <>
      <Grid item>
        <FormFieldArea>
          <ServiceFormTitle control={props.control} />
          <ServiceFormEditFunctions
            control={props.control}
            enabledLanguages={props.enabledLanguages}
            selectedLanguage={selectedLanguageCode}
            hasOtherModifiedVersion={!!props.service.otherModifiedVersion}
            serviceId={props.service.id}
            responsibleOrgId={props.responsibleOrganization?.id}
            saveInProgress={props.saveInProgress}
            resetForm={props.resetForm}
            saveDraft={props.saveDraft}
            alignItemsLeft={false}
          />
          <ServiceFormErrorBox />
          <Box>
            <LanguageVersionTabsErrors language={selectedLanguageCode} />
            <MissingOrganizationErrors language={selectedLanguageCode} control={props.control} />
          </Box>
          <FormDivider />
          <FormBlock mt={3} mb={2}>
            <Heading variant='h2'>{t('Ptv.Service.Form.Title.Text')}</Heading>
          </FormBlock>
          <LanguageTabs
            onTabChange={props.onTabChange}
            selectedLanguage={selectedLanguageCode}
            toggleCompare={props.onCompareModeChanged}
            isComparing={displayComparison}
            getFormValues={props.getFormValues}
          >
            {LanguagePriorities.map((lang) => {
              return (
                <TabPanel<Language> key={lang} tab={lang} selectedTab={selectedLanguageCode}>
                  <ServiceQualityChecker
                    formType='Services'
                    language={lang}
                    enabled={meta.selectedLanguageCode === lang && !meta.displayComparison}
                    control={props.control}
                  />
                  {meta.displayComparison && <CompareLanguageSelection enabledLanguages={props.enabledLanguages} />}
                  <React.Fragment>
                    <DetailedTabErrors language={selectedLanguageCode} formName='Service' />
                    <FormBlock mt={3}>
                      <Heading variant='h3'>{t('Ptv.Service.Form.Gd.Title.Text')}</Heading>
                    </FormBlock>
                    <FormBlock>
                      <Paragraph>{t('Ptv.Service.Form.Gd.Description.Text')}</Paragraph>
                    </FormBlock>
                    <FormBlock style={{ maxWidth: LongFieldMaxWidth }}>
                      <GeneralDescriptionSelector
                        control={props.control}
                        setValue={props.setValue}
                        selectedGeneralDescription={props.generalDescription}
                        tabLanguage={lang}
                        getFormValues={props.getFormValues}
                        trigger={props.trigger}
                      />
                    </FormBlock>
                  </React.Fragment>
                  <FormDivider my={3} />
                  <FormBlock>
                    <Heading variant='h3'>{t('Ptv.Service.Form.BasicInfo.Title.Text')}</Heading>
                  </FormBlock>
                  <FormBlock>
                    <Text>{t('Ptv.Service.Form.Title.Description')}</Text>
                  </FormBlock>
                  <FormDivider my={3} />
                  <LanguageVersionForm
                    language={lang}
                    control={props.control}
                    generalDescription={props.generalDescription}
                    enabledLanguages={props.enabledLanguages}
                    setValue={props.setValue}
                    getFormValues={props.getFormValues}
                    trigger={props.trigger}
                  />
                </TabPanel>
              );
            })}
          </LanguageTabs>
        </FormFieldArea>
      </Grid>
      <Grid item>
        <FormFieldArea>
          <FormBlock mt={3}>
            <Heading variant='h3'>{t('Ptv.Service.Form.ClassificationAndKeywords.Title.Text')}</Heading>
          </FormBlock>
          <FormBlock>
            <Paragraph>{t(classificationAndKeywordsDescKey)}</Paragraph>
          </FormBlock>
          <FormDivider my={3} />
          <ClassificationAndKeywordsForm
            control={props.control}
            language={selectedLanguageCode}
            enabledLanguages={props.enabledLanguages}
          />
        </FormFieldArea>
      </Grid>
      <Grid item>
        <FormFieldArea>
          {props.enabledLanguages.map((lang) => {
            return (
              <TabPanel<Language> key={lang} tab={lang} selectedTab={selectedLanguageCode}>
                <FormBlock my={3}>
                  <Heading variant='h3' id={`languageVersions[${selectedLanguageCode}].organization`}>
                    {t('Ptv.Service.Form.ServiceOrganization.Title.Text')}
                  </Heading>
                </FormBlock>
                <FormBlock>
                  <Paragraph>{t('Ptv.Service.Form.ServiceOrganization.Title.Description')}</Paragraph>
                </FormBlock>
                <FormDivider my={3} />
                <ServiceOrganizationForm
                  control={props.control}
                  namespace={serviceOrganizationNamespace}
                  language={selectedLanguageCode}
                  enabledLanguages={props.enabledLanguages}
                  getFormValues={props.getFormValues}
                  setValue={props.setValue}
                  trigger={props.trigger}
                />
                <Box>
                  <FormBlock>
                    <Paragraph>{t('Ptv.Service.Form.Field.ServiceProducers.Title.Description')}</Paragraph>
                  </FormBlock>
                  <FormBlock my={3}>
                    <VisualHeading variant='h5'>{t('Ptv.Service.Form.Field.ServiceProducers.SelectSection.Title')}</VisualHeading>
                  </FormBlock>
                  <FormBlock>
                    <Paragraph>{t('Ptv.Service.Form.Field.ServiceProducers.SelectSection.Descrption')}</Paragraph>
                  </FormBlock>
                  <FormBlock>
                    <ServiceProviders
                      namespace={serviceOrganizationNamespace}
                      control={props.control}
                      setValue={props.setValue}
                      tabLanguage={selectedLanguageCode}
                    />
                  </FormBlock>
                </Box>
                <FormDivider my={3} />
                <FormBlock>
                  <ServiceVouchersEdit
                    control={props.control}
                    trigger={props.trigger}
                    namespace={serviceOrganizationNamespace}
                    tabLanguage={selectedLanguageCode}
                    enabledLanguages={props.enabledLanguages}
                    setValue={props.setValue}
                  />
                </FormBlock>
              </TabPanel>
            );
          })}
          <FormDivider mt={3} />
          <Box marginTop='25px'>
            <ServiceFormEditFunctions
              control={props.control}
              selectedLanguage={selectedLanguageCode}
              enabledLanguages={props.enabledLanguages}
              hasOtherModifiedVersion={!!props.service.otherModifiedVersion}
              serviceId={props.service.id}
              responsibleOrgId={props.responsibleOrganization?.id}
              saveInProgress={props.saveInProgress}
              resetForm={props.resetForm}
              saveDraft={props.saveDraft}
              alignItemsLeft={true}
            />
          </Box>
        </FormFieldArea>
      </Grid>
    </>
  );
}
