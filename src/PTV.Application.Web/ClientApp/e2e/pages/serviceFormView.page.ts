import { Locator, Page } from '@playwright/test';

export class ServiceFormViewPage {
  readonly page: Page;

  // Expanders
  readonly classificationAndKeywordsExpander: Locator;

  readonly languageVersionFiStatusText: Locator;
  readonly languageVersionSvStatusText: Locator;
  readonly languageVersionEnStatusText: Locator;
  readonly languageVersionSeStatusText: Locator;
  readonly languageVersionSmnStatusText: Locator;
  readonly languageVersionSmsStatusText: Locator;

  readonly ontologyTermsHeader: Locator;
  readonly keywordsHeader: Locator;

  readonly serviceNameHeader: Locator;

  selectedLanguage = 'fi';

  constructor(page: Page, selectedLanguage?: string) {
    if (selectedLanguage) {
      this.selectedLanguage = selectedLanguage;
    }
    this.page = page;

    this.serviceNameHeader = page.locator('#serviceName').first();

    this.languageVersionFiStatusText = page.locator('#languageVersion-fi-status');
    this.languageVersionSvStatusText = page.locator('#languageVersion-sv-status');
    this.languageVersionEnStatusText = page.locator('#languageVersion-en-status');
    this.languageVersionSeStatusText = page.locator('#languageVersion-se-status');
    this.languageVersionSmnStatusText = page.locator('#languageVersion-smn-status');
    this.languageVersionSmsStatusText = page.locator('#languageVersion-sms-status');

    // Expanders
    this.classificationAndKeywordsExpander = page.locator('button:has-text("Classification and keywords")');

    this.ontologyTermsHeader = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.ontologyTerms`);
    this.keywordsHeader = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.keywords`);
  }
}
