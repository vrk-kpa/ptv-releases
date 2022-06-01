import React, { ReactNode, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { styled } from '@mui/material/styles';
import clsx from 'clsx';
import { Heading, HeadingProps } from 'suomifi-ui-components';
import { Tooltip } from 'components/Tooltip';

type HeadingWithTooltipProps = HeadingProps & {
  tooltipContent: ReactNode;
  tooltipAriaLabel: string;
  headingClassName?: string;
};

export const HeadingWithTooltip = styled((props: HeadingWithTooltipProps) => {
  const { tooltipContent, children, className, headingClassName, tooltipAriaLabel, ...rest } = props;
  const [wrapperRef, setWrapperlRef] = useState<HTMLDivElement | null>(null);
  const { t } = useTranslation();

  return (
    <div className={clsx(className, 'heading-wrapper')} ref={(ref) => setWrapperlRef(ref)}>
      <Heading {...rest} className={clsx(headingClassName, 'heading-with-tooltip')}>
        {children}
      </Heading>
      {!!tooltipContent && (
        <Tooltip
          ariaInfoButtonLabelText={t('Ptv.Common.Tooltip.Button.Label', { label: tooltipAriaLabel })}
          ariaCloseButtonLabelText={t('Ptv.Common.Tooltip.CloseButton.Label', { label: tooltipAriaLabel })}
          anchorElement={wrapperRef}
        >
          {tooltipContent}
        </Tooltip>
      )}
    </div>
  );
})(() => ({
  '&.heading-wrapper': {
    marginBottom: '20px',
  },
  '& .fi-heading.heading-with-tooltip': {
    display: 'inline',
    verticalAlign: 'middle',
  },
}));
