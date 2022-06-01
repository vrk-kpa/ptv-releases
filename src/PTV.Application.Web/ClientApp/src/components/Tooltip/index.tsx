import React, { ReactElement, ReactNode, useLayoutEffect, useRef, useState } from 'react';
import { styled } from '@mui/material/styles';
import clsx from 'clsx';
import IconButton from 'components/IconButton';
import { TooltipContent } from './TooltipContent';

type TooltipProps = {
  children: ReactNode;
  ariaInfoButtonLabelText: string;
  ariaCloseButtonLabelText: string;
  className?: string;
  contentClassName?: string | undefined;
  anchorElement?: HTMLElement | null;
};

export const Tooltip = styled(
  ({
    children,
    className,
    anchorElement,
    contentClassName,
    ariaInfoButtonLabelText,
    ariaCloseButtonLabelText,
  }: TooltipProps): ReactElement => {
    const [open, setOpen] = useState(false);
    const buttonRef = useRef<HTMLButtonElement>(null);
    const contentRef = useRef<HTMLDivElement>(null);
    const [contentArrowOffsetPx, setContentArrowOffsetPx] = useState(0);
    const [anchorRefObserver, setAnchorRefObserver] = useState<ResizeObserver | null>(null);

    useLayoutEffect(() => {
      function calculateContentArrowOffset() {
        if (!!buttonRef && buttonRef.current && !!contentRef && !!contentRef.current) {
          return buttonRef.current.getBoundingClientRect().left - contentRef.current.getBoundingClientRect().left;
        }
        return 0;
      }

      function setContentArrowOffset() {
        setContentArrowOffsetPx(calculateContentArrowOffset());
      }

      window.addEventListener('resize', setContentArrowOffset);

      setAnchorRefObserver(
        new ResizeObserver(() => {
          setContentArrowOffset();
        })
      );

      setContentArrowOffset();
      return () => {
        window.removeEventListener('resize', setContentArrowOffset);
      };
    }, []);

    useLayoutEffect(() => {
      if (!!anchorElement && !!anchorRefObserver) {
        anchorRefObserver.observe(anchorElement);
      }
      return () => {
        if (!!anchorRefObserver) {
          anchorRefObserver.disconnect();
        }
      };
    }, [anchorElement, anchorRefObserver]);

    return (
      <>
        <IconButton
          ref={buttonRef}
          icon='infoFilled'
          className={clsx(className, 'tooltip-button')}
          iconClassName='tooltip-button-icon'
          onClick={(event) => {
            event.preventDefault();
            setOpen(!open);
          }}
          aria-expanded={open}
          aria-label={ariaInfoButtonLabelText}
        />
        {open && (
          <TooltipContent
            ref={contentRef}
            arrowOffsetPx={contentArrowOffsetPx}
            onCloseButtonClick={() => {
              if (!!buttonRef) {
                buttonRef.current?.focus();
              }
              setOpen(!open);
            }}
            closeButtonLabelText={ariaCloseButtonLabelText}
            className={contentClassName}
          >
            {children}
          </TooltipContent>
        )}
      </>
    );
  }
)(({ theme }) => {
  return {
    '&.tooltip-button': {
      display: 'inline',
      height: '16px',
      width: '16px',
      marginLeft: '5px',
      verticalAlign: 'middle',
      '& .tooltip-button-icon': {
        color: theme.suomifi.colors.highlightBase,
      },
    },
  };
});
