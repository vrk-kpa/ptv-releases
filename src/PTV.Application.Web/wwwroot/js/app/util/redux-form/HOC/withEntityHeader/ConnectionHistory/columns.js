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
import { defineMessages, FormattedMessage } from 'util/react-intl'
import NameCell from 'appComponents/Cells/NameCell'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import ModifiedByCell from 'appComponents/Cells/ModifiedByCell'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import HeaderFormatter from 'appComponents/HeaderFormatter/HeaderFormatter'

const messages = defineMessages({
  name: {
    id: 'FrontPage.Shared.Search.Header.Name',
    defaultMessage: 'Nimi'
  },
  languages: {
    id: 'AppComponents.RadioButtonCell.Header.Languages',
    defaultMessage: 'Kieli ja tila'
  },
  edited: {
    id: 'FrontPage.Shared.Search.Header.Edited',
    defaultMessage: 'Muokattu'
  },
  modified: {
    id: 'FrontPage.Shared.Search.Header.Modifier',
    defaultMessage: 'Muokkaaja'
  },
  operationType: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.ConnectionHistory.OperationType',
    defaultMessage: 'Muutos'
  },
  addOperation: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.ConnectionHistory.AddOperation',
    defaultMessage: 'Lisätty'
  },
  removeOperation: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.ConnectionHistory.RemoveOperation',
    defaultMessage: 'Poistettu'
  }
})

const columns = [
  {
    property: 'languagesAvailabilities',
    header: {
      label: <HeaderFormatter label={messages.languages} />
    },
    cell: {
      formatters: [
        (languagesAvailabilities, { rowData }) => {
          return <LanguageBarCell {...rowData} />
        }
      ]
    }
  },
  {
    property: 'name',
    header: {
      label: <HeaderFormatter label={messages.name} />
    },
    cell: {
      formatters: [
        name => {
          return <NameCell name={name} />
        }
      ]
    }
  },
  {
    property: 'createdBy',
    header: {
      label: <HeaderFormatter label={messages.modified} />
    },
    cell: {
      formatters: [
        createdBy => <ModifiedByCell modifiedBy={createdBy} />
      ]
    }
  },
  {
    property: 'operationType',
    header: {
      label: <HeaderFormatter label={messages.operationType} />
    },
    cell: {
      formatters: [
        operationType => {
          return {
            added: <FormattedMessage {...messages.addOperation} />,
            deleted: <FormattedMessage {...messages.removeOperation} />
          }[operationType.toLowerCase()]
        }
      ]
    }
  },
  {
    property: 'created',
    header: {
      label: <HeaderFormatter label={messages.edited} />
    },
    cell: {
      formatters: [
        created => <ModifiedAtCell modifiedAt={created} />
      ]
    }
  }
]

export default columns
