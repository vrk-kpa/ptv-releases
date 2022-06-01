import React from 'react';
import { useTranslation } from 'react-i18next';
import { Routes as ReactRouterRoutes, Route } from 'react-router';
import { ChangePageTitle } from 'components/ChangePageTitle';
import RedirectToPath from 'components/RedirectToPath';
import { FormMetaContextProvider, createInitialState } from 'context/formMeta';
import { QualityAgentContextProvider, createInitialState as createInitialQualityAgentState } from 'context/qualityAgent';
import { ServiceContextProvider } from 'features/service/ServiceContextProvider';
import ServiceContainer from 'features/service/containers';

export default function Routes(): React.ReactElement {
  const { t } = useTranslation();

  return (
    <ReactRouterRoutes>
      <Route
        path='/service/*'
        element={
          <ServiceContextProvider>
            <FormMetaContextProvider initialState={createInitialState()}>
              <QualityAgentContextProvider initialState={createInitialQualityAgentState()}>
                <ChangePageTitle pageTitle={t('Ptv.Router.ServiceForm.Title')} />
                <ServiceContainer />
              </QualityAgentContextProvider>
            </FormMetaContextProvider>
          </ServiceContextProvider>
        }
      />

      {/* Routes unknown to us redirect to old app search. Redirecting to root
      causes infinite loop. Maybe related to two react apps + server side routing? */}
      <Route path='*' element={<RedirectToPath path='/frontpage/search' />} />
    </ReactRouterRoutes>
  );
}
