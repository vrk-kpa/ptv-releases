import React, { ReactElement, ReactNode, cloneElement, isValidElement, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { styled } from '@mui/material/styles';
import { ExternalLink, Text } from 'suomifi-ui-components';
import { NoValueLabel } from 'components/NoValueLabel/NoValueLabel';
import { VisualHeading } from 'components/VisualHeading';

type RhfReadOnlyFieldProps = {
  value: ReactNode;
  tooltipComponent?: ReactElement;
  labelText?: string;
  asLink?: boolean;
  id: string;
  variant?: 'body' | 'lead' | 'bold';
  className?: string;
};

export const RhfReadOnlyField = styled((props: RhfReadOnlyFieldProps): React.ReactElement | null => {
  const { id, labelText, tooltipComponent, asLink, value, variant = 'body', className } = props;
  const { t } = useTranslation();
  const [wrapperRef, setWrapperRef] = useState<HTMLDivElement | null>(null);

  function getTooltipComponent(tooltipComponent: ReactElement | undefined): ReactNode {
    if (isValidElement(tooltipComponent)) {
      return cloneElement(tooltipComponent, {
        anchorElement: wrapperRef,
      });
    }
    return null;
  }

  return (
    <div className={className} id={id} ref={(ref) => setWrapperRef(ref)}>
      {labelText && (
        <div className='label'>
          <VisualHeading variant='h5' className='label-text'>
            {labelText}
          </VisualHeading>
          {!!tooltipComponent && getTooltipComponent(tooltipComponent)}
        </div>
      )}
      {(value &&
        (asLink ? (
          <ExternalLink className='small-text' labelNewWindow={t('Ptv.Link.Label.NewWindow')} href={value.toString()}>
            {value}
          </ExternalLink>
        ) : (
          <Text variant={variant} className='small-text'>
            {value}
          </Text>
        ))) || <NoValueLabel />}
    </div>
  );
})(() => ({
  '& .label': {
    display: 'block',
    marginBottom: '10px',
    '& .label-text': {
      verticalAlign: 'middle',
      display: 'inline',
    },
  },
  '& .small-text': {
    fontSize: '16px',
  },
}));
