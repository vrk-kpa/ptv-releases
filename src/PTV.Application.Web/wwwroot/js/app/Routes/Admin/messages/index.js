/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { defineMessages } from 'util/react-intl'

export const messages = defineMessages({
  pageTitle: {
    id: 'AdminPage.Title',
    defaultMessage: 'Nämä tehtävät nousevat automaattisesti PTV-järjestelmästä.'
  },
  pageTooltip: {
    id: 'AdminPage.Tooltip',
    defaultMessage: 'Nämä tehtävät nousevat automaattisesti PTV-järjestelmästä.'
  },
  adminPageTabName: {
    id: 'AdminPage.AdminTasksTab.Title',
    defaultMessage: 'Tehtävät',
    description: 'Routes.Tasks.Main.Tab.Name'
  },
  sahaPtvTabName: {
    id: 'AdminPage.SAHAPTVMAppingTab.Title',
    defaultMessage: 'SAHA/PTV Mapping'
  },
  adminTranslationOrdersSection: {
    id: 'AdminTasks.TranslationOrderSection.Title',
    defaultMessage: 'Käännöstilauksiin liittyvät tehtävät'
  },
  adminLinkValidatorSection: {
    id: 'AdminTasks.LinkValidatorSection.Title',
    defaultMessage: 'Linkkivalidaattoriin liittyvät tehtävät'
  },
  adminScheduledTasksSection: {
    id: 'AdminTasks.ScheduledTasksSection.Title',
    defaultMessage: 'Ajastettuihin ajoihin liittyvät tehtävät'
  },
  failedTranslationOrdersTitle: {
    id: 'AdminTasks.FailedTranslationOrders.Title',
    defaultMessage: 'Epäonnistuneet käännöstilaukset ({count})'
  },
  failedTranslationOrdersTooltip: {
    id: 'AdminTasks.FailedTranslationOrders.Tooltip',
    defaultMessage: 'Failed translation orders tooltip'
  },
  failedTranslationOrdersSectionButtonTitle: {
    id: 'AdminTasks.FailedTranslationOrders.SectionButton.Title',
    defaultMessage: 'Nouda käännökset uudelleen'
  },
  scheduledTasksTitle: {
    id: 'AdminTasks.ScheduledTasks.Title',
    defaultMessage: 'Scheduled tasks'
  },
  scheduledTasksTooltip: {
    id: 'AdminTasks.ScheduledTasks.Tooltip',
    defaultMessage: 'Failed scheduled tasks tooltip'
  },
  searchOrganizationTitle: {
    id: 'AdminMapping.Organization.Title',
    defaultMessage: 'Hae PTV-organisaatio'
  },
  clearButton: {
    id: 'Components.Buttons.ClearButton',
    defaultMessage: 'Tyhjennä'
  },
  searchButton: {
    id: 'Components.Buttons.SearchButton',
    defaultMessage: 'Hae'
  },
  searchCountTitle: {
    id: 'AdminMapping.SearchCount.Title',
    defaultMessage: 'Hakutuloksia'
  },
  emptyMappingMessage: {
    id: 'AdminMapping.EmptyMessage.Title',
    defaultMessage: 'No mapping found for the selected organizations'
  },
  adminMappingHeader: {
    id: 'AdminMapping.PageHeader.Title',
    defaultMessage: 'Voit hakea organisaatio organisaation nimellä tai PTV-tunnisteella.'
  },
  adminMappingOrganizationDialogDescription: {
    id: 'AdminMapping.OrganizationDialog.Description.Title',
    defaultMessage: 'Voit hakea organisaatio organisaation nimellä tai PTV-tunnisteella.'
  },
  forceJobButtonTitle:{
    id: 'ScheduledTasks.ForceButton.Title',
    defaultMessage: 'Force job'
  },
  scheduledTasksSectionButtonTitle: {
    id: 'AdminTasks.ScheduledTasks.SectionButton.Title',
    defaultMessage: 'Refresh'
  },
  failedTOErrorDescription: {
    id: 'AdminTasks.FailedTranslationOrders.ErrorDescription.Title',
    defaultMessage: 'Error description'
  },
  failedTOErrorNotAvailable: {
    id: 'AdminTasks.FailedTranslationOrders.ErrorNotAvailable.Title',
    defaultMessage: 'Not available'
  },
  failedTOExecutionCount: {
    id: 'AdminTasks.FailedTranslationOrders.ExecutionCount.Title',
    defaultMessage: 'Count of former failed executions'
  },
  failedTOShowLog: {
    id: 'AdminTasks.FailedTranslationOrders.ShowLog.Title',
    defaultMessage: 'Show log'
  },
  failedTOCopyErrorFeedback: {
    id: 'AdminTasks.FailedTranslationOrders.CopyErrorFeedback.Title',
    defaultMessage: 'Error message has been copied to clipboard'
  },
  scheduledTasksDownloadButtonTitle: {
    id: 'AdminTasks.ScheduledTasks.DownloadButton.Title',
    defaultMessage: 'Download logs'
  }
})
