import React, { useCallback } from 'react';
import { Control, UseFormSetValue, UseFormTrigger, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { useMatomo } from '@jonkoops/matomo-tracker-react';
import { Box, Grid } from '@mui/material';
import { styled } from '@mui/material/styles';
import { Expander, ExpanderContent, ExpanderGroup, ExpanderTitleButton, Heading, Paragraph, Text } from 'suomifi-ui-components';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { Language } from 'types/enumTypes';
import { ServiceFormValues, ServiceModel, cLv, cService } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { LanguageVersionWithName } from 'types/languageVersionTypes';
import { MatomoActionTypes, MatomoCategories } from 'types/matomo';
import { OrganizationModel } from 'types/organizationTypes';
import EditButton from 'components/Buttons/EditButton';
import { TabPanel } from 'components/TabPanel';
import { FormBlock } from 'components/formLayout/FormBlock';
import { FormDivider } from 'components/formLayout/FormDivider';
import { FormFieldArea } from 'components/formLayout/FormFieldArea';
import { switchFormModeAndSelectLanguage, useFormMetaContext } from 'context/formMeta';
import { Dispatch } from 'context/formMeta/DispatchFormMetaContext';
import { useCanUpdateService } from 'hooks/security/useCanUpdateService';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { toServiceUiModel } from 'mappers/serviceMapper';
import { LanguagePriorities } from 'utils/languages';
import { getEnabledLanguagesByPriority } from 'utils/service';
import { ClassificationAndKeywordsReadView } from 'features/service/components/ClassificationAndKeywords/ClassificationAndKeywordsReadView';
import { SelectedGdView } from 'features/service/components/GeneralDescriptionSelector/SelectedGdView';
import { LanguageVersionForm } from 'features/service/components/LanguageVersionForm';
import { SelectedGDChipsView } from 'features/service/components/SelectedGDChipsView/SelectedGDChipsView';
import { ServiceChannelsForm } from 'features/service/components/ServiceChannelsForm';
import { ServiceFormHeader } from 'features/service/components/ServiceFormHeader';
import { ServiceFormPublishFunctions } from 'features/service/components/ServiceFormPublishFunctions';
import { ServiceOrganizationForm } from 'features/service/components/ServiceOrganizationForm';
import ServiceProvidersView from 'features/service/components/ServiceProviders/ServiceProvidersView';
import ServiceVouchersView from 'features/service/components/ServiceVouchers/ServiceVouchersView';
import { anyLanguageInTranslation } from 'features/service/utils';
import { LanguageTabs } from './LanguageTabs';

type ServiceFormViewModeProps = {
  className?: string;
  service: ServiceFormValues;
  enabledLanguages: Language[];
  formMetaDispatch: Dispatch;
  control: Control<ServiceModel>;
  responsibleOrganization: OrganizationModel | null | undefined;
  generalDescription: GeneralDescriptionModel | null | undefined;
  trigger: UseFormTrigger<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
  resetForm: () => void;
  onTabChange: (language: Language) => void;
  onCompareModeChanged: () => void;
  getFormValues: () => ServiceFormValues;
  updateServiceAndResetForm: (formModel: ServiceFormValues) => void;
  invalidateAndRefetchServiceFormHeaderData: () => void;
};

export const ServiceFormViewMode = styled((props: ServiceFormViewModeProps): React.ReactElement => {
  const { t } = useTranslation();
  const { displayComparison, selectedLanguageCode } = useFormMetaContext();
  const navigate = useNavigate();
  const uiLang = useGetUiLanguage();
  const { formMetaDispatch, setValue, updateServiceAndResetForm, invalidateAndRefetchServiceFormHeaderData } = props;
  const { trackEvent } = useMatomo();

  const serviceOrganizationNamespace = `${cService.languageVersions}.${selectedLanguageCode}`;

  const languageVersions = useWatch({
    name: `${cService.languageVersions}`,
    control: props.control,
  });

  const hasTranslationOrder = anyLanguageInTranslation(languageVersions);

  const addLanguages = useCallback(
    (languages: LanguageVersionWithName[]) => {
      for (const lang of languages) {
        setValue(`${cService.languageVersions}.${lang.language}.${cLv.isEnabled}`, true);
        setValue(`${cService.languageVersions}.${lang.language}.${cLv.name}`, lang.name);
      }
      switchFormModeAndSelectLanguage(formMetaDispatch, { mode: 'edit', language: languages[languages.length - 1].language });
      trackEvent({
        category: MatomoCategories.ServiceForm,
        action: MatomoActionTypes.AddLanguages,
      });
    },
    [formMetaDispatch, setValue, trackEvent]
  );

  const onServiceCopied = useCallback(
    (copy: ServiceFormValues) => {
      // When user wants to create a copy of the service we will redirect
      // her back to /service and pass the service in state.
      updateServiceAndResetForm(copy);
      switchFormModeAndSelectLanguage(formMetaDispatch, {
        mode: 'edit',
        language: getEnabledLanguagesByPriority(copy.languageVersions)[0],
      });
      navigate('/service', { state: { serviceCopy: copy } });
      trackEvent({
        category: MatomoCategories.ServiceForm,
        action: MatomoActionTypes.ServiceCopied,
      });
    },
    [updateServiceAndResetForm, formMetaDispatch, navigate, trackEvent]
  );

  const onServiceArchivedOrRestored = useCallback(
    (data: ServiceApiModel) => {
      const formModel = toServiceUiModel(data);
      updateServiceAndResetForm(formModel);
      navigate(`/service/${data.id}`);
      invalidateAndRefetchServiceFormHeaderData();
      trackEvent({
        category: MatomoCategories.ServiceForm,
        action: MatomoActionTypes.ServiceArchivedOrRestored,
      });
    },
    [updateServiceAndResetForm, navigate, invalidateAndRefetchServiceFormHeaderData, trackEvent]
  );

  const onPublishSucceeded = useCallback(
    (data: ServiceApiModel) => {
      updateServiceAndResetForm(toServiceUiModel(data));
      navigate(`/service/${data.id}`);
      invalidateAndRefetchServiceFormHeaderData();
      trackEvent({
        category: MatomoCategories.ServiceForm,
        action: MatomoActionTypes.PublishSucceeded,
      });
    },
    [updateServiceAndResetForm, navigate, invalidateAndRefetchServiceFormHeaderData, trackEvent]
  );

  const onSaveSucceededPublishFailed = useCallback(
    (data: ServiceApiModel) => {
      navigate(`/service/${data.id}`);
      updateServiceAndResetForm(toServiceUiModel(data));
      trackEvent({
        category: MatomoCategories.ServiceForm,
        action: MatomoActionTypes.SaveSucceededPublishFailed,
      });
    },
    [navigate, trackEvent, updateServiceAndResetForm]
  );

  const onTranslationOrderSucceeded = useCallback(
    (data: ServiceApiModel) => {
      navigate(`/service/${data.id}`);
      updateServiceAndResetForm(toServiceUiModel(data));
      invalidateAndRefetchServiceFormHeaderData();
      trackEvent({
        category: MatomoCategories.ServiceForm,
        action: MatomoActionTypes.TranslationOrderSucceeded,
      });
    },
    [navigate, updateServiceAndResetForm, invalidateAndRefetchServiceFormHeaderData, trackEvent]
  );

  const { canEdit } = useCanUpdateService({
    hasOtherModifiedVersion: !!props.service.otherModifiedVersion,
    responsibleOrgId: props.responsibleOrganization?.id,
    status: props.service.status,
  });

  return (
    <span className={props.className}>
      <Grid item className='header-wrapper'>
        <ServiceFormHeader
          language={selectedLanguageCode}
          addLanguages={addLanguages}
          control={props.control}
          resetForm={props.resetForm}
          otherModifiedVersion={props.service.otherModifiedVersion}
          hasTranslationOrder={hasTranslationOrder}
          responsibleOrgId={props.responsibleOrganization?.id}
          status={props.service.status}
          updateService={props.updateServiceAndResetForm}
          getFormValues={props.getFormValues}
          onServiceCopied={onServiceCopied}
          onServiceArchivedOrRestored={onServiceArchivedOrRestored}
          publishSucceeded={onPublishSucceeded}
          saveSucceededPublishFailed={onSaveSucceededPublishFailed}
          translationOrderSuccess={onTranslationOrderSucceeded}
        />
      </Grid>

      <Grid item>
        <FormFieldArea>
          <FormBlock mt={0} mb={2}>
            <Heading variant='h2'>{t('Ptv.Service.Form.Title.Text')}</Heading>
          </FormBlock>

          <LanguageTabs
            onTabChange={props.onTabChange}
            selectedLanguage={selectedLanguageCode}
            isComparing={displayComparison}
            getFormValues={props.getFormValues}
          >
            <SelectedGDChipsView generalDescription={props.generalDescription} language={uiLang} />
            <ExpanderGroup openAllText='Open all expander' closeAllText='Close all expanders' className='expander-group'>
              <Expander>
                <ExpanderTitleButton asHeading='h3'>{t('Ptv.Service.Form.BasicInfo.Title.Text')}</ExpanderTitleButton>
                <ExpanderContent>
                  {LanguagePriorities.map((lang) => {
                    return (
                      <TabPanel<Language> key={lang} tab={lang} selectedTab={selectedLanguageCode}>
                        <SelectedGdView selectedGeneralDescription={props.generalDescription} />
                        <FormDivider my={3} />
                        <LanguageVersionForm
                          generalDescription={props.generalDescription}
                          language={lang}
                          control={props.control}
                          enabledLanguages={props.enabledLanguages}
                          setValue={props.setValue}
                          getFormValues={props.getFormValues}
                          trigger={props.trigger}
                        />
                      </TabPanel>
                    );
                  })}
                </ExpanderContent>
              </Expander>
              <Expander>
                <ExpanderTitleButton asHeading='h3'>{t('Ptv.Service.Form.ClassificationAndKeywords.Title.Text')}</ExpanderTitleButton>
                <ExpanderContent>
                  <ClassificationAndKeywordsReadView
                    control={props.control}
                    language={selectedLanguageCode}
                    enabledLanguages={props.enabledLanguages}
                  />
                </ExpanderContent>
              </Expander>
              <Expander>
                <ExpanderTitleButton asHeading='h3' toggleButtonProps={{ id: `languageVersions[${selectedLanguageCode}].organization` }}>
                  {t('Ptv.Service.Form.ServiceOrganization.Title.Text')}
                </ExpanderTitleButton>
                <ExpanderContent>
                  {props.enabledLanguages.map((lang) => {
                    return (
                      <TabPanel<Language> key={lang} tab={lang} selectedTab={selectedLanguageCode}>
                        <ServiceOrganizationForm
                          control={props.control}
                          namespace={serviceOrganizationNamespace}
                          language={selectedLanguageCode}
                          enabledLanguages={props.enabledLanguages}
                          getFormValues={props.getFormValues}
                          setValue={props.setValue}
                          trigger={props.trigger}
                        />
                        <ServiceProvidersView
                          namespace={serviceOrganizationNamespace}
                          control={props.control}
                          setValue={props.setValue}
                          tabLanguage={selectedLanguageCode}
                        />
                        <FormDivider my={3} />
                        <FormBlock>
                          <ServiceVouchersView
                            control={props.control}
                            namespace={serviceOrganizationNamespace}
                            tabLanguage={selectedLanguageCode}
                          />
                        </FormBlock>
                      </TabPanel>
                    );
                  })}
                </ExpanderContent>
              </Expander>
            </ExpanderGroup>
          </LanguageTabs>
          <FormDivider mt={3} />
          <Box marginTop='25px'>
            <EditButton disabled={!canEdit || hasTranslationOrder} />
          </Box>
        </FormFieldArea>
      </Grid>
      <Grid item marginTop='15px'>
        <FormFieldArea>
          <ServiceChannelsForm
            control={props.control}
            language={selectedLanguageCode}
            getFormValues={props.getFormValues}
            setValue={setValue}
          />
        </FormFieldArea>
      </Grid>
      <Grid item marginTop='15px'>
        <FormFieldArea>
          <Box mb={2}>
            <Heading variant='h2'>{t('Ptv.Service.Publish.Title')}</Heading>
          </Box>
          <Box marginBottom='5px'>
            <Paragraph>
              <Text>{t('Ptv.Service.Publish.Description')}</Text>
            </Paragraph>
          </Box>
          <ServiceFormPublishFunctions
            control={props.control}
            hasOtherModifiedVersion={!!props.service.otherModifiedVersion}
            status={props.service.status}
            responsibleOrgId={props.responsibleOrganization?.id}
            updateService={props.updateServiceAndResetForm}
            getFormValues={props.getFormValues}
            publishSucceeded={onPublishSucceeded}
            saveSucceededPublishFailed={onSaveSucceededPublishFailed}
            alignItemsLeft={true}
          />
        </FormFieldArea>
      </Grid>
    </span>
  );
})(() => ({
  '& .header-wrapper': {
    marginTop: '20px',
    marginBottom: '20px',
  },
  '& .fi-expander-group.expander-group': {
    marginTop: '20px',
    '& .fi-expander-group_all-button': {
      display: 'none',
    },
  },
}));

ServiceFormViewMode.displayName = 'ServiceFormViewMode';
