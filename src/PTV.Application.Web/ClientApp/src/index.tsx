import React, { Suspense } from 'react';
import { CookiesProvider } from 'react-cookie';
import { render } from 'react-dom';
import { QueryClientProvider } from 'react-query';
import { ReactQueryDevtools } from 'react-query/devtools';
import { BrowserRouter } from 'react-router-dom';
import { styled } from '@mui/material/styles';
import 'suomifi-ui-components/dist/main.css';
import AppSelector from 'components/AppSelector';
import LoadingIndicator from 'components/LoadingIndicator';
import { AppContextProvider } from 'context/AppContextProvider';
import { createQueryClient } from 'utils/reactquery';
import './custom.css';
import './i18';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href') || undefined;
const rootElement = document.getElementById('root');
const queryClient = createQueryClient();

const Centered = styled('div')({
  margin: 'auto',
});

render(
  <QueryClientProvider client={queryClient}>
    <BrowserRouter basename={baseUrl}>
      <CookiesProvider>
        <React.StrictMode>
          <Suspense
            fallback={
              <Centered>
                <LoadingIndicator />
              </Centered>
            }
          >
            <AppContextProvider>
              <AppSelector />
            </AppContextProvider>
          </Suspense>
        </React.StrictMode>
      </CookiesProvider>
    </BrowserRouter>
    <ReactQueryDevtools initialIsOpen={false} />
  </QueryClientProvider>,
  rootElement
);
