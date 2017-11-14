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
import React from 'react'
import { defineMessages } from 'react-intl'
import {
  PublishingStatusCell,
  ServiceClassesCell,
  ModifiedAtCell,
  ModifiedByCell,
  NameCell,
  OrganizationCell,
  ServiceTypeCell,
  ShortDescriptionCell,
  ColumnHeaderName
} from 'Routes/FrontPage/routes/Search/components'
import './styles.scss'
import {
  LanguageBarCell,
  // NameCell,
  Cell
} from 'appComponents'
import { formTypesEnum, formAllTypes, getKey } from 'enums'
import { PTVIcon } from 'Components'

const messages = defineMessages({
  sharedStatusAndLanguages: {
    id: 'Routes.Search.Util.ColumnsDefinitions.StatusAndLanguages.Title',
    defaultMessage: 'Kieli ja tila'
  },
  sharedPublishingStatus: {
    id: 'FrontPage.Shared.Search.Header.PublishingStatus',
    defaultMessage: 'Tila'
  },
  sharedName: {
    id: 'FrontPage.Shared.Search.Header.Name',
    defaultMessage: 'Nimi'
  },
  sharedEdited: {
    id: 'FrontPage.Shared.Search.Header.Edited',
    defaultMessage: 'Muokattu'
  },
  sharedModified: {
    id: 'FrontPage.Shared.Search.Header.Modifier',
    defaultMessage: 'Muokkaaja'
  },
  sharedContentType: {
    id: 'FrontPage.Shared.Search.Header.ContentType',
    defaultMessage: 'Sisältotyyppi'
  },
  // Organizations messages //
  organizationMainOrganizations: {
    id: 'FrontPage.Organization.Search.Header.MainOrganizations',
    defaultMessage: 'Pääorganisaatio'
  },
  organizationSubOrganizations: {
    id: 'FrontPage.Organization.Search.Header.SubOrganizations',
    defaultMessage: 'Alaorganisaation näyttäminen'
  },
  generalDescriptionsServiceClass: {
    id: 'FrontPage.GeneralDescription.Search.Header.ServiceClass',
    defaultMessage: 'Palveluluokka'
  },
  generalDescriptionsShortDescription: {
    id: 'FrontPage.GeneralDescription.Search.Header.ShortDescription',
    defaultMessage: 'Lyhyt kuvaus'
  },
  // Services messages //
  serviceServiceType: {
    id: 'FrontPage.Services.Search.Header.ServiceType',
    defaultMessage: 'Palvelutyyppi'
  },
  serviceServiceClasss: {
    id: 'FrontPage.Services.Search.Header.ServiceClass',
    defaultMessage: 'Palveluluokka'
  },
  serviceOntologyWords: {
    id: 'FrontPage.Services.Search.Header.OntologyWords',
    defaultMessage: 'Ontologiakäsitteet'
  },
  serviceConnectedChannels: {
    id: 'FrontPage.Services.Search.Header.ConnectedChannels',
    defaultMessage: 'Kanavat'
  },
  serviceOrganization: {
    id: 'FrontPage.Services.Search.Header.Organizations',
    defaultMessage: 'Organisaatio'
  },
  // Channels messages //
  channelFormIdentifier: {
    id: 'FrontPage.Channels.Search.Header.FormIdentifier',
    defaultMessage: 'Lomaketunnus'
  },
  channelConnectedServices: {
    id: 'FrontPage.Channels.Search.Header.ConnectedServices',
    defaultMessage: 'Palvelut'
  },
  channelOrganization: {
    id: 'FrontPage.Channels.Search.Header.Organization',
    defaultMessage: 'Organisaatio'
  }
})

const handleNameClick = (data, handler) => (value) => (
  handler({ rowData: data,
    ...value })
)

const getGeneralDescriptionColumnsDefinition = (formatMessage, onLanguageSelect, searchDomain, onSort, handlePreviewOnClick) => {
  return [
    {
      property: 'languageAvailabilities',
      header: {
        label: formatMessage(messages.sharedStatusAndLanguages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <Cell dWidth='dw60' ldWidth='ldw100'>
            <LanguageBarCell clickable {...rowData} />
          </Cell>
        ]
      }
    },
    {
      property: 'name',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(messages.sharedName)}
                column={column}
                contentType={searchDomain}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (name, { rowData }) => {
            const handleOnClick = () => {
              handlePreviewOnClick(rowData.id, formTypesEnum.GENERALDESCRIPTIONFORM)
            }
            return <Cell dWidth='dw160' ldWidth='ldw220' inline>
              <PTVIcon name='icon-eye' onClick={handleOnClick} />
              <NameCell {...rowData} />
            </Cell>
          }
        ]
      }
    },
    {
      property: 'serviceClasses',
      header: {
        label: formatMessage(messages.generalDescriptionsServiceClass)
      },
      cell: {
        formatters: [
          (serviceClassIds, { rowData }) => <Cell dWidth='dw200' ldWidth='ldw280'>
            <ServiceClassesCell serviceClassIds={serviceClassIds} {...rowData} />
          </Cell>
        ]
      }
    },
    {
      property: 'shortDescription',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(messages.generalDescriptionsShortDescription)}
                column={column}
                contentType={searchDomain}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (shortDescription, { rowData }) => <Cell dWidth='dw260' ldWidth='ldw320'>
            <ShortDescriptionCell {...rowData} />
          </Cell>
        ]
      }
    }
  ]
}
const getOrganizationsColumnsDefinition = (formatMessage, onLanguageSelect, searchDomain, onSort, handlePreviewOnClick) => {
  return [
    {
      property: 'languageAvailabilities',
      header: {
        label: formatMessage(messages.sharedStatusAndLanguages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <Cell dWidth='dw60' ldWidth='ldw100'>
            <LanguageBarCell clickable {...rowData} />
          </Cell>
        ]
      }
    },
    {
      property: 'name',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(messages.sharedName)}
                column={column}
                contentType={searchDomain}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (name, { rowData }) => {
            const handleOnClick = () => {
              handlePreviewOnClick(rowData.id, formTypesEnum.ORGANIZATIONFORM)
            }
            return <Cell dWidth='dw160' ldWidth='ldw220' inline>
              <PTVIcon name='icon-eye' onClick={handleOnClick} />
              <NameCell onLanguageClick={onLanguageSelect} {...rowData} />
            </Cell>
          }
        ]
      }
    },
    {
      property: 'mainOrganization',
      header: {
        label: formatMessage(messages.organizationMainOrganizations)
      },
      cell: {
        // PTV-2274: cannot use OrganizationCell, server returns organization name instead of ID
        formatters: [
          mainOrganization => <Cell dWidth='dw140' ldWidth='ldw180'>
            <div>
              {mainOrganization}
            </div>
          </Cell>
        ]
      }
    },
    {
      property: 'subOrganizations',
      header: {
        label: formatMessage(messages.organizationSubOrganizations)
      },
      cell: {
        // PTV-2274: cannot use OrganizationCell, server returns organization name instead of ID
        formatters: [
          subOrganizations => <Cell dWidth='dw120' ldWidth='ldw160'>
            <div>
              {subOrganizations}
            </div>
          </Cell>
        ]
      }
    },
    {
      property: 'modified',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(messages.sharedEdited)}
                column={column}
                contentType={searchDomain}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          modifiedAt => <Cell dWidth='dw80' ldWidth='ldw100'>
            <ModifiedAtCell modifiedAt={modifiedAt} />
          </Cell>
        ]
      }
    },
    {
      property: 'modifiedBy',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(messages.sharedModified)}
                column={column}
                contentType={searchDomain}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          modifiedBy => <Cell dWidth='dw120' ldWidth='ldw160'>
            <ModifiedByCell componentClass='small' modifiedBy={modifiedBy} />
          </Cell>
        ]
      }
    }
  ]
}
const getServicesColumnsDefinition = (formatMessages, onLanguageSelect, searchDomain, onSort, handlePreviewOnClick) => {
  return [
    {
      property: 'languageAvailabilities',
      header: {
        label: formatMessages(messages.sharedStatusAndLanguages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <Cell dWidth='dw60' ldWidth='ldw100'>
            <LanguageBarCell clickable {...rowData} />
          </Cell>
        ]
      }
    },
    {
      property: 'name',
      header: {
        label: formatMessages(messages.sharedName),
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessages(messages.sharedName)}
                column={column}
                contentType={searchDomain}
                onSort={onSort}

              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (name, { rowData }) => {
            const handleOnClick = () => {
              handlePreviewOnClick(rowData.id, formTypesEnum.SERVICEFORM)
            }
            return <Cell dWidth='dw160' ldWidth='ldw220' inline>
              <PTVIcon name='icon-eye' onClick={handleOnClick} />
              <NameCell onLanguageClick={onLanguageSelect} {...rowData} />
            </Cell>
          }
        ]
      }
    },
    {
      property: 'serviceTypeId',
      header: {
        // label: formatMessages(messages.sharedContentType)
        label: formatMessages(messages.serviceServiceType)
      },
      cell: {
        formatters: [
          (serviceTypeId, { serviceType }) => <Cell dWidth='dw100' ldWidth='ldw140'>
            <ServiceTypeCell id={serviceTypeId} serviceType={serviceType} />
          </Cell>
        ]
      }
    },
    {
      property: 'organizations',
      header: {
        label: formatMessages(messages.serviceOrganization)
      },
      cell: {
        formatters: [
          organizationIds => <Cell dWidth='dw160' ldWidth='ldw200'>
            <OrganizationCell OrganizationIds={organizationIds} />
          </Cell>
        ]
      }
    },
    {
      property: 'modified',
      header: {
        label: formatMessages(messages.sharedEdited),
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessages(messages.sharedEdited)}
                column={column}
                contentType={searchDomain}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          modifiedAt => <Cell dWidth='dw80' ldWidth='ldw100'>
            <ModifiedAtCell modifiedAt={modifiedAt} />
          </Cell>
        ]
      }
    },
    {
      property: 'modifiedBy',
      header: {
        label: formatMessages(messages.sharedModified),
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessages(messages.sharedModified)}
                column={column}
                contentType={searchDomain}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          modifiedBy => <Cell dWidth='dw120' ldWidth='ldw160'>
            <ModifiedByCell modifiedBy={modifiedBy} />
          </Cell>
        ]
      }
    }
  ]
}

const getChannelsColumnsDefinition = (formatMessage, onLanguageSelect, searchDomain, onSort, handlePreviewOnClick) => {
  return [
    {
      property: 'languageAvailabilities',
      header: {
        label: formatMessage(messages.sharedStatusAndLanguages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <Cell dWidth='dw60' ldWidth='ldw100'>
            <LanguageBarCell clickable {...rowData} />
          </Cell>
        ]
      }
    },
    {
      property: 'name',
      header: {
        label: formatMessage(messages.sharedName),
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(messages.sharedName)}
                column={column}
                contentType={searchDomain}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (name, { rowData }) => {
            const handleOnClick = () => {
              const formName = rowData.subEntityType && getKey(formAllTypes, rowData.subEntityType.toLowerCase())
              handlePreviewOnClick(rowData.id, formName)
            }
            return <Cell dWidth='dw200' ldWidth='ldw260' inline>
              <PTVIcon name='icon-eye' onClick={handleOnClick} />
              <NameCell onLanguageClick={onLanguageSelect} {...rowData} />
            </Cell>
          }
        ]
      }
    },
    {
      property: 'organizationId',
      header: {
        label: formatMessage(messages.channelOrganization)
      },
      cell: {
        formatters: [
          organizationId => <Cell dWidth='dw200' ldWidth='ldw260'>
            <OrganizationCell OrganizationIds={[organizationId]} />
          </Cell>
        ]
      }
    },
    {
      property: 'modified',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(messages.sharedEdited)}
                column={column}
                contentType={searchDomain}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          modifiedAt => <Cell dWidth='dw80' ldWidth='ldw100'>
            <ModifiedAtCell modifiedAt={modifiedAt} />
          </Cell>
        ]
      }
    },
    {
      property: 'modifiedBy',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(messages.sharedModified)}
                column={column}
                contentType={searchDomain}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          modifiedBy => <Cell dWidth='dw140' ldWidth='ldw180'>
            <ModifiedByCell modifiedBy={modifiedBy} />
          </Cell>
        ]
      }
    }
  ]
}
export const getSearchDomainColumns = (formatMessage, searchDomain, onLanguageSelect, onSort, handlePreviewOnClick) => {
  switch (searchDomain) {
    case 'services': return getServicesColumnsDefinition(formatMessage, onLanguageSelect, searchDomain, onSort, handlePreviewOnClick)
    case 'eChannel':
    case 'webPage':
    case 'printableForm':
    case 'phone':
    case 'serviceLocation':
    case 'channels': return getChannelsColumnsDefinition(formatMessage, onLanguageSelect, searchDomain, onSort, handlePreviewOnClick)
    case 'generalDescriptions': return getGeneralDescriptionColumnsDefinition(formatMessage, onLanguageSelect, searchDomain, onSort, handlePreviewOnClick)
    case 'organizations': return getOrganizationsColumnsDefinition(formatMessage, onLanguageSelect, searchDomain, onSort, handlePreviewOnClick)
    default: return []
  }
}
