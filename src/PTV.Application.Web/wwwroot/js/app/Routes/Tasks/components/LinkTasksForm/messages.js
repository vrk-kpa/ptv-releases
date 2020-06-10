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
  urlUnstableLabel: {
    id: 'Tasks.BrokenLinks.Details.Url.Label',
    defaultMessage: 'Muokkaa verkko-osoitetta tarvittaessa.'
  },
  urlExceptionLabel: {
    id: 'BrokenLinks.Exception.Details.Url.Label',
    defaultMessage: 'Poikkeukseksi merkityn linkin osoite. Muokataksesi linkkiä, poista se poikkeustilasta.'
  },
  contentTableTitle: {
    id: 'Tasks.BrokenLinks.Details.ContentTable.Title',
    defaultMessage: 'Sisällöt, joissa verkko-osoite on käytössä ({count})'
  },
  contentTableName: {
    id: 'Tasks.BrokenLinks.Details.ContentTable.Name',
    defaultMessage: 'Sisällön nimi'
  },
  contentTableType: {
    id: 'Tasks.BrokenLinks.Details.ContentTable.Type',
    defaultMessage: 'Sisällötyyppi'
  },
  exceptionToggleTitle: {
    id: 'Tasks.BrokenLinks.Details.Exception.Title',
    defaultMessage: 'Merkitse linkki poikkeukseksi'
  },
  exceptionToggleTooltip: {
    id: 'Tasks.BrokenLinks.Details.Exception.Tooltip',
    defaultMessage: 'Mikäli toimiva linkki tulee toistuvasti epävakaiden linkkien listaan, voit merkitä sen poikkeukseksi. Poikkeustilaiset linkit eivät enää nouse epävakaiden listaan. Voit merkitä poikkeukseksi esim. sivut, jotka latautuvat hitaasti, mutta toimivat.'
  },
  exceptionToggleDescription: {
    id: 'Tasks.BrokenLinks.Details.Exception.Description',
    defaultMessage: 'Linkintarkistus ei tuo poikkeustilaisia linkkejä uudelleen epävakaiden linkkien listaan.'
  },
  exceptionCommentTitle: {
    id: 'Tasks.BrokenLinks.Details.Exception.Comment',
    defaultMessage: 'Lisätietoja linkin toiminnasta'
  },
  exceptionCommentTooltip: {
    id: 'BrokenLinks.Exception.Details.Comment.Tooltip',
    defaultMessage: 'Tooltip for exception link comment'
  },
  exceptionCommentPlaceholder: {
    id: 'Tasks.BrokenLinks.Details.Exception.Placeholder',
    defaultMessage: 'Linkki nousee aina epävakaana, mutta toimii.'
  },
  unstableCommentPlaceholder: {
    id: 'BrokenLinks.Unstable.Details.Comment.Placeholder',
    defaultMessage: 'Kuvaa tähän miksi linkki on merkitty poikkeuksi.'
  }
})
