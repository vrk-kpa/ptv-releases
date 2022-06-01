import React, { useContext } from 'react';
import { Control } from 'react-hook-form';
import { styled } from '@mui/material/styles';
import { DateTime } from 'luxon';
import { Modal } from 'suomifi-ui-components';
import { getKeys } from 'utils';
import { PublishingType } from 'types/api/publishingType';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { ValidationMessageModel, ValidationModel } from 'types/api/validationModel';
import { Language } from 'types/enumTypes';
import { ServiceFormValues, ServiceModel } from 'types/forms/serviceFormTypes';
import { LanguageVersionType } from 'types/languageVersionTypes';
import { HttpError } from 'types/miscellaneousTypes';
import { DispatchContext, setServerError } from 'context/formMeta';
import { resetNotificationVisibilities } from 'context/formMeta/actions';
import { PublishServiceParameters, usePublishService } from 'hooks/service/usePublishService';
import { useServiceValidation } from 'hooks/service/useValidateService';
import { toServiceApiModel } from 'mappers/serviceMapper';
import { DispatchContext as ServiceDispatchContext } from 'features/service/DispatchContextProvider';
import { ValidateService } from 'features/service/actions';
import { PublishDialogContent } from './PublishDialogContent';

export const StyledDiv = styled('div')`
  margin: 0 0 20px 0;
  & .fi-checkbox {
    margin-bottom: 20px;
  }
  & .fi-hint-text {
    margin-bottom: 10px;
  }
  & .fi-label-text {
    margin-bottom: 10px;
  }
`;

export type PublishDialogItem = {
  language: Language;
  checked: boolean;
  scheduledPublish: boolean;
  scheduledPublishDate: DateTime;
  isValidPublishDate: boolean;
  scheduledArchive: boolean;
  scheduledArchiveDate: DateTime;
  isValidArchiveDate: boolean;
  isValidDateCombination: boolean;
};

type PublishDialogProps = {
  isOpen: boolean;
  control: Control<ServiceModel>;
  close: () => void;
  updateService: (service: ServiceFormValues) => void;
  getFormValues: () => ServiceFormValues;
  publishSucceeded: (data: ServiceApiModel) => void;
  saveSucceededPublishFailed: (data: ServiceApiModel) => void;
};

export function PublishDialog(props: PublishDialogProps): React.ReactElement {
  const metaDispatchContext = useContext(DispatchContext);
  const serviceDispatch = useContext(ServiceDispatchContext);

  // api calls
  const validateServiceApi = useServiceValidation();
  const publishServiceApi = usePublishService();

  const serviceApiModel = toServiceApiModel(props.getFormValues());

  function selectedLanguagesHaveValidationErrors(
    validationResult: ValidationModel<ServiceApiModel>,
    publishState: LanguageVersionType<PublishDialogItem>
  ): boolean {
    const selectedLanguages = Object.values(publishState)
      .filter((value) => value.checked)
      .map((item) => item.language);
    for (const lang of selectedLanguages) {
      const model = validationResult.validatedFields[lang];
      if (!model) continue;
      if (model.length !== 0) return true;
    }

    return false;
  }

  function publishService(
    service: ServiceApiModel,
    publishState: LanguageVersionType<PublishDialogItem>,
    validationResult: ValidationModel<ServiceApiModel>
  ) {
    const selectedLangsValidationResult = {
      entity: validationResult.entity,
      validatedFields: getKeys(validationResult.validatedFields).reduce((results, lang: Language) => {
        if (publishState[lang]?.checked) {
          results[lang] = validationResult.validatedFields[lang];
          return results;
        }
        return results;
      }, {} as LanguageVersionType<ValidationMessageModel[]>),
    };

    ValidateService(serviceDispatch, selectedLangsValidationResult);
    if (selectedLanguagesHaveValidationErrors(selectedLangsValidationResult, publishState)) {
      resetNotificationVisibilities(metaDispatchContext);
      return;
    }
    publishServiceApi.mutate(createPublishRequest(service.id || '', publishState), {
      onSuccess: onClosePublishSucceeded,
    });
  }

  function handlePublish(publishState: LanguageVersionType<PublishDialogItem>) {
    validateServiceApi.mutate(serviceApiModel, {
      onSuccess: (validationResult: ValidationModel<ServiceApiModel>) => {
        setServerError(metaDispatchContext, undefined);
        publishService(serviceApiModel, publishState, validationResult);
      },
      onError: (error: HttpError) => {
        if (error.details) {
          setServerError(metaDispatchContext, error.details);
        }
      },
    });
  }

  function resetFormAndClose() {
    validateServiceApi.reset();
    props.close();
  }

  function onClosePublishSucceeded(data: ServiceApiModel) {
    resetFormAndClose();
    props.publishSucceeded(data);
  }

  const isLoading = validateServiceApi.isLoading || publishServiceApi.isLoading;
  const hasError = validateServiceApi.error || publishServiceApi.error;

  function getErrorKey(): string {
    if (validateServiceApi.error) return 'Ptv.Error.ServerError.ValidationFailed';
    if (publishServiceApi.error) return 'Ptv.Error.ServerError.PublishFailed';
    return '';
  }

  return (
    <Modal appElementId='root' visible={props.isOpen} onEscKeyDown={resetFormAndClose}>
      <PublishDialogContent
        control={props.control}
        isLoading={isLoading}
        hasError={!!hasError}
        errorKey={getErrorKey()}
        validationResult={validateServiceApi.data}
        getFormValues={props.getFormValues}
        onPublish={(publishState) => handlePublish(publishState)}
        onCancel={resetFormAndClose}
      />
    </Modal>
  );
}

function createPublishRequest(serviceId: string, publishState: LanguageVersionType<PublishDialogItem>): PublishServiceParameters {
  const request = {} as LanguageVersionType<PublishingType>;

  const languages = getKeys(publishState);

  for (const language of languages) {
    /* eslint-disable @typescript-eslint/no-non-null-assertion */
    const element = publishState[language]!;

    if (!element.checked) {
      continue;
    }

    request[language] = {
      status: 'Published',
      publishAt: element.scheduledPublish ? element.scheduledPublishDate?.toISO() : null,
      archiveAt: element.scheduledArchive ? element.scheduledArchiveDate?.toISO() : null,
    };
  }

  return {
    serviceId: serviceId,
    data: request,
  };
}
