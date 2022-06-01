import React, { HTMLProps, ReactElement, ReactNode, Ref, forwardRef } from 'react';
import { css, styled } from '@mui/material/styles';
import clsx from 'clsx';
import IconButton from 'components/IconButton';

type TooltipContentProps = HTMLProps<HTMLDivElement> & {
  onCloseButtonClick: () => void;
  arrowOffsetPx: number;
  closeButtonLabelText: string;
  children: ReactNode;
};

const UnstyledTooltipContent = forwardRef(
  (
    { children, arrowOffsetPx, onCloseButtonClick, className, closeButtonLabelText, ...rest }: TooltipContentProps,
    ref: Ref<HTMLDivElement>
  ): ReactElement => {
    return (
      <div {...rest} ref={ref} className={clsx(className, 'tooltip-wrapper')}>
        {children}
        <IconButton aria-label={closeButtonLabelText} className='close-icon-button' onClick={onCloseButtonClick} icon='close' />
      </div>
    );
  }
);

UnstyledTooltipContent.displayName = 'UnstyledTooltipContent';

export const TooltipContent = styled(UnstyledTooltipContent)(({ theme, arrowOffsetPx = 0 }) => ({
  position: 'relative',
  border: `1px solid ${theme.suomifi.colors.depthDark2}`,
  borderRadius: theme.suomifi.radius.basic,
  backgroundColor: theme.suomifi.colors.highlightLight4,
  padding: '20px 43px 20px 20px',
  boxShadow: theme.suomifi.shadows.menuShadow,
  marginTop: '10px',
  marginBottom: '10px',
  '&:before, &:after': {
    content: "''",
    position: 'absolute',
    height: 0,
    width: 0,
    bottom: '100%',
    left: `${arrowOffsetPx}px`,
    border: 'solid transparent',
    pointerEvents: 'none',
  },
  '&:before': {
    borderBottomColor: theme.suomifi.colors.depthDark2,
    borderWidth: '8px',
    marginRight: '-8px',
  },
  '&:after': {
    borderBottomColor: theme.suomifi.colors.highlightLight4,
    borderWidth: '6.5px',
    marginRight: '-6.5px',
  },
  '& .close-icon-button': {
    position: 'absolute',
    top: '20px',
    right: '15px',
    height: '18px',
    width: '18px',
  },
  '& .fi-heading': {
    marginBottom: '10px',
    marginTop: 0,
  },
  '&.tooltip-wrapper': css`
    ${theme.suomifi.typography.bodyTextSmall},
    & .fi-text {
      ${theme.suomifi.typography.bodyTextSmall}
    }
  `,
}));
