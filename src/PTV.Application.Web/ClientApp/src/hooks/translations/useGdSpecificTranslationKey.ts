import { Control, useWatch } from 'react-hook-form';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';

export function useGdSpecificTranslationKey(control: Control<ServiceModel>, gdNotSelectedKey: string, gdSelectedKey: string): string {
  const gd = useWatch({ control: control, name: `${cService.generalDescription}` });
  return gd ? gdSelectedKey : gdNotSelectedKey;
}
