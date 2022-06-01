import React from 'react';
import { Button } from 'suomifi-ui-components';
import { copyToClipboard } from 'utils';
import LinkButton from './LinkButton';

type CopyTextButtonProps = {
  text: string;
  label: string;
  asLink?: boolean;
  className?: string | undefined;
};

export default function CopyTextButton(props: CopyTextButtonProps): React.ReactElement {
  const handleClick = () => copyToClipboard(props.text);

  if (props.asLink) {
    return <LinkButton onClick={handleClick} label={props.label} className={props.className} />;
  }

  return (
    <Button variant='secondary' onClick={handleClick} id='button-copy-text' className={props.className}>
      {props.label}
    </Button>
  );
}
