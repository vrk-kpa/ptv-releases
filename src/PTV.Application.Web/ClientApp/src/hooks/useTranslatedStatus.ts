import { useTranslation } from 'react-i18next';

type ErrorObject = {
  key: string;
  [property: string]: string | number;
};

type MetaObject = {
  error?: ErrorObject | string | undefined;
  touched: boolean;
};

type TranslatedStatusResult = {
  status: 'default' | 'error';
  statusText: string | undefined;
};

export const useTranslatedStatus = (meta: MetaObject): TranslatedStatusResult => {
  const { error, touched } = meta;
  const status = error && touched ? 'error' : 'default';
  const statusText = status === 'error' ? error : undefined;
  const { t } = useTranslation();

  let translatedStatusText;
  if (typeof statusText === 'object') {
    const { key, ...rest } = statusText;
    translatedStatusText = t(key, rest);
  } else if (typeof statusText === 'string') {
    translatedStatusText = t(statusText);
  } else {
    translatedStatusText = statusText;
  }

  return {
    status,
    statusText: translatedStatusText,
  };
};
