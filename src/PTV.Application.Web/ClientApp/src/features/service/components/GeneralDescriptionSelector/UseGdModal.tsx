import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Button, Checkbox, Modal, ModalContent, ModalFooter, ModalTitle, Paragraph, Text } from 'suomifi-ui-components';
import { ServiceFormValues } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import LoadingIndicator from 'components/LoadingIndicator';
import { useFormMetaContext } from 'context/formMeta';
import { useGetPublishedGdByUnificRootId } from 'hooks/queries/useGetPublishedGdByUnificRootId';
import { GdSearchItem } from 'hooks/queries/useSearchGeneralDescriptions';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { toGDUiModel } from 'mappers/gdMapper';
import { getServiceModelValue } from 'utils/service';
import { getTextByLangPriority } from 'utils/translations';

const useStyles = makeStyles(() => ({
  currentName: {
    marginBottom: '20px',
  },
  checkbox: {
    marginTop: '20px',
  },
  loadingIndicator: {
    display: 'flex',
    flex: 1,
    justifyContent: 'center',
    alignItems: 'flex-start',
  },
}));

type UseGdModalProps = {
  searchResult: GdSearchItem;
  isOpen: boolean;
  close: () => void;
  select: (gd: GeneralDescriptionModel, useGdName: boolean) => void;
  getFormValues: () => ServiceFormValues;
};

function CurrentName({ currentName }: { currentName: string | null | undefined }): React.ReactElement | null {
  const { t } = useTranslation();
  if (!currentName) {
    return null;
  }
  return (
    <div>
      <Text variant='bold'>{t('Ptv.Service.Form.GdSelect.ServiceName.Title')}</Text>
      <Paragraph>{currentName}</Paragraph>
    </div>
  );
}

export function UseGdModal(props: UseGdModalProps): React.ReactElement {
  const { t } = useTranslation();
  const { selectedLanguageCode } = useFormMetaContext();
  const [checked, setChecked] = useState<boolean>(true);
  const classes = useStyles();
  const uiLang = useGetUiLanguage();

  const query = useGetPublishedGdByUnificRootId(props.searchResult.unificRootId);

  const formValues = props.getFormValues();
  const serviceName = getServiceModelValue(formValues.languageVersions, selectedLanguageCode, (x) => x.name, null);

  function select() {
    if (!query.data) {
      return;
    }

    const uiModel = toGDUiModel(query.data);
    props.select(uiModel, checked);
  }

  return (
    <Modal appElementId='root' scrollable={false} visible={props.isOpen} onEscKeyDown={() => props.close()}>
      <ModalContent>
        <ModalTitle>{t('Ptv.Service.Form.GdSelect.Title')}</ModalTitle>
        <div>
          {serviceName && (
            <div className={classes.currentName}>
              <CurrentName currentName={serviceName} />
            </div>
          )}
          <div>
            <Text variant='bold'>{t('Ptv.Service.Form.GdSelect.GdName.Title')}</Text>
          </div>
          <Paragraph>{getTextByLangPriority(uiLang, props.searchResult.names, '')}</Paragraph>
          <div className={classes.checkbox}>
            <Checkbox
              checked={checked}
              onClick={({ checkboxState }) => {
                setChecked(checkboxState);
              }}
            >
              {t('Ptv.Service.Form.GdSelect.ReplaceName.Label')}
            </Checkbox>
          </div>
          <div className={classes.loadingIndicator}>{query.isLoading && <LoadingIndicator />}</div>
        </div>
      </ModalContent>
      <ModalFooter>
        <Button disabled={query.isLoading} onClick={select}>
          {t('Ptv.Common.Yes')}
        </Button>
        <Button variant='secondary' onClick={() => props.close()}>
          {t('Ptv.Action.Cancel.Label')}
        </Button>
      </ModalFooter>
    </Modal>
  );
}
