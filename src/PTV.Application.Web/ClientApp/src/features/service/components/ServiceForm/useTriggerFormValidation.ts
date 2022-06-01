import { useEffect } from 'react';
import { UseFormTrigger } from 'react-hook-form';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { useFormMetaContext } from 'context/formMeta';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';

type UseTriggerValidationProps = {
  trigger: UseFormTrigger<ServiceModel>;
};

export function useTriggerFormValidation(props: UseTriggerValidationProps): void {
  const uiLang = useGetUiLanguage();
  const { mode } = useFormMetaContext();
  const { trigger } = props;

  useEffect(() => {
    // RHF does not trigger validation on first render so we need to trigger it manually.
    // Otherwise when you create new service the "save as draft" button is disabled but
    // you don't see the error on the name field.
    //
    // If the UI language changes we need to trigger validation so that validation
    // errors are shown using correct language. This is because translation happens in the
    // validation function (RHF does not currently support passing translation info to field state)

    // If the form mode changes we need to trigger validation. This is because it is possible
    // to create invalid service from the old ui/open api. Without trigger() when user switched
    // to edit mode the save as draft button is disabled but user does not see the error message
    trigger();
  }, [trigger, uiLang, mode]);
}
