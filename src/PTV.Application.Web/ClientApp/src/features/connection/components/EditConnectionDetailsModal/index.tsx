import React, { useCallback, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useQueryClient } from 'react-query';
import { Box, Grid } from '@mui/material';
import { Button, ModalContent, ModalFooter, ModalTitle } from 'suomifi-ui-components';
import { ConnectionApiModel } from 'types/api/connectionApiModel';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel } from 'types/forms/connectionFormTypes';
import { ServiceFormValues } from 'types/forms/serviceFormTypes';
import { initialNotificationStatuses } from 'types/notificationStatus';
import LoadingIndicator from 'components/LoadingIndicator';
import { Message } from 'components/Message';
import PtvModal from 'components/PtvModal';
import { useFormMetaContext } from 'context/formMeta';
import { FormMetaContextProvider, IFormMetaContext } from 'context/formMeta/FormMetaContext';
import { QualityAgentContextProvider, createInitialState } from 'context/qualityAgent';
import { useSaveConnection } from 'hooks/queries/useSaveConnection';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { toUiModel } from 'mappers/connectionMapper';
import { toApiModel } from 'mappers/connectionSaveMapper';
import { getServiceModelValue } from 'utils/service';
import { ConnectionSummary } from 'features/connection/components/ConnectionSummary';
import { ConnectionContextProvider } from 'features/connection/context/ConnectionContextProvider';
import { validateConnectionFormModel } from 'features/connection/validation';
import { LanguageTabs } from './LanguageTabs';
import { SaveButton } from './SaveButton';
import { ValidationNotification } from './ValidationNotification';

type EditConnectionDetailsModalProps = {
  channel: ConnectableChannel;
  connection: ConnectionApiModel;
  visible: boolean;
  onCloseDiscardedChanges: () => void;
  onCloseSavedChanges: (model: ConnectionApiModel) => void;
  getFormValues: () => ServiceFormValues;
};

export function EditConnectionDetailsModal(props: EditConnectionDetailsModalProps): React.ReactElement {
  const { t } = useTranslation();
  const uiLang = useGetUiLanguage();
  const { selectedLanguageCode } = useFormMetaContext();
  const uiModel = toUiModel(props.connection);
  const meta = getFormMetaState(selectedLanguageCode ?? uiLang, uiModel);
  const query = useSaveConnection();
  const service = props.getFormValues();
  const serviceName = getServiceModelValue(service.languageVersions, meta.selectedLanguageCode, ({ name }) => name, '');
  const queryClient = useQueryClient();

  const { control, getValues, reset, trigger, setValue } = useForm<ConnectionFormModel>({
    defaultValues: uiModel,
    resolver: validateConnectionFormModel,
    mode: 'all',
    context: useFormMetaContext(),
  });

  useEffect(() => {
    // RHF does not automatically trigger validation. Trigger it when modal
    // is shown so that user sees the errors. Otherwise form's isValid is false
    // but errors object is empty -> nothing to show to the user.
    if (props.visible) {
      trigger();
    }
  }, [trigger, props.visible]);

  function onSavedSuccessfully(data: ConnectionApiModel) {
    reset(toUiModel(data));
    props.onCloseSavedChanges(data);
    queryClient.invalidateQueries(['next/connection/for-service'], { active: true });
  }

  function onSave() {
    const values = getValues();
    const apiModel = toApiModel(values);
    query.mutate(apiModel, { onSuccess: onSavedSuccessfully });
  }

  function close() {
    reset(toUiModel(props.connection));
    props.onCloseDiscardedChanges();
  }

  const getFormValues = useCallback((): ConnectionFormModel => {
    return getValues();
  }, [getValues]);

  return (
    <PtvModal appElementId='root' scrollable={true} visible={props.visible} onEscKeyDown={() => close()}>
      <ModalContent>
        <ModalTitle>{t('Ptv.ConnectionDetails.Form.Title.Text')}</ModalTitle>
        <ConnectionContextProvider connection={toUiModel(props.connection)}>
          <FormMetaContextProvider initialState={meta}>
            <QualityAgentContextProvider initialState={createInitialState()}>
              <ConnectionSummary serviceName={serviceName} serviceOrganization={service.responsibleOrganization} channel={props.channel} />
              <LanguageTabs control={control} setValue={setValue} getFormValues={getFormValues} trigger={trigger} />
            </QualityAgentContextProvider>
          </FormMetaContextProvider>
        </ConnectionContextProvider>
      </ModalContent>
      <ModalFooter>
        <Grid container>
          <Grid item container>
            <Grid item>
              <Box marginRight='15px'>
                <SaveButton isQueryRunning={query.isLoading} onSave={onSave} control={control} />
              </Box>
            </Grid>
            <Grid item>
              <Box marginRight='15px'>
                <Button variant='secondaryNoBorder' onClick={close}>
                  {t('Ptv.Action.Cancel.Label')}
                </Button>
              </Box>
            </Grid>
            {query.isLoading && (
              <Grid item>
                <LoadingIndicator />
              </Grid>
            )}
            {query.error && (
              <Grid item>
                <Message type='error'>{t('Ptv.Error.ServerError.SaveFailed')}</Message>
              </Grid>
            )}
          </Grid>
          <Grid item container>
            <ValidationNotification control={control} />
          </Grid>
        </Grid>
      </ModalFooter>
    </PtvModal>
  );
}

function getFormMetaState(wantedLanguage: Language, connection: ConnectionFormModel): IFormMetaContext {
  // Display the language version based on the ui language. If that does not exist, just use the first language
  const languages = Object.keys(connection.languageVersions) as Language[];
  const defaultLanguage = languages[0];
  const lang = languages.find((x) => x === wantedLanguage);

  return {
    selectedLanguageCode: lang || defaultLanguage,
    compareLanguageCode: undefined,
    availableLanguages: languages,
    displayComparison: false,
    mode: 'edit',
    serverError: undefined,
    notificationStatuses: initialNotificationStatuses,
  };
}
