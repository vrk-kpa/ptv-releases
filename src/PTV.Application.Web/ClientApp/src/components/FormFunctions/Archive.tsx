import React, { useState } from 'react';
import { Control, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { Button, Heading, Paragraph } from 'suomifi-ui-components';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { Language, PublishingStatus } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { CellType, RowType, Table } from 'components/Table';
import { getEnabledLanguagesByPriority } from 'utils/service';
import { areAllVersionsArchived, areOtherVersionsArchived, isArchived, isArchivedOrRemoved } from 'utils/status';
import { getKeyForLanguage, getKeysForStatusType } from 'utils/translations';
import { ArchiveServiceModal } from 'features/service/components/Archive/ArchiveServiceModal';
import { RestoreServiceModal } from 'features/service/components/Restore/RestoreServiceModal';

type ArchiveProps = {
  control: Control<ServiceModel>;
  canChangeState: boolean;
  hasTranslationOrder: boolean;
  onServiceArchivedOrRestored: (data: ServiceApiModel) => void;
};

type ModalSettings = {
  serviceId: string | null | undefined;
  language: Language | null | undefined;
  isOpen: boolean;
};

export default function Archive(props: ArchiveProps): React.ReactElement {
  const { t } = useTranslation();
  const [archiveSettings, setArchiveSettings] = useState<ModalSettings>({ serviceId: null, language: null, isOpen: false });
  const [restoreSettings, setRestoreSettings] = useState<ModalSettings>({ serviceId: null, language: null, isOpen: false });

  const languageVersions = useWatch({ control: props.control, name: `${cService.languageVersions}`, exact: true });
  const serviceId = useWatch({ control: props.control, name: `${cService.id}` });
  const serviceStatus = useWatch({ control: props.control, name: `${cService.status}` });

  const languages = getEnabledLanguagesByPriority(languageVersions);
  const isAllArchived = isArchived(serviceStatus);

  const serviceHasNotBeenRemoved = !!serviceId && serviceStatus !== 'Removed';
  const canArchiveAll = !!serviceId && !isArchivedOrRemoved(serviceStatus) && props.canChangeState && !props.hasTranslationOrder;
  const canRestoreAll = serviceHasNotBeenRemoved && isAllArchived && props.canChangeState;

  const headers = [t('Ptv.Form.Header.Archive.Language'), t('Ptv.Form.Header.Archive.Name'), t('Ptv.Form.Header.Archive.Status'), ''];

  function canRestore(lvStatus: PublishingStatus): boolean {
    // If all the language versions have been archived you cannot restore just one of them.
    // Same issue in the old UI. Best guess: this would leave the service
    // in wrong state where all language versions have been restored but the service has not been.
    if (areAllVersionsArchived(languageVersions)) return false;
    return !!serviceId && isArchived(lvStatus) && props.canChangeState;
  }

  function canArchive(language: Language, lvStatus: PublishingStatus): boolean {
    // If you have multiple language versions and all but one of them is archived then you cannot arcive the
    // last one (server throws exception). Same issue in the old UI. Best guess: this would leave the service
    // in wrong state where all language versions have been archived but the service has not been.
    if (areOtherVersionsArchived(language, languageVersions)) return false;
    return serviceHasNotBeenRemoved && !isArchived(lvStatus) && props.canChangeState;
  }

  const renderCellButton = (language: Language, lvStatus: PublishingStatus, hasTranslationOrder: boolean): React.ReactElement => {
    return (
      <>
        {isArchived(lvStatus) && (
          <Button
            disabled={!canRestore(lvStatus) || hasTranslationOrder}
            variant='secondaryNoBorder'
            icon='mapLayers'
            onClick={() => toggleRestoreModal(true, language)}
          >
            {t('Ptv.Form.Header.Archive.RestoreButton')}
          </Button>
        )}
        {!isArchived(lvStatus) && (
          <Button
            disabled={!canArchive(language, lvStatus) || hasTranslationOrder}
            variant='secondaryNoBorder'
            icon='mapLayers'
            onClick={() => toggleArchiveModal(true, language)}
          >
            {t('Ptv.Form.Header.Archive.ArchiveButton')}
          </Button>
        )}
      </>
    );
  };

  const getCells = (ln: Language): CellType[] => {
    const languageVersion = languageVersions[ln];
    return [
      { value: t(getKeyForLanguage(ln)) },
      { value: languageVersion.name },
      { value: t(getKeysForStatusType(languageVersion.status)) },
      {
        value: '0',
        customComponent: renderCellButton(ln, languageVersion.status, props.hasTranslationOrder),
      },
    ];
  };

  const getRows = (): RowType[] =>
    languages.map((ln) => {
      return {
        cells: getCells(ln),
        id: ln,
      };
    });

  function toggleArchiveModal(isOpen: boolean, language: Language | null | undefined = null) {
    setArchiveSettings({ serviceId: serviceId, language: language, isOpen: isOpen });
  }

  function toggleRestoreModal(isOpen: boolean, language: Language | null | undefined = null) {
    setRestoreSettings({ serviceId: serviceId, language: language, isOpen: isOpen });
  }

  function archiveServiceSuccess(data: ServiceApiModel) {
    props.onServiceArchivedOrRestored(data);
    setArchiveSettings({ serviceId: null, language: null, isOpen: false });
  }

  function restoreServiceSuccess(data: ServiceApiModel) {
    props.onServiceArchivedOrRestored(data);
    setRestoreSettings({ serviceId: null, language: null, isOpen: false });
  }

  return (
    <div>
      <Heading variant='h4' as='h3'>
        {t('Ptv.Form.Header.Archive.Title')}
      </Heading>
      <Paragraph>{t('Ptv.Form.Header.Archive.Description')}</Paragraph>
      <Box mb='20px'>
        <Table ariaLabel={t('Ptv.Form.Header.Archive.Title')} headers={headers} rows={getRows()} />
      </Box>

      {isAllArchived && (
        <Button disabled={!canRestoreAll} variant='secondary' icon='mapLayers' onClick={() => toggleRestoreModal(true)}>
          {t('Ptv.Form.Header.Archive.RestoreAllButton')}
        </Button>
      )}

      <RestoreServiceModal
        serviceId={serviceId}
        isOpen={restoreSettings.isOpen}
        language={restoreSettings.language}
        success={restoreServiceSuccess}
        cancel={() => toggleRestoreModal(false)}
      />

      {!isAllArchived && (
        <Button disabled={!canArchiveAll} variant='secondary' icon='mapLayers' onClick={() => toggleArchiveModal(true)}>
          {t('Ptv.Form.Header.Archive.ArchiveAllButton')}
        </Button>
      )}
      <ArchiveServiceModal
        serviceId={serviceId}
        isOpen={archiveSettings.isOpen}
        language={archiveSettings.language}
        success={archiveServiceSuccess}
        cancel={() => toggleArchiveModal(false)}
      />
    </div>
  );
}
