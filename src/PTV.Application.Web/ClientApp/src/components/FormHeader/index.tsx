import React, { useState } from 'react';
import { KeyboardBackspace } from '@mui/icons-material';
import { Heading } from 'suomifi-ui-components';
import { FormHeaderProps } from 'types';
import { Language } from 'types/enumTypes';
import { getFirstTranslationKey } from 'utils/languages';

export default function FormHeader(props: FormHeaderProps): React.ReactElement {
  const firstUsedLanguage = getFirstTranslationKey(props.names) || 'fi';
  const [selectedLanguage] = useState<Language>(firstUsedLanguage);

  return (
    <div>
      <div>
        <KeyboardBackspace />
        <span>Back</span>
      </div>
      <Heading variant='h1hero'>{(selectedLanguage && props.names[selectedLanguage]) || 'Add service'}</Heading>
    </div>
  );
}
