import { createInstance } from '@jonkoops/matomo-tracker-react';
import { AppEnvironment } from 'types/enumTypes';

type MatomoEnvSettings = {
  env: AppEnvironment;
  siteId: number;
};

const enviromentSettings: MatomoEnvSettings[] = [
  { env: 'Dev', siteId: 28 },
  { env: 'Test', siteId: 30 },
  { env: 'Qa', siteId: 31 },
  { env: 'Trn', siteId: 32 },
  { env: 'Prod', siteId: 29 },
];

const isProductionNodeEnv = (): boolean => process.env.NODE_ENV === 'production';

const matomoBaseUrl = 'https://suomi.matomo.cloud';

const createPtvMatomoInstance = (appEnv: AppEnvironment) => {
  const envSettings = enviromentSettings.find((x) => x.env === appEnv);

  // Disabled when running localy via npm run
  const matomoDisabled = !isProductionNodeEnv();

  if (!envSettings) {
    return createInstance({
      urlBase: matomoBaseUrl,
      // SiteId is required even if matomo is disabled. PTV Dev site id is 28
      siteId: 28,
      disabled: true,
    });
  }

  return createInstance({
    urlBase: matomoBaseUrl,
    siteId: envSettings.siteId,
    disabled: matomoDisabled,
    heartBeat: {
      active: true,
      seconds: 15,
    },
    linkTracking: false,
  });
};

export { createPtvMatomoInstance };
