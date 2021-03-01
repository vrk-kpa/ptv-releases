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
  languageVersionTitleNew: {
    id: 'Routes.Form.LanguageVersion.New.Title',
    defaultMessage: 'Uusi kieliversio'
  },
  organizationLabel: {
    id: 'FrontPage.SelectOrganization.Title',
    defaultMessage: 'Organisaatio'
  },
  organizationTooltip: {
    id: 'FrontPage.SelectOrganization.Tooltip',
    defaultMessage: 'Valitse pudotusvalikosta haluamasi organisaatio tai organisaatiotaso.'
  },
  organizationPlaceholder: {
    id: 'Component.Select.Placeholder',
    defaultMessage: '- valitse -'
  },
  noneSelectedLabel: {
    id: 'SelectedOption.None.Title',
    defaultMessage: 'Ei valittu'
  }
})

export const buttonMessages = defineMessages({
  showMore: {
    id: 'Components.Buttons.ShowMoreButton',
    defaultMessage: 'Näytä lisää'
  },
  close: {
    id: 'Components.Buttons.Close',
    defaultMessage: 'Sulje',
    description: 'AppComponents.PreviewDialog.Buttons.Close.Title'
  }
})
