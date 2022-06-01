import React, { RefObject } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Editor } from 'draft-js';
import { ConnectionFormModel } from 'types/forms/connectionFormTypes';
import { QualityResult } from 'types/qualityAgentResponses';
import { RhfTextEditor } from 'fields/RhfTextEditor';

export type DescriptionProps = {
  name: string;
  id: string;
  control: Control<ConnectionFormModel>;
  qualityResults?: QualityResult[];
  forwardedRef?: RefObject<Editor>;
};

export function Description(props: DescriptionProps): React.ReactElement {
  const { t } = useTranslation();

  return (
    <RhfTextEditor
      forwardedRef={props.forwardedRef}
      control={props.control}
      name={props.name}
      id={props.id}
      labelText={t('Ptv.ConnectionDetails.BasicInformation.Description.Label')}
      optionalText={t('Ptv.Common.Optional')}
      placeHolder={t('Ptv.ConnectionDetails.BasicInformation.Description.Placeholder')}
      maxCharacters={500}
      mode='edit'
      qualityResults={props.qualityResults}
    />
  );
}
