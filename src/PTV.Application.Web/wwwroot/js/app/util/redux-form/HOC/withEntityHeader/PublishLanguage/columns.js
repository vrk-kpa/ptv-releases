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
import { defineMessages } from 'util/react-intl'
import DateTimeInputCell from 'appComponents/Cells/DateTimeInputCell/DateTimeInputCell'
import HeaderFormatter from 'appComponents/HeaderFormatter/HeaderFormatter'
import { formEntityTypes } from 'enums'
import { EntitySelectors } from 'selectors'

const messages = defineMessages({
  validFromCell: {
    id: 'Components.PublishingEntityForm.Cell.ValidFrom',
    defaultMessage: 'Julkaisupäivä'
  },
  validToCell: {
    id: 'Components.PublishingEntityForm.Cell.ValidTo',
    defaultMessage: 'Arkistoitumispäivä'
  }
})

export const getPublishColumnsDefinition = (
  formatMessage,
  publishStatusId,
  archiveStatusId,
  disableEdit,
  onSubmit,
  expireOn,
  canArchiveAsti) => {
  return [
    {
      property: 'validFrom',
      header: {
        label: <HeaderFormatter label={messages.validFromCell} />
      },
      cell: {
        formatters: [
          (validFrom, { rowIndex, rowData }) => {
            return <DateTimeInputCell
              name={`languagesAvailabilities[${rowIndex}].validFrom`}
              valuePath={['languagesAvailabilities', rowIndex, 'validFrom']}
              label={null}
              type='from'
              isCompareMode={false}
              compare={false}
              readOnly={disableEdit ||
                !rowData.statusId ||
                rowData.statusId === publishStatusId ||
                rowData.statusId === archiveStatusId
              }
              formatMessage={formatMessage}
              onSubmit={() => onSubmit(rowIndex, 'SchedulePublish')}
              isSavingSelector={EntitySelectors[formEntityTypes[rowData.formName]].getEntityDialogIsPublishing}
              filterDate={expireOn}
            />
          }
        ]
      }
    },
    {
      property: 'validTo',
      header: {
        label: <HeaderFormatter label={messages.validToCell} />
      },
      cell: {
        formatters: [
          (validTo, { rowIndex, rowData }) => {
            return <DateTimeInputCell
              name={`languagesAvailabilities[${rowIndex}].validTo`}
              valuePath={['languagesAvailabilities', rowIndex, 'validTo']}
              label={null}
              type='to'
              isCompareMode={false}
              compare={false}
              readOnly={disableEdit || !rowData.statusId || rowData.statusId === archiveStatusId || !canArchiveAsti}
              formatMessage={formatMessage}
              onSubmit={() => onSubmit(rowIndex, 'ScheduleArchive')}
              isSavingSelector={EntitySelectors[formEntityTypes[rowData.formName]].getEntityDialogIsPublishing}
              filterDatePath={['languagesAvailabilities', rowIndex, 'validFrom']}
            />
          }
        ]
      }
    }
  ]
}
