import { Language } from 'types/enumTypes';
import { ServiceFormContext } from 'types/forms/serviceFormTypes';
import { IAppContext } from 'context/AppContextProvider';

export function createServiceFormContext(appContext: IAppContext, uiLanguage: Language): ServiceFormContext {
  return {
    serviceClasses: appContext.staticData.serviceClasses,
    uiLanguage: uiLanguage,
  };
}
