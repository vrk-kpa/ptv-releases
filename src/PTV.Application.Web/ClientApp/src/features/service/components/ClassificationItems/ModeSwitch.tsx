import React, { FunctionComponent, useContext } from 'react';
import { Button, ButtonProps } from 'suomifi-ui-components';
import { componentMode } from 'types/enumTypes';
import { DispatchContext } from 'features/service/contexts/ClassificationItems/DispatchContextProvider';
import { toggleMode } from 'features/service/contexts/ClassificationItems/actions';

interface ModeSwitchInterface extends ButtonProps {
  label: string;
  mode: componentMode;
  buttonKey: string;
  id: string;
}

export const ModeSwitch: FunctionComponent<ModeSwitchInterface> = ({
  id,
  label,
  mode,
  buttonKey,
  variant = 'secondary',
  icon,
  iconRight,
}) => {
  const dispatch = useContext(DispatchContext);

  const handleModeSwitch = (mode: componentMode) => {
    toggleMode(dispatch, mode);
  };

  return (
    <Button
      icon={icon ? icon : undefined}
      iconRight={iconRight ? iconRight : undefined}
      key={buttonKey}
      variant={variant}
      onClick={() => handleModeSwitch(mode)}
      id={id}
    >
      {label}
    </Button>
  );
};
