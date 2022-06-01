import React, { FunctionComponent } from 'react';
import { Control, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { Language, Mode } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { HeadingWithTooltip } from 'components/HeadingWithTooltip';
import { VisualHeading } from 'components/VisualHeading';
import { useGdSpecificTranslationKey } from 'hooks/translations/useGdSpecificTranslationKey';
import { getGdValueOrDefault } from 'utils/gd';
import { ServiceLawsEdit } from './ServiceLawsEdit';
import { ServiceLawsGdView } from './ServiceLawsGdView';
import { ServiceLawsView } from './ServiceLawsView';

type ServiceLawsProps = {
  name: string;
  id: string;
  mode: Mode;
  language: Language;
  gd: GeneralDescriptionModel | null | undefined;
  control: Control<ServiceModel>;
  trigger: UseFormTrigger<ServiceModel>;
  className?: string;
};

export const ServiceLaws: FunctionComponent<ServiceLawsProps> = (props: ServiceLawsProps) => {
  const { t } = useTranslation();
  const tooltipKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.Field.Laws.Tooltip',
    'Ptv.Service.Form.Field.Laws.GdSelected.Tooltip'
  );

  function renderLawsView(): React.ReactElement {
    if (props.gd) {
      const laws = getGdValueOrDefault(props.gd?.languageVersions, props.language, (x) => x.laws, undefined);
      return (
        <div>
          <Box mt={2}>
            <VisualHeading smallScreen variant='h5'>
              {t('Ptv.Service.Form.FromGD.Label')}
            </VisualHeading>
            <ServiceLawsGdView id={props.id} name={props.name} value={laws || []} />
          </Box>
          <Box mt={2}>
            <VisualHeading smallScreen variant='h5'>
              {t('Ptv.Service.Form.FromService.Label')}
            </VisualHeading>
            <ServiceLawsView name={props.name} id={props.id} control={props.control} language={props.language} />
          </Box>
        </div>
      );
    }

    return <ServiceLawsView name={props.name} id={props.id} control={props.control} language={props.language} />;
  }

  function renderLawsEdit(): React.ReactElement {
    return (
      <ServiceLawsEdit
        name={props.name}
        id={props.id}
        language={props.language}
        gd={props.gd}
        control={props.control}
        trigger={props.trigger}
      />
    );
  }

  return (
    <Box className={props.className}>
      <HeadingWithTooltip variant='h4' tooltipContent={t(tooltipKey)} tooltipAriaLabel={t('Ptv.Service.Form.Field.Laws.Label')}>
        {t('Ptv.Service.Form.Field.Laws.Label')}
      </HeadingWithTooltip>
      {props.mode === 'view' && renderLawsView()}
      {props.mode === 'edit' && renderLawsEdit()}
    </Box>
  );
};
