export type FormStateApiModel = {
  id?: string | null | undefined;
  formName?: string | null | undefined;
  entityType: string | null | undefined;
  entityId: string | null | undefined;
  userName?: string | null | undefined;
  state: string | null | undefined;
  exists?: boolean;
  dataModelVersion: string | null | undefined;
};
