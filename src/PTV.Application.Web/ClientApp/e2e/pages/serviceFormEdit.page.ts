import { Locator, Page } from '@playwright/test';

export class ServiceFormEditPage {
  readonly page: Page;

  readonly serviceName: Locator;
  readonly editButton: Locator;
  readonly addLawButton: Locator;
  readonly publishButton: Locator;
  readonly serviceClassesOpenButton: Locator;
  readonly ontologyTermsOpenButton: Locator;
  readonly industrialClassesOpenButton: Locator;
  readonly addVoucherButton: Locator;
  readonly saveButton: Locator;
  readonly addLanguageButton: Locator;

  readonly inputName: Locator;
  readonly serviceType: Locator;
  readonly summary: Locator;

  readonly description: Locator;
  readonly userInstructions: Locator;
  readonly conditions: Locator;
  readonly chargeInformation: Locator;

  readonly areaInformation: Locator;
  readonly areaInformationMunicipalities: Locator;
  readonly areaInformationProvinces: Locator;
  readonly areaInformationBusinessRegions: Locator;
  readonly areaInformationHospitalRegions: Locator;
  readonly lifeEvents: Locator;
  readonly responsibleOrganization: Locator;
  readonly otherResponsibleOrganization: Locator;
  readonly purchaseProducers: Locator;

  readonly selectLanguages: Locator;
  readonly chargeType: Locator;
  readonly fundingType: Locator;
  readonly hasSelfProducers: Locator;
  readonly voucherType: Locator;

  readonly targetGroups: Locator;

  selectedLanguage = 'fi';

  constructor(page: Page, selectedLanguage?: string) {
    if (selectedLanguage) {
      this.selectedLanguage = selectedLanguage;
    }
    this.page = page;

    this.serviceName = page.locator('#serviceName');

    // Buttons
    this.editButton = page.locator('#service-form-button-edit');
    this.publishButton = page.locator('#service-form-button-publish');
    this.addLawButton = page.locator(`#add\\.law\\.${this.selectedLanguage}`);
    this.serviceClassesOpenButton = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.serviceClasses\\.open`);
    this.ontologyTermsOpenButton = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.ontologyterms\\.open`);
    this.industrialClassesOpenButton = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.industrialClasses\\.open`);
    this.addVoucherButton = page.locator(`#add\\.voucher\\.${this.selectedLanguage}`);
    this.saveButton = page.locator(`#service-form-button-save`);
    this.addLanguageButton = page.locator('#add-languageversion-button');

    // Input fields
    this.inputName = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.name`);
    this.summary = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.summary`);

    // Rich text editors
    this.description = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.description`);
    this.userInstructions = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.userInstructions`);
    this.conditions = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.conditions`);
    this.chargeInformation = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.charge`);

    // Select dropdowns
    this.selectLanguages = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.languages`);
    this.areaInformationMunicipalities = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.areaInformation\\.municipalities`);
    this.areaInformationProvinces = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.areaInformation\\.provinces`);
    this.areaInformationBusinessRegions = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.areaInformation\\.businessRegions`);
    this.areaInformationHospitalRegions = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.areaInformation.\\hospitalRegions`);
    this.lifeEvents = page.locator(`#lifeEvents`);
    this.responsibleOrganization = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.responsible-organization`);
    this.otherResponsibleOrganization = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.otherResponsibleOrganizations`);
    this.purchaseProducers = page.locator(`#purchaseProducers`);

    // Radiobuttons
    this.serviceType = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.serviceType`);
    this.areaInformation = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.areaInformation\\.areaInformationType`);
    this.chargeType = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.chargeType`);
    this.fundingType = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.fundingType`);
    this.hasSelfProducers = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.hasSelfProducers`);
    this.voucherType = page.locator(`#languageVersions\\.${this.selectedLanguage}\\.voucherType`);

    // Checkboxes
    // TODO: TargetGroups contains multiple targetgroup fieldsets that contain the checkboxes
    this.targetGroups = page.locator(`fieldset[id=languageVersions\\.${this.selectedLanguage}\\.targetGroups]`);
  }

  async setLanguage(language: string) {
    this.selectedLanguage = language;
  }

  async gotoNewService() {
    await this.page.goto('/service');
  }

  async gotoId(id: string) {
    await this.page.goto(`/service/${id}`);
  }

  async clickHeaderSaveDraft() {
    await this.saveButton.first().click();
  }

  async clickFooterSaveDraft() {
    await this.saveButton.last().click();
  }

  async clickHeaderPublish() {
    await this.publishButton.first().click();
  }

  async clickFooterPublish() {
    await this.saveButton.last().click();
  }
}
