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
  ShortDescriptionCell
} from 'Routes/FrontPageV2/components'
import './styles.scss'

const messages = defineMessages({
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
  channelPhoneTypes: {
    id: 'FrontPage.Channels.Search.Header.PhoneTypes',
    defaultMessage: 'Puhelinkanavan tyyppi'
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

const getGeneralDescriptionColumnsDefinition = (formatMessage, onLanguageSelect) => {
  return [
    {
      property: 'name',
      header: {
        label: formatMessage(messages.sharedName)
      },
      cell: {
        formatters: [
          (name, { rowData }) => <NameCell {...rowData} />
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
          (serviceClassIds, { rowData }) => <ServiceClassesCell serviceClassIds={serviceClassIds} {...rowData} />
        ]
      }
    },
    {
      property: 'shortDescription',
      header: {
        label: formatMessage(messages.generalDescriptionsShortDescription)
      },
      cell: {
        formatters: [
          (shortDescription, { rowData }) => <ShortDescriptionCell {...rowData} />
        ]
      }
    }
  ]
}
const getOrganizationsColumnsDefinition = (formatMessage, onLanguageSelect) => {
  return [
    {
      property: 'publishingStatusId',
      header: {
        label: formatMessage(messages.sharedPublishingStatus)
      },
      cell: {
        formatters: [
          publishingStatusId => <PublishingStatusCell publishingStatusId={publishingStatusId} />
        ]
      }
    },
    {
      property: 'name',
      header: {
        label: formatMessage(messages.sharedName)
      },
      cell: {
        formatters: [
          (name, { rowData }) => <NameCell onLanguageClick={onLanguageSelect} {...rowData} />
        ]
      }
    },
    {
      property: 'mainOrganization',
      header: {
        label: formatMessage(messages.organizationMainOrganizations)
      }
    },
    {
      property: 'subOrganizations',
      header: {
        label: formatMessage(messages.organizationSubOrganizations)
      }
    },
    {
      property: 'modified',
      header: {
        label: formatMessage(messages.sharedEdited)
      },
      cell: {
        formatters: [
          modifiedAt => <ModifiedAtCell modifiedAt={modifiedAt} />
        ]
      }
    },
    {
      property: 'modifiedBy',
      header: {
        label: formatMessage(messages.sharedModified)
      }
    }
  ]
}
const getServicesColumnsDefinition = (formatMessages, onLanguageSelect) => {
  return [
    {
      property: 'publishingStatusId',
      header: {
        label: formatMessages(messages.sharedPublishingStatus)
      },
      cell: {
        formatters: [
          publishingStatusId => <PublishingStatusCell publishingStatusId={publishingStatusId} />
        ]
      }
    },
    {
      property: 'name',
      header: {
        label: formatMessages(messages.sharedName)
      },
      cell: {
        formatters: [
          (name, { rowData }) => <NameCell onLanguageClick={onLanguageSelect} {...rowData} />
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
          (serviceTypeId, { serviceType }) => <ServiceTypeCell id={serviceTypeId} serviceType={serviceType} />
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
          organizationIds => <OrganizationCell OrganizationIds={organizationIds} />
        ]
      }
    },
    {
      property: 'modified',
      header: {
        label: formatMessages(messages.sharedEdited)
      },
      cell: {
        formatters: [
          modifiedAt => <ModifiedAtCell modifiedAt={modifiedAt} />
        ]
      }
    },
    {
      property: 'modifiedBy',
      header: {
        label: formatMessages(messages.sharedModified)
      },
      cell: {
        formatters: [
          modifiedBy => <ModifiedByCell modifiedBy={modifiedBy} />
        ]
      }
    }
  ]
}

const getChannelsColumnsDefinition = (formatMessage, onLanguageSelect) => {
  return [
    {
      property: 'publishingStatusId',
      header: {
        label: formatMessage(messages.sharedPublishingStatus)
      },
      cell: {
        formatters: [
          publishingStatusId => <PublishingStatusCell publishingStatusId={publishingStatusId} />
        ]
      }
    },
    {
      property: 'name',
      header: {
        label: formatMessage(messages.sharedName)
      },
      cell: {
        formatters: [
          (name, { rowData }) => <NameCell onLanguageClick={onLanguageSelect} {...rowData} />
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
          organizationId => <OrganizationCell OrganizationIds={[organizationId]} />
        ]
      }
    },
    {
      property: 'modified',
      header: {
        label: formatMessage(messages.sharedEdited)
      },
      cell: {
        formatters: [
          modifiedAt => <ModifiedAtCell modifiedAt={modifiedAt} />
        ]
      }
    },
    {
      property: 'modifiedBy',
      header: {
        label: formatMessage(messages.sharedModified)
      }
    }
  ]
}
export const getSearchDomainColumns = (formatMessage, searchDomain, onLanguageSelect) => {
  switch (searchDomain) {
    case 'services': return getServicesColumnsDefinition(formatMessage, onLanguageSelect)
    case 'eChannel':
    case 'webPage':
    case 'printableForm':
    case 'phone':
    case 'serviceLocation':
    case 'channels': return getChannelsColumnsDefinition(formatMessage, onLanguageSelect)
    case 'generalDescriptions': return getGeneralDescriptionColumnsDefinition(formatMessage, onLanguageSelect)
    case 'organizations': return getOrganizationsColumnsDefinition(formatMessage, onLanguageSelect)
    default: return []
  }
}
