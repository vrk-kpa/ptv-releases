import React from 'react';
import { useTranslation } from 'react-i18next';
import { styled } from '@mui/material/styles';
import { StaticChip } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { VisualHeading } from 'components/VisualHeading';
import { getGdValueOrDefault } from 'utils/gd';

type SelectedGDChipsViewProps = {
  generalDescription: GeneralDescriptionModel | null | undefined;
  language: Language;
  className?: string;
};

export const SelectedGDChipsView = styled((props: SelectedGDChipsViewProps) => {
  const { t } = useTranslation();

  if (!props.generalDescription) {
    return null;
  }

  return (
    <div className={props.className}>
      <VisualHeading className='visual-heading' variant='h5'>
        {t('Ptv.Service.Form.GdSearch.SelectedGd.Title')}
      </VisualHeading>
      <StaticChip>{getGdValueOrDefault(props.generalDescription.languageVersions, props.language, (lv) => lv.name, null)}</StaticChip>
    </div>
  );
})(({ theme }) => ({
  '& .visual-heading': {
    marginBottom: '10px',
  },
}));

SelectedGDChipsView.displayName = 'SelectedGDChipsView';
