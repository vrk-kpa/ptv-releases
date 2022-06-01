import React, { ReactElement, ReactNode, RefObject, cloneElement, isValidElement, useState } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { styled } from '@mui/material/styles';
import { Editor } from 'draft-js';
import { RhfTextEditor } from 'fields';
import { Label } from 'suomifi-ui-components';
import { Language, Mode } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { QualityResult } from 'types/qualityAgentResponses';
import { GdDescription } from './GdDescription';

type DescriptionProps = {
  className?: string;
  gd: GeneralDescriptionModel | null | undefined;
  id: string;
  name: string;
  gdFieldName: string;
  language: Language;
  labelText: string;
  hintText?: string;
  tooltipComponent?: ReactElement;
  placeHolder?: string;
  optionalText?: string;
  maxCharacters: number;
  compare?: boolean;
  mode: Mode;
  control: Control<ServiceModel>;
  qualityResults: QualityResult[];
  forwardedRef?: RefObject<Editor>;
};

export const Description = styled((props: DescriptionProps): React.ReactElement => {
  const { t } = useTranslation();
  const { control, labelText, name, id, gd, language, gdFieldName, qualityResults, tooltipComponent, className, optionalText, ...rest } =
    props;
  const [wrapperlRef, setWrapperlRef] = useState<HTMLDivElement | null>(null);

  function getTooltipComponent(tooltipComponent: ReactElement | undefined): ReactNode {
    if (isValidElement(tooltipComponent)) {
      return cloneElement(tooltipComponent, {
        anchorElement: wrapperlRef,
      });
    }
    return null;
  }

  const serviceValueLabel = gd ? t('Ptv.Service.Form.FromService.Label') : undefined;

  return (
    <div ref={(ref) => setWrapperlRef(ref)} className={className}>
      <div>
        <Label optionalText={optionalText} className='label-text'>
          {labelText}
        </Label>
        {!!tooltipComponent && getTooltipComponent(tooltipComponent)}
      </div>
      <GdDescription gd={gd} fieldName={gdFieldName} language={language} control={props.control} />
      <RhfTextEditor
        forwardedRef={props.forwardedRef}
        control={control}
        name={name}
        id={id}
        labelText={serviceValueLabel}
        qualityResults={qualityResults}
        {...rest}
      />
    </div>
  );
})(() => ({
  '& .label-text': {
    display: 'inline-block',
    verticalAlign: 'middle',
  },
}));
