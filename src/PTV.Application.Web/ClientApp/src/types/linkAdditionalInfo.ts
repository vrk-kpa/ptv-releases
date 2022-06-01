export type LinkAdditionalInfo = {
  name: string | null | undefined;
  url: string | null | undefined;
  additionalInformation: string | null | undefined;
  orderNumber: number | null | undefined;
};

export enum cLinkAdditionalInfo {
  name = 'name',
  url = 'url',
  additionalInformation = 'additionalInformation',
}

export const CreateLinkAdditionalInfo = (): LinkAdditionalInfo => ({
  name: '',
  url: '',
  additionalInformation: '',
  orderNumber: null,
});
