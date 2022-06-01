import React, { useState } from 'react';
import { Control } from 'react-hook-form';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { PublishingStatus } from 'types/enumTypes';
import { ServiceFormValues, ServiceModel } from 'types/forms/serviceFormTypes';
import { PublishDialog } from 'features/service/components/PublishDialog';
import { PublishButton } from './PublishButton';

type ServiceFormPublishFunctionsProps = {
  control: Control<ServiceModel>;
  hasOtherModifiedVersion: boolean;
  status: PublishingStatus;
  responsibleOrgId: string | null | undefined;
  updateService: (service: ServiceFormValues) => void;
  getFormValues: () => ServiceFormValues;
  publishSucceeded: (data: ServiceApiModel) => void;
  saveSucceededPublishFailed: (data: ServiceApiModel) => void;
  alignItemsLeft: boolean;
};

export function ServiceFormPublishFunctions(props: ServiceFormPublishFunctionsProps): React.ReactElement {
  const [isPublishModalOpen, setIsPublishModalOpen] = useState<boolean>(false);
  return (
    <>
      <PublishButton
        control={props.control}
        hasOtherModifiedVersion={props.hasOtherModifiedVersion}
        responsibleOrgId={props.responsibleOrgId}
        status={props.status}
        onOpenPublishDialog={() => setIsPublishModalOpen(true)}
        alignItemsLeft={props.alignItemsLeft}
      />
      <PublishDialog
        isOpen={isPublishModalOpen}
        close={() => setIsPublishModalOpen(false)}
        control={props.control}
        updateService={props.updateService}
        getFormValues={props.getFormValues}
        publishSucceeded={props.publishSucceeded}
        saveSucceededPublishFailed={props.saveSucceededPublishFailed}
      />
    </>
  );
}
