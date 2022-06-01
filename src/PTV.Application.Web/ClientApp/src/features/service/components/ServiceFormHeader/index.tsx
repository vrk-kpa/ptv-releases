import React from 'react';
import { Control, useWatch } from 'react-hook-form';
import { Grid } from '@mui/material';
import { styled } from '@mui/material/styles';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { Language, PublishingStatus } from 'types/enumTypes';
import { OtherVersionType } from 'types/forms/otherVersionType';
import { ServiceFormValues, ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { LanguageVersionWithName } from 'types/languageVersionTypes';
import FormFunctions from 'components/FormFunctions';
import FormHistory from 'components/FormHistory';
import { FormDivider } from 'components/formLayout/FormDivider';
import { FormFieldArea } from 'components/formLayout/FormFieldArea';
import { useCanUpdateService } from 'hooks/security/useCanUpdateService';
import { getEnabledLanguagesWithName } from 'utils/service';
import { AddLanguageVersion } from 'features/service/components/AddLanguageVersion';
import { BackLink } from 'features/service/components/ServiceFormHeader/BackLink';
import { ServiceFormPublishFunctions } from 'features/service/components/ServiceFormPublishFunctions';
import { ServiceFormTitle } from 'features/service/components/ServiceFormTitle';
import { LanguageVersionTabsErrors } from 'features/service/components/ValidationErrors/LanguageVersionTabsErrors';
import { MissingOrganizationErrors } from 'features/service/components/ValidationErrors/MissingOrganizationErrors';
import LanguageVersions from './LanguageVersions';
import { NewerVersionNotification } from './NewerVersionNotification';
import { TranslationOrderNotification } from './TranslationOrderNotification';

const StyledFormFieldArea = styled(FormFieldArea)(() => ({
  '& .fi-notification': {
    marginBottom: '16px',
  },
}));

type ServiceFormHeaderProps = {
  language: Language;
  control: Control<ServiceModel>;
  otherModifiedVersion: OtherVersionType | null | undefined;
  hasTranslationOrder: boolean;
  status: PublishingStatus;
  responsibleOrgId: string | null | undefined;
  addLanguages: (languages: LanguageVersionWithName[]) => void;
  resetForm: () => void;
  updateService: (service: ServiceFormValues) => void;
  getFormValues: () => ServiceFormValues;
  onServiceCopied: (copy: ServiceFormValues) => void;
  onServiceArchivedOrRestored: (data: ServiceApiModel) => void;
  publishSucceeded: (data: ServiceApiModel) => void;
  saveSucceededPublishFailed: (data: ServiceApiModel) => void;
  translationOrderSuccess: (data: ServiceApiModel) => void;
};

export function ServiceFormHeader(props: ServiceFormHeaderProps): React.ReactElement {
  const { canChangeServiceStatus, canEdit } = useCanUpdateService({
    hasOtherModifiedVersion: !!props.otherModifiedVersion,
    responsibleOrgId: props.responsibleOrgId,
    status: props.status,
  });

  function getExistingLanguages(): LanguageVersionWithName[] {
    return getEnabledLanguagesWithName(props.getFormValues().languageVersions);
  }

  const lastTranslations = useWatch({ control: props.control, name: `${cService.lastTranslations}` });
  const gd = useWatch({ control: props.control, name: `${cService.generalDescription}` });

  return (
    <StyledFormFieldArea>
      <Grid container justifyContent='space-between' mb={2}>
        <Grid item>
          <BackLink control={props.control} resetForm={props.resetForm} />
          <ServiceFormTitle control={props.control} />
        </Grid>
        <Grid item alignSelf='end'>
          <ServiceFormPublishFunctions
            control={props.control}
            hasOtherModifiedVersion={!!props.otherModifiedVersion}
            status={props.status}
            responsibleOrgId={props.responsibleOrgId}
            updateService={props.updateService}
            getFormValues={props.getFormValues}
            publishSucceeded={props.publishSucceeded}
            saveSucceededPublishFailed={props.saveSucceededPublishFailed}
            alignItemsLeft={false}
          />
        </Grid>
      </Grid>
      <FormDivider mb={3} />
      <LanguageVersionTabsErrors language={props.language} />
      <MissingOrganizationErrors language={props.language} control={props.control} />
      <NewerVersionNotification otherModifiedVersion={props.otherModifiedVersion} />
      <TranslationOrderNotification hasTranslationOrder={props.hasTranslationOrder} lastTranslations={lastTranslations} />
      <FormFunctions
        control={props.control}
        canUpdate={canChangeServiceStatus}
        hasTranslationOrder={props.hasTranslationOrder}
        getFormValues={props.getFormValues}
        onServiceCopied={props.onServiceCopied}
        onServiceArchivedOrRestored={props.onServiceArchivedOrRestored}
        translationOrderSuccess={props.translationOrderSuccess}
      />
      <FormHistory control={props.control} selectedLanguage={props.language} />
      <LanguageVersions control={props.control} />
      <AddLanguageVersion
        hasGeneralDescription={!!gd}
        disabled={!canEdit || props.hasTranslationOrder}
        addLanguages={props.addLanguages}
        getExistingLanguages={getExistingLanguages}
      />
    </StyledFormFieldArea>
  );
}
