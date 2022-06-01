export type NotificationStatuses = {
  languageVersionTabsErrorsVisible: boolean;
  detailedTabErrorsVisible: boolean;
  missingOrganizationErrorsVisible: boolean;
  translationVisible: boolean;
};

export const initialNotificationStatuses: NotificationStatuses = {
  languageVersionTabsErrorsVisible: true,
  detailedTabErrorsVisible: true,
  missingOrganizationErrorsVisible: true,
  translationVisible: true,
};
