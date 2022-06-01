import React from 'react';
import { Control, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Heading, Icon } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceLanguageVersionValues, ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { CellWithStatus } from 'components/Cells';
import { CellType, RowType, SmallTable } from 'components/Table';
import { displayDate, toOptionalDateTime } from 'utils/date';
import { getEnabledLanguagesByPriority } from 'utils/service';
import { getKeyForLanguage, getKeysForStatusType } from 'utils/translations';

const useStyles = makeStyles(() => ({
  narrow: {
    width: '20%',
  },
  scheduledIcon: {
    verticalAlign: 'middle !important',
    marginRight: '10px',
  },
  title: {
    marginTop: '20px',
  },
}));

type LanguageVersionsProps = {
  control: Control<ServiceModel>;
};

export default function LanguageVersions(props: LanguageVersionsProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const languageVersions = useWatch({ control: props.control, name: `${cService.languageVersions}`, exact: true });
  const languages = getEnabledLanguagesByPriority(languageVersions);

  const renderSchedulingCell = (languageVersion: ServiceLanguageVersionValues): React.ReactElement | null => {
    const { scheduledArchive, scheduledPublish } = languageVersion;
    let message: string;

    if (!scheduledArchive && !scheduledPublish) {
      return null;
    }

    if (!!scheduledArchive) {
      message = !!scheduledPublish ? 'Ptv.Form.Header.LanguageStatus.ScheduledBoth' : 'Ptv.Form.Header.LanguageStatus.ScheduledArchive';
    } else {
      message = 'Ptv.Form.Header.LanguageStatus.ScheduledPublish';
    }

    return (
      <div>
        <Icon icon='calendar' className={classes.scheduledIcon} />
        <span>
          {t(message, {
            publish: displayDate(toOptionalDateTime(scheduledPublish)),
            archive: displayDate(toOptionalDateTime(scheduledArchive)),
          })}
        </span>
      </div>
    );
  };

  const getCells = (language: Language): CellType[] => {
    const languageVersion = languageVersions[language];

    return [
      {
        value: t(getKeyForLanguage(language)),
        className: classes.narrow,
        asHeader: true,
      },
      {
        value: languageVersion.status,
        className: classes.narrow,
        customComponent: (
          <CellWithStatus
            id={`languageVersion-${language}-status`}
            status={languageVersion.status}
            value={t(getKeysForStatusType(languageVersion.status))}
          />
        ),
      },
      {
        value: '',
        customComponent: renderSchedulingCell(languageVersion),
      },
    ];
  };

  const getRows = (): RowType[] => {
    return languages.map((lang) => ({
      id: lang,
      cells: getCells(lang),
    }));
  };

  return (
    <div className={classes.title}>
      <Heading variant='h5' as='h2'>
        {t('Ptv.Form.Header.LanguageStatus')}
      </Heading>
      <SmallTable ariaLabel={t('Ptv.Form.Header.LanguageStatus')} rows={getRows()} />
    </div>
  );
}
