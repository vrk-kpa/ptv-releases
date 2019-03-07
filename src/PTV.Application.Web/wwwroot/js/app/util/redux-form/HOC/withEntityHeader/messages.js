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

import { defineMessages } from 'util/react-intl'
import { formTypesEnum } from 'enums'

export default defineMessages({
  archiveEntityButton: {
    id: 'Components.Buttons.ArchiveButton',
    defaultMessage: 'Arkistoi {type}'
  },
  archiveLanguageButton: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.LanguageLabel.ArchiveLanguageButton.Title',
    defaultMessage: 'Arkistoi kieliversio'
  },
  archiveLanguageDialogTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.LanguageLabel.ArchiveDialog.Title',
    defaultMessage: 'Haluatko arkistoida kieliversion?'
  },
  archiveLanguageDialogText: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.LanguageLabel.ArchiveDialog.Text',
    defaultMessage: 'Kaikki kieliversion ({language}) tiedot arkistoidaan.'
  },
  withdrawLanguageButton: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.LanguageLabel.WithdrawLanguageButton.Title',
    defaultMessage: 'Palauta luonnokseksi'
  },
  withdrawEntityButton: {
    id: 'Components.Buttons.WithdrawButton',
    defaultMessage: 'Palauta luonnokseksi'
  },
  withdrawLanguageDialogTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.LanguageLabel.WithdrawDialog.Title',
    defaultMessage: 'Haluatko palauttaa julkaistun kieliversion luonnokseksi?'
  },
  withdrawLanguageDialogText: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.LanguageLabel.WithdrawDialog.Text',
    defaultMessage: 'Kaikki kieliversion ({language}) tiedot siirtyvät pois julkaisusta,  ja ne palautetaan luonnostilaan.'
  },
  restoreEntityButton: {
    id: 'Components.Buttons.RestoreButton',
    defaultMessage: 'Palauta arkistosta'
  },
  restoreLanguageButton: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.LanguageLabel.RestoreLanguageButton.Title',
    defaultMessage: 'Palauta arkistosta'
  },
  restoreLanguageDialogTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.LanguageLabel.RestoreDialog.Title',
    defaultMessage: 'Haluatko palauttaa arkistoidun kieliversion luonnokseksi?'
  },
  restoreLanguageDialogText: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.LanguageLabel.RestoreDialog.Text',
    defaultMessage: 'Kaikki kieliversion ({language}) tiedot palautetaan luonnostilaan.'
  },
  linkToPublish: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.LinkToPublish.Title',
    defaultMessage: 'Aiempi julkaistu versio {date}'
  },
  linkToModified: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.LinkToModified.Title',
    defaultMessage: 'Aiempi muokattu versio {date}'
  },
  acceptButton: {
    id: 'Components.Buttons.Accept',
    defaultMessage: 'Kyllä'
  },
  cancelButton: {
    id: 'Components.Buttons.Cancel',
    defaultMessage: 'Peruuta'
  },
  entityActionSelectPlaceholder: {
    id: 'EntityAction.Placeholder',
    defaultMessage: 'Toiminnot',
    description: 'Routes.Connections.components.WorkbenchActions.ActionSelectTitle.Placeholder'
  },
  copyButton: {
    id: 'EntityActions.Copy.Title',
    defaultMessage: 'Kopioi pohjaksi',
    description: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.CopyLink.Title'
  },
  translateButton: {
    id: 'EntityActions.Translate.Title',
    defaultMessage: 'Käännöstilaukset',
    description: 'Components.TranslationOrder.Link.Title'
  },
  createServiceButton: {
    id: 'EntityActions.CreateService.Title',
    defaultMessage: 'Luo uusi palvelu pohjakuvauksesta'
  }
})

export const dialogMessages = {
  [formTypesEnum.SERVICEFORM]: {
    archive: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.Service.ArchiveDialog.Title',
        defaultMessage: 'Haluatko arkistoida palvelun?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.Service.ArchiveDialog.Text',
        defaultMessage: 'Kaikki palvelun tiedot ja kieliversiot arkistoidaan.'
      }
    }),
    withdraw: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.Service.WithdrawDialog.Title',
        defaultMessage: 'Haluatko palauttaa julkaistun palvelun luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.Service.WithdrawDialog.Text',
        defaultMessage: 'Kaikki palvelun tiedot ja kieliversiot siirtyvät pois julkaisusta, ja ne palautetaan luonnostilaan.'
      }
    }),
    restore: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.Service.RestoreDialog.Title',
        defaultMessage: 'Haluatko palauttaa arkistoidun palvelun luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.Service.RestoreDialog.Text',
        defaultMessage: 'Kaikki palvelun tiedot ja kieliversiot palautetaan luonnostilaan.'
      }
    })
  },
  [formTypesEnum.SERVICECOLLECTIONFORM]: {
    archive: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ServiceCollection.ArchiveDialog.Title',
        defaultMessage: 'Haluatko arkistoida palvelun?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ServiceCollection.ArchiveDialog.Text',
        defaultMessage: 'Kaikki palvelun tiedot ja kieliversiot arkistoidaan.'
      }
    }),
    withdraw: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ServiceCollection.WithdrawDialog.Title',
        defaultMessage: 'Haluatko palauttaa julkaistun palvelun luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ServiceCollection.WithdrawDialog.Text',
        defaultMessage: 'Kaikki palvelun tiedot ja kieliversiot siirtyvät pois julkaisusta, ja ne palautetaan luonnostilaan.'
      }
    }),
    restore: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ServiceCollection.RestoreDialog.Title',
        defaultMessage: 'Haluatko palauttaa arkistoidun palvelun luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ServiceCollection.RestoreDialog.Text',
        defaultMessage: 'Kaikki palvelun tiedot ja kieliversiot palautetaan luonnostilaan.'
      }
    })
  },
  [formTypesEnum.ELECTRONICCHANNELFORM]: {
    archive: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ElectronicChannel.ArchiveDialog.Title',
        defaultMessage: 'Haluatko arkistoida verkkoasioinnin?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ElectronicChannel.ArchiveDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot arkistoidaan.'
      }
    }),
    withdraw: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ElectronicChannel.WithdrawDialog.Title',
        defaultMessage: 'Haluatko palauttaa julkaistun verkkoasioinnin luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ElectronicChannel.WithdrawDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot siirtyvät pois julkaisusta, ja ne palautetaan luonnostilaan.'
      }
    }),
    restore: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ElectronicChannel.RestoreDialog.Title',
        defaultMessage: 'Haluatko palauttaa arkistoidun verkkoasioinnin luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ElectronicChannel.RestoreDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot palautetaan luonnostilaan.'
      }
    })
  },
  [formTypesEnum.PHONECHANNELFORM]: {
    archive: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.PhoneChannel.ArchiveDialog.Title',
        defaultMessage: 'Haluatko arkistoida puhelinasioinnin?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.PhoneChannel.ArchiveDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot arkistoidaan.'
      }
    }),
    withdraw: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.PhoneChannel.WithdrawDialog.Title',
        defaultMessage: 'Haluatko palauttaa julkaistun puhelinasioinnin luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.PhoneChannel.WithdrawDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot siirtyvät pois julkaisusta, ja ne palautetaan luonnostilaan.'
      }
    }),
    restore: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.PhoneChannel.RestoreDialog.Title',
        defaultMessage: 'Haluatko palauttaa arkistoidun puhelinasioinnin luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.PhoneChannel.RestoreDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot palautetaan luonnostilaan.'
      }
    })
  },
  [formTypesEnum.PRINTABLEFORM]: {
    archive: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.PrintableChannel.ArchiveDialog.Title',
        defaultMessage: 'Haluatko arkistoida tulostettavan lomakkeen?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.PrintableChannel.ArchiveDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot arkistoidaan.'
      }
    }),
    withdraw: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.PrintableChannel.WithdrawDialog.Title',
        defaultMessage: 'Haluatko palauttaa julkaistun tulostettavan lomakkeen luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.PrintableChannel.WithdrawDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot siirtyvät pois julkaisusta, ja ne palautetaan luonnostilaan.'
      }
    }),
    restore: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.PrintableChannel.RestoreDialog.Title',
        defaultMessage: 'Haluatko palauttaa arkistoidun tulostettavan lomakkeen luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.PrintableChannel.RestoreDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot palautetaan luonnostilaan.'
      }
    })
  },
  [formTypesEnum.SERVICELOCATIONFORM]: {
    archive: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ServiceLocationChannel.ArchiveDialog.Title',
        defaultMessage: 'Haluatko arkistoida palvelupaikan?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ServiceLocationChannel.ArchiveDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot arkistoidaan.'
      }
    }),
    withdraw: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ServiceLocationChannel.WithdrawDialog.Title',
        defaultMessage: 'Haluatko palauttaa julkaistun palvelupaikan luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ServiceLocationChannel.WithdrawDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot siirtyvät pois julkaisusta, ja ne palautetaan luonnostilaan.'
      }
    }),
    restore: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ServiceLocationChannel.RestoreDialog.Title',
        defaultMessage: 'Haluatko palauttaa arkistoidun palvelupaikan luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ServiceLocationChannel.RestoreDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot palautetaan luonnostilaan.'
      }
    })
  },
  [formTypesEnum.WEBPAGEFORM]: {
    archive: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.WebPageChannel.ArchiveDialog.Title',
        defaultMessage: 'Haluatko arkistoida verkkosivun?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.WebPageChannel.ArchiveDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot arkistoidaan.'
      }
    }),
    withdraw: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.WebPageChannel.WithdrawDialog.Title',
        defaultMessage: 'Haluatko palauttaa julkaistun verkkosivun luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.WebPageChannel.WithdrawDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot siirtyvät pois julkaisusta, ja ne palautetaan luonnostilaan.'
      }
    }),
    restore: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.WebPageChannel.RestoreDialog.Title',
        defaultMessage: 'Haluatko palauttaa arkistoidun verkkosivun luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.WebPageChannel.RestoreDialog.Text',
        defaultMessage: 'Kaikki asiointikanavan tiedot ja kieliversiot palautetaan luonnostilaan.'
      }
    })
  },
  [formTypesEnum.GENERALDESCRIPTIONFORM]: {
    archive: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.GeneralDescription.ArchiveDialog.Title',
        defaultMessage: 'Haluatko arkistoida pohjakuvauksen?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.GeneralDescription.ArchiveDialog.Text',
        defaultMessage: 'Kaikki tiedot ja kieliversiot arkistoidaan. Liitos pohjakuvaukseen poistuu niiltä palveluita, joissa se on ollut käytössä.'
      }
    }),
    withdraw: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.GeneralDescription.WithdrawDialog.Title',
        defaultMessage: 'Haluatko palauttaa julkaistun pohjakuvauksen luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.GeneralDescription.WithdrawDialog.Text',
        defaultMessage: 'Kaikki pohjakuvauksen tiedot ja kieliversiot siirtyvät pois julkaisusta, ja ne palautetaan luonnostilaan.'
      }
    }),
    restore: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.GeneralDescription.RestoreDialog.Title',
        defaultMessage: 'Haluatko palauttaa arkistoidun pohjakuvauksen luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.GeneralDescription.RestoreDialog.Text',
        defaultMessage: 'Kaikki pohjakuvauksen tiedot ja kieliversiot palautetaan luonnostilaan.'
      }
    })
  },
  [formTypesEnum.ORGANIZATIONFORM]: {
    archive: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.Organization.ArchiveDialog.Title',
        defaultMessage: 'Haluatko arkistoida organisaation?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.Organization.ArchiveDialog.Text',
        defaultMessage: 'Kaikki organisaation tiedot ja kieliversiot arkistoidaan. Tässä pitäisi varmaan kertoa myös, mihin muuhun tällä on vaikutusta?'
      }
    }),
    withdraw: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.Organization.WithdrawDialog.Title',
        defaultMessage: 'Haluatko palauttaa julkaistun organisaation luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.Organization.WithdrawDialog.Text',
        defaultMessage: 'Kaikki organisaation tiedot ja kieliversiot siirtyvät pois julkaisusta, ja ne palautetaan luonnostilaan. Tässä pitäisi varmaan kertoa myös, mihin muuhun tällä on vaikutusta?'
      }
    }),
    restore: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.Organization.RestoreDialog.Title',
        defaultMessage: 'Haluatko palauttaa arkistoidun organisaation luonnokseksi?'
      },
      text: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.Organization.RestoreDialog.Text',
        defaultMessage: 'Kaikki organisaation tiedot ja kieliversiot palautetaan luonnostilaan.'
      }
    })
  }
}

export const idMessages = {
  [formTypesEnum.SERVICEFORM]: {
    headline: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.Service.Headline.Title',
        defaultMessage: 'Title for identification of service'
      },
      tooltip: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.Service.Headline.Tooltip',
        defaultMessage: 'Tooltip for identification of service'
      }
    })
  },
  [formTypesEnum.SERVICECOLLECTIONFORM]: {
    headline: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.ServiceColletion.Headline.Title',
        defaultMessage: 'Title for identification of service colletion'
      },
      tooltip: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.ServiceColletion.Headline.Tooltip',
        defaultMessage: 'Tooltip for identification of service colletion'
      }
    })
  },
  [formTypesEnum.ELECTRONICCHANNELFORM]: {
    headline: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.ElectronicChannel.Headline.Title',
        defaultMessage: 'Title for identification of electronic channel'
      },
      tooltip: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.ElectronicChannel.Headline.Tooltip',
        defaultMessage: 'Tooltip for identification of electronic channel'
      }
    })
  },
  [formTypesEnum.PHONECHANNELFORM]: {
    headline: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.PhoneChannel.Headline.Title',
        defaultMessage: 'Title for identification of phone channel'
      },
      tooltip: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.PhoneChannel.Headline.Tooltip',
        defaultMessage: 'Tooltip for identification of phone channel'
      }
    })
  },
  [formTypesEnum.PRINTABLEFORM]: {
    headline: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.PrintableChannel.Headline.Title',
        defaultMessage: 'Title for identification of printable channel'
      },
      tooltip: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.PrintableChannel.Headline.Tooltip',
        defaultMessage: 'Tooltip for identification of printable channel'
      }
    })
  },
  [formTypesEnum.SERVICELOCATIONFORM]: {
    headline: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.ServiceLocationChannel.Headline.Title',
        defaultMessage: 'Title for identification of service location channel'
      },
      tooltip: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.ServiceLocationChannel.Headline.Tooltip',
        defaultMessage: 'Tooltip for identification of service location channel'
      }
    })
  },
  [formTypesEnum.WEBPAGEFORM]: {
    headline: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.WebPageChannel.Headline.Title',
        defaultMessage: 'Title for identification of web page channel'
      },
      tooltip: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.WebPageChannel.Headline.Tooltip',
        defaultMessage: 'Tooltip for identification of web page channel'
      }
    })
  },
  [formTypesEnum.GENERALDESCRIPTIONFORM]: {
    headline: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.GeneralDescription.Headline.Title',
        defaultMessage: 'Title for identification of general description'
      },
      tooltip: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.GeneralDescription.Headline.Tooltip',
        defaultMessage: 'Tooltip for identification of general description'
      }
    })
  },
  [formTypesEnum.ORGANIZATIONFORM]: {
    headline: defineMessages({
      title: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.Organization.Headline.Title',
        defaultMessage: 'Title for identification of organization'
      },
      tooltip: {
        id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityVersion.Organization.Headline.Tooltip',
        defaultMessage: 'Tooltip for identification of organization'
      }
    })
  }
}
