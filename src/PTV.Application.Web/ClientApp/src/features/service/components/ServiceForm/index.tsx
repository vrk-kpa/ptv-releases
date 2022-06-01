import React, { useCallback, useContext, useEffect } from 'react';
import { useForm, useWatch } from 'react-hook-form';
import { useQueryClient } from 'react-query';
import { useNavigate } from 'react-router';
import { DevTool } from '@hookform/devtools';
import { useMatomo } from '@jonkoops/matomo-tracker-react';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { Language } from 'types/enumTypes';
import { ServiceFormValues, ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { MatomoActionTypes, MatomoCategories } from 'types/matomo';
import { HttpError } from 'types/miscellaneousTypes';
import {
  DispatchContext as FormMetaDispatchContext,
  selectLanguage,
  setServerError,
  switchCompareMode,
  switchFormMode,
  useFormMetaContext,
} from 'context/formMeta';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { useSaveService } from 'hooks/service/useSaveService';
import { useGetEnabledLanguagesByPriority } from 'hooks/useGetEnabledLanguagesByPriority';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { toServiceApiModel, toServiceUiModel } from 'mappers/serviceMapper';
import { DispatchContext as ServiceDispatchContext } from 'features/service/DispatchContextProvider';
import { ValidateService } from 'features/service/actions';
import { validateServiceForm } from 'features/service/validation';
import { createServiceFormContext } from 'features/service/validation/context';
import { ServiceFormEditMode } from './EditMode';
import { ServiceFormViewMode } from './ViewMode';
import { useTriggerFormValidation } from './useTriggerFormValidation';

const useStyles = makeStyles(() => ({
  fullArea: {
    display: 'flex',
    flex: 1,
  },
  header: {
    marginTop: '20px',
    marginBottom: '20px',
  },
}));

type ServiceFormProps = {
  service: ServiceFormValues;
  updateService?: (service: ServiceFormValues) => void;
};

export function ServiceForm(props: ServiceFormProps): React.ReactElement {
  const queryClient = useQueryClient();
  const classes = useStyles();
  const navigate = useNavigate();
  const appContext = useAppContextOrThrow();
  const uiLang = useGetUiLanguage();
  const formMetaDispatch = useContext(FormMetaDispatchContext);
  const serviceDispatch = useContext(ServiceDispatchContext);
  const { mode, displayComparison } = useFormMetaContext();
  const saveMutation = useSaveService();
  const formContext = createServiceFormContext(appContext, uiLang);
  const saveInProgress = saveMutation.isLoading;
  const { trackPageView, trackEvent } = useMatomo();

  useEffect(() => {
    trackPageView({});
  }, [trackPageView]);

  function onCompareModeChanged() {
    switchCompareMode(formMetaDispatch, !displayComparison);
  }

  const { control, getValues, setValue, reset, trigger } = useForm<ServiceModel>({
    defaultValues: props.service,
    mode: 'all',
    resolver: validateServiceForm,
    context: formContext,
  });

  useTriggerFormValidation({ trigger: trigger });

  const responsibleOrganization = useWatch({ control: control, name: `${cService.responsibleOrganization}` });
  const generalDescription = useWatch({ control: control, name: `${cService.generalDescription}` });

  const updateServiceAndResetForm = useCallback(
    (formModel: ServiceFormValues) => {
      reset(formModel);
      props.updateService?.(formModel);
    },
    [props, reset]
  );

  const invalidateAndRefetchServiceFormHeaderData = useCallback(() => {
    queryClient.invalidateQueries(['editHistoryQuery'], { active: true });
    queryClient.invalidateQueries(['translationHistoryQuery'], { active: true });
    queryClient.invalidateQueries(['connectionHistoryQuery'], { active: true });
  }, [queryClient]);

  const saveDraft = useCallback(() => {
    const saveSucceeded = (data: ServiceApiModel) => {
      navigate(`/service/${data.id}`, { replace: true });
      switchFormMode(formMetaDispatch, 'view');
      const formModel = toServiceUiModel(data);
      updateServiceAndResetForm(formModel);
      invalidateAndRefetchServiceFormHeaderData();
      setServerError(formMetaDispatch, undefined);
      ValidateService(serviceDispatch, null);
      trackEvent({
        category: MatomoCategories.ServiceForm,
        action: MatomoActionTypes.SaveDraftSucceeded,
      });
    };

    const saveFailed = (error: HttpError) => {
      setServerError(formMetaDispatch, error?.details);
      trackEvent({
        category: MatomoCategories.ServiceForm,
        action: MatomoActionTypes.SaveDraftFailed,
      });
    };

    const apiModel = toServiceApiModel(getValues());
    saveMutation.mutate(apiModel, {
      onSuccess: saveSucceeded,
      onError: saveFailed,
    });
  }, [
    getValues,
    saveMutation,
    navigate,
    formMetaDispatch,
    updateServiceAndResetForm,
    invalidateAndRefetchServiceFormHeaderData,
    serviceDispatch,
    trackEvent,
  ]);

  const onTabChange = useCallback(
    (language: Language) => {
      selectLanguage(formMetaDispatch, language);
    },
    [formMetaDispatch]
  );

  const getFormValues = useCallback((): ServiceFormValues => {
    return getValues();
  }, [getValues]);

  const resetForm = useCallback(() => {
    setServerError(formMetaDispatch, undefined);
    ValidateService(serviceDispatch, null);
    reset();
    trackEvent({
      category: MatomoCategories.ServiceForm,
      action: MatomoActionTypes.ResetForm,
    });
  }, [formMetaDispatch, reset, serviceDispatch, trackEvent]);

  const enabledLanguages = useGetEnabledLanguagesByPriority(control);

  return (
    <div className={classes.fullArea}>
      <form className={classes.fullArea}>
        <Grid item container direction='column' paddingBottom='80px'>
          {mode === 'edit' && (
            <ServiceFormEditMode
              service={props.service}
              generalDescription={generalDescription}
              enabledLanguages={enabledLanguages}
              saveInProgress={saveInProgress}
              control={control}
              responsibleOrganization={responsibleOrganization}
              onTabChange={onTabChange}
              trigger={trigger}
              setValue={setValue}
              onCompareModeChanged={onCompareModeChanged}
              resetForm={resetForm}
              saveDraft={saveDraft}
              getFormValues={getFormValues}
            />
          )}
          {mode === 'view' && (
            <ServiceFormViewMode
              service={props.service}
              generalDescription={generalDescription}
              enabledLanguages={enabledLanguages}
              control={control}
              responsibleOrganization={responsibleOrganization}
              formMetaDispatch={formMetaDispatch}
              onTabChange={onTabChange}
              trigger={trigger}
              setValue={setValue}
              onCompareModeChanged={onCompareModeChanged}
              resetForm={resetForm}
              getFormValues={getFormValues}
              updateServiceAndResetForm={updateServiceAndResetForm}
              invalidateAndRefetchServiceFormHeaderData={invalidateAndRefetchServiceFormHeaderData}
            />
          )}
        </Grid>
      </form>
      <DevTool control={control} />
    </div>
  );
}
