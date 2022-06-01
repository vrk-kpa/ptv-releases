import React, { HTMLProps, ReactElement, forwardRef } from 'react';
import { styled } from '@mui/material/styles';
import { BaseIconKeys, Icon, IconProps } from 'suomifi-ui-components';

type IconButtonProps = Omit<HTMLProps<HTMLButtonElement>, 'type'> & {
  icon: BaseIconKeys;
  iconClassName?: string;
  iconProps?: Omit<IconProps, 'icon' | 'className'>;
};

const UnstyledIconButton = forwardRef((props: IconButtonProps, ref?: React.Ref<HTMLButtonElement>): ReactElement => {
  const { icon, iconClassName, ...rest } = props;
  return (
    <button ref={ref} {...rest}>
      <Icon icon={icon} className={iconClassName} />
    </button>
  );
});

UnstyledIconButton.displayName = 'UnstyledIconButton';

const IconButton = styled(UnstyledIconButton)(({ theme }) => ({
  border: 'none',
  background: 'none',
  padding: 0,
  cursor: 'pointer',

  '&:focus': {
    outline: 'none',
    position: 'relative',
    '&::after': theme.absoluteFocus,
  },
  '& .fi-icon': {
    height: '100%',
    width: '100%',
  },
}));

export default IconButton;
