/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
import { schema } from 'normalizr'
import { adminTaskTypesEnum } from 'enums'
import { defineMessages } from 'util/react-intl'

export const messages = defineMessages({
  AccessibilityRegisterJob: {
    id: 'Routes.AdminPage.TaskScheduler.AccessibilityRegisterJob.Title',
    defaultMessage: 'Accessibility Register Job'
  },
  AccessibilityRegisterJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.AccessibilityRegisterJob.Description',
    defaultMessage: 'Task for downloading and importing of accessibility register info.'
  },
  ArchiveEntitiesByExpirationDateJob: {
    id: 'Routes.AdminPage.TaskScheduler.ArchiveEntitiesByExpirationDateJob.Title',
    defaultMessage: 'Archive Entities By Expiration Date Job'
  },
  ArchiveEntitiesByExpirationDateJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.ArchiveEntitiesByExpirationDateJob.Description',
    defaultMessage: 'Task for archiving entities after expiration date.'
  },
  BusinessRegionCodesJob: {
    id: 'Routes.AdminPage.TaskScheduler.BusinessRegionCodesJob.Title',
    defaultMessage: 'Business Region Codes Job'
  },
  BusinessRegionCodesJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.BusinessRegionCodesJob.Description',
    defaultMessage: 'Task for updating of business region codes.'
  },
  ClearNotificationsJob: {
    id: 'Routes.AdminPage.TaskScheduler.ClearNotificationsJob.Title',
    defaultMessage: 'Clear Notifications Job'
  },
  ClearNotificationsJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.ClearNotificationsJob.Description',
    defaultMessage: 'Task for removing notification after expiration date.'
  },
  DigitalAuthorizationCodesJob: {
    id: 'Routes.AdminPage.TaskScheduler.DigitalAuthorizationCodesJob.Title',
    defaultMessage: 'Digital Authorization Codes Job'
  },
  DigitalAuthorizationCodesJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.DigitalAuthorizationCodesJob.Description',
    defaultMessage: 'Task for updating of digital authorizations.'
  },
  EmailNotifyJob: {
    id: 'Routes.AdminPage.TaskScheduler.EmailNotifyJob.Title',
    defaultMessage: 'Email Notify Job'
  },
  EmailNotifyJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.EmailNotifyJob.Description',
    defaultMessage: 'Task for generating notification and task number for all main organizations.'
  },
  GeoServerJob: {
    id: 'Routes.AdminPage.TaskScheduler.GeoServerJob.Title',
    defaultMessage: 'Geo Server Job'
  },
  GeoServerJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.GeoServerJob.Description',
    defaultMessage: 'Task for updating of database for GeoServer.'
  },
  HospitalRegionCodesJob: {
    id: 'Routes.AdminPage.TaskScheduler.HospitalRegionCodesJob.Title',
    defaultMessage: 'Hospital Region Codes Job'
  },
  HospitalRegionCodesJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.HospitalRegionCodesJob.Description',
    defaultMessage: 'Task for updating of hospital region codes.'
  },
  IndustrialClassesFintoJob: {
    id: 'Routes.AdminPage.TaskScheduler.IndustrialClassesFintoJob.Title',
    defaultMessage: 'Industrial Classes Finto Job'
  },
  IndustrialClassesFintoJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.IndustrialClassesFintoJob.Description',
    defaultMessage: 'Task for updating finto data - industrial classes.'
  },
  LanguageCodesJob: {
    id: 'Routes.AdminPage.TaskScheduler.LanguageCodesJob.Title',
    defaultMessage: 'Language Codes Job'
  },
  LanguageCodesJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.LanguageCodesJob.Description',
    defaultMessage: 'Task for updating of language codes.'
  },
  LifeEventsFintoJob: {
    id: 'Routes.AdminPage.TaskScheduler.LifeEventsFintoJob.Title',
    defaultMessage: 'Life Events Finto Job'
  },
  LifeEventsFintoJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.LifeEventsFintoJob.Description',
    defaultMessage: 'Task for updating of finto data - life events.'
  },
  LocalizationJob: {
    id: 'Routes.AdminPage.TaskScheduler.LocalizationJob.Title',
    defaultMessage: 'Localization Job'
  },
  LocalizationJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.LocalizationJob.Description',
    defaultMessage: 'Task for downloading localization messages from transifex service.'
  },
  MassJob: {
    id: 'Routes.AdminPage.TaskScheduler.MassJob.Title',
    defaultMessage: 'Mass Job'
  },
  MassJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.MassJob.Description',
    defaultMessage: 'Task for scheduled publishing and archiving.'
  },
  MunicipalityCodesJob: {
    id: 'Routes.AdminPage.TaskScheduler.MunicipalityCodesJob.Title',
    defaultMessage: 'Municipality Codes Job'
  },
  MunicipalityCodesJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.MunicipalityCodesJob.Description',
    defaultMessage: 'Task for updating of municipality codes.'
  },
  OntologyTermsFintoJob: {
    id: 'Routes.AdminPage.TaskScheduler.OntologyTermsFintoJob.Title',
    defaultMessage: 'Ontology Terms Finto Job'
  },
  OntologyTermsFintoJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.OntologyTermsFintoJob.Description',
    defaultMessage: 'Task for updating of finto data - ontology terms.'
  },
  PermanentDeleteJob: {
    id: 'Routes.AdminPage.TaskScheduler.PermanentDeleteJob.Title',
    defaultMessage: 'Permanent Delete Job'
  },
  PermanentDeleteJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.PermanentDeleteJob.Description',
    defaultMessage: 'Task for physically removing content after specific time.'
  },
  PostalCodeCoordinatesJob: {
    id: 'Routes.AdminPage.TaskScheduler.PostalCodeCoordinatesJob.Title',
    defaultMessage: 'Postal Code Coordinates Job'
  },
  PostalCodeCoordinatesJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.PostalCodeCoordinatesJob.Description',
    defaultMessage: 'Download coordinates from geo.stat.fi'
  },
  PostalCodesJob: {
    id: 'Routes.AdminPage.TaskScheduler.PostalCodesJob.Title',
    defaultMessage: 'Postal Codes Job'
  },
  PostalCodesJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.PostalCodesJob.Description',
    defaultMessage: 'Task for updating of postal codes.'
  },
  ProvinceCodesJob: {
    id: 'Routes.AdminPage.TaskScheduler.ProvinceCodesJob.Title',
    defaultMessage: 'Province Codes Job'
  },
  ProvinceCodesJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.ProvinceCodesJob.Description',
    defaultMessage: 'Task for updating of province (region) codes.'
  },
  ServiceClassesFintoJob: {
    id: 'Routes.AdminPage.TaskScheduler.ServiceClassesFintoJob.Title',
    defaultMessage: 'Service Classes Finto Job'
  },
  ServiceClassesFintoJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.ServiceClassesFintoJob.Description',
    defaultMessage: 'Task for updating of finto data - service classes.'
  },
  StreetDataJob: {
    id: 'Routes.AdminPage.TaskScheduler.StreetDataJob.Title',
    defaultMessage: 'Street Data Job'
  },
  StreetDataJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.StreetDataJob.Description',
    defaultMessage: 'Downloads geodata from CLS API.'
  },
  TargetGroupsFintoJob: {
    id: 'Routes.AdminPage.TaskScheduler.TargetGroupsFintoJob.Title',
    defaultMessage: 'Target Groups Finto Job'
  },
  TargetGroupsFintoJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.TargetGroupsFintoJob.Description',
    defaultMessage: 'Task for updating of finto data - target groups.'
  },
  TranslationOrderHandlingJob: {
    id: 'Routes.AdminPage.TaskScheduler.TranslationOrderHandlingJob.Title',
    defaultMessage: 'Translation Order Handling Job'
  },
  TranslationOrderHandlingJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.TranslationOrderHandlingJob.Description',
    defaultMessage: 'Task for handling orders to repetition and canceling.'
  },
  TranslationOrderProcessingDataByStateJob: {
    id: 'Routes.AdminPage.TaskScheduler.TranslationOrderProcessingDataByStateJob.Title',
    defaultMessage: 'Translation Order Processing Data By State Job'
  },
  TranslationOrderProcessingDataByStateJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.TranslationOrderProcessingDataByStateJob.Description',
    defaultMessage: 'Task for updating states of orders.'
  },
  TranslationOrderSendAgainJob: {
    id: 'Routes.AdminPage.TaskScheduler.TranslationOrderSendAgainJob.Title',
    defaultMessage: 'Translation Order Send Again Job'
  },
  TranslationOrderSendAgainJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.TranslationOrderSendAgainJob.Description',
    defaultMessage: 'Task for sending translation order with error again.'
  },
  TranslationOrderSendNewJob: {
    id: 'Routes.AdminPage.TaskScheduler.TranslationOrderSendNewJob.Title',
    defaultMessage: 'Translation Order Send New Job'
  },
  TranslationOrderSendNewJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.TranslationOrderSendNewJob.Description',
    defaultMessage: 'Task for sending new orders to translation company.'
  },
  PostiStreetLoaderJob: {
    id: 'Routes.AdminPage.TaskScheduler.PostiStreetLoaderJob.Title',
    defaultMessage: 'Posti Street Loader Job'
  },
  PostiStreetLoaderJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.PostiStreetLoaderJob.Description',
    defaultMessage: 'Downloads street data from Posti system.'
  },
  OldArchivedAncientJob: {
    id: 'Routes.AdminPage.TaskScheduler.OldArchivedAncientJob.Title',
    defaultMessage: 'Old Archived Ancient Job'
  },
  OldArchivedAncientJobDescription: {
    id: 'Routes.AdminPage.TaskScheduler.OldArchivedAncientJob.Description',
    defaultMessage: 'Task for marking old archived content.'
  }
})

const adminTaskPage = new schema.Entity('adminTasks')

const adminTaskPageScheduler = new schema.Entity('adminTasks', {},
  {
    idAttribute: () => adminTaskTypesEnum.SCHEDULEDTASKS,
    processStrategy: (value, parent, key) => ({
      [value.code]: { ...value, name: messages[value.code], description: messages[value.code + 'Description'] }
    })
  })
const schedulerJobsSchema = new schema.Object({
  jobs: new schema.Array(adminTaskPageScheduler)
})

const getEntities = searchSchema => new schema.Entity('adminTasks', {
  entities: new schema.Array(searchSchema)
})

export const AdminTasksSchemas = {
  ADMIN_TASK_PAGE: adminTaskPage,
  ADMIN_TASK_PAGE_ARRAY: new schema.Array(adminTaskPage),
  SCHEDULER_JOBS: schedulerJobsSchema,
  GET_ENTITIES: getEntities
}
