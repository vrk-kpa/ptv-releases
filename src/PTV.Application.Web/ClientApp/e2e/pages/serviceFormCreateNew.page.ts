import { Locator, Page } from '@playwright/test';

export class ServiceFormCreateNewPage {
  readonly page: Page;

  readonly languageVersionFiCheckbox: Locator;
  readonly languageVersionSvCheckbox: Locator;
  readonly languageVersionEnCheckbox: Locator;
  readonly languageVersionSeCheckbox: Locator;
  readonly languageVersionSmnCheckbox: Locator;
  readonly languageVersionSmsCheckbox: Locator;

  readonly languageVersionFiInput: Locator;
  readonly languageVersionSvInput: Locator;
  readonly languageVersionEnInput: Locator;
  readonly languageVersionSeInput: Locator;
  readonly languageVersionSmnInput: Locator;
  readonly languageVersionSmsInput: Locator;

  readonly confimButton: Locator;
  readonly cancelButton: Locator;

  constructor(page: Page) {
    this.page = page;

    this.languageVersionFiCheckbox = page.locator(`#languageVersionCheckbox\\.fi`);
    this.languageVersionSvCheckbox = page.locator(`#languageVersionCheckbox\\.sv`);
    this.languageVersionEnCheckbox = page.locator(`#languageVersionCheckbox\\.en`);
    this.languageVersionSeCheckbox = page.locator(`#languageVersionCheckbox\\.se`);
    this.languageVersionSmnCheckbox = page.locator(`#languageVersionCheckbox\\.smn`);
    this.languageVersionSmsCheckbox = page.locator(`#languageVersionCheckbox\\.sms`);

    this.languageVersionFiInput = page.locator(`#input-fi`);
    this.languageVersionSvInput = page.locator(`#input-sv`);
    this.languageVersionEnInput = page.locator(`#input-en`);
    this.languageVersionSeInput = page.locator(`#input-se`);
    this.languageVersionSmnInput = page.locator(`#input-smn`);
    this.languageVersionSmsInput = page.locator(`#input-sms`);

    this.confimButton = page.locator(`#add\\.language\\.versions\\.confirm`);
    this.cancelButton = page.locator(`#add\\.language\\.versions\\.cancel`);
  }

  async goto() {
    await this.page.goto('/service');
  }
}
