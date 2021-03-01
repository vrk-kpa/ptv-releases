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
import React, { Fragment } from 'react'
import { compose } from 'redux'
import MassToolConfirm from 'Routes/Search/components/MassTool/MassToolConfirm'
import TooltipLabel from 'appComponents/TooltipLabel'
import Paragraph from 'appComponents/Paragraph'
import { MassToolType } from 'util/redux-form/fields'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { massToolTypes } from 'enums'
import PropTypes from 'prop-types'
import { formValues } from 'redux-form/immutable'
import styles from './styles.scss'

const messages = defineMessages({

  massToolNotUpdatedTitle: {
    id: 'MassToolSelection.Tasks.NotUpdated.Title',
    defaultMessage: 'Select bulk action'
  },
  massToolNotUpdatedTooltip: {
    id: 'MassToolSelection.Tasks.NotUpdated.Tooltip',
    defaultMessage: 'Info of selecting bulk action'
  },
  massToolNotUpdatedDescription: {
    id: 'MassToolSelection.Tasks.NotUpdated.Description',
    defaultMessage: 'Select the action that you want to perform to a content group.'
  },
  massToolPublishTitle: {
    id: 'MassToolSelection.Tasks.NotUpdated.Publish.Title',
    defaultMessage: 'Bulk publishing'
  },
  massToolPublishTooltip: {
    id: 'MassToolSelection.Tasks.NotUpdated.Publish.Tooltip',
    defaultMessage: 'Info of bulk publishing'
  },
  massToolPublishDescription1: {
    id: 'MassToolSelection.Tasks.NotUpdated.Publish.Description1',
    defaultMessage: 'Select the not updated contents that you want to include in the bulk publication. The contents you have selected will be collected into the content cart. You can modify your selections by clicking the “Content cart” icon.' // eslint-disable-line
  },
  massToolPublishDescription2: {
    id: 'MassToolSelection.Tasks.NotUpdate.Publish.Description2',
    defaultMessage: 'When you have selected the contents, you can click the “Bulk publish” icon to move on to a different view in which you can check the contents one by one and then publish them all at the same time.' // eslint-disable-line
  },
  massToolArchiveTitle: {
    id: 'MassToolSelection.Tasks.NotUpdated.Archive.Title',
    defaultMessage: 'Bulk archiving'
  },
  massToolArchiveTasksTooltip: {
    id: 'MassToolSelection.Tasks.NotUpdated.Archive.Tooltip',
    defaultMessage: 'Info of bulk archiving'
  },
  massToolArchiveTasksDescription1: {
    id: 'MassToolSelection.Tasks.NotUpdated.Archive.Description1',
    defaultMessage: 'Select the not updated contents that you want to include in the bulk archiving. The contents you have selected will be collected into the content cart. You can modify your selections by clicking the “Content cart” icon.' // eslint-disable-line
  },
  massToolArchiveTasksDescription2: {
    id: 'MassToolSelection.Tasks.NotUpdated.Archive.Description2',
    defaultMessage: 'When you have selected the contents, you can click the “Archive” icon and then archive them all at the same time.' // eslint-disable-line
  }
})

const showOnlyMassTypes = [massToolTypes.PUBLISH, massToolTypes.ARCHIVE]

const MassToolSelectionTasksNotUpdated = ({ intl: { formatMessage }, massToolType }) => {
  const isMassPublish = massToolType === massToolTypes.PUBLISH
  return (
    <Fragment>
      <div className='row'>
        <div className='col-lg-12'>
          <TooltipLabel
                labelProps={{ labelText: formatMessage(messages.massToolNotUpdatedTitle) }}
                tooltipProps={{ tooltip: formatMessage(messages.massToolNotUpdatedTooltip) }}
                componentClass={styles.label}
              />

          <Paragraph>{formatMessage(messages.massToolNotUpdatedDescription)}</Paragraph>
          <div className='row'>
            <div className='col-lg-6'>
              <div className={styles.massToolType}>
                <MassToolType allowOnlyTypes={showOnlyMassTypes}
                  small
                />
              </div>
            </div>
            <div className='col-lg-18'>
              <TooltipLabel
                labelProps={{ labelText: formatMessage(isMassPublish && messages.massToolPublishTitle || messages.massToolArchiveTitle) }}
                tooltipProps={{ tooltip: formatMessage(isMassPublish && messages.massToolPublishTooltip || messages.massToolArchiveTasksTooltip) }}
                componentClass={styles.label}
              />
              <Paragraph>
                {formatMessage(isMassPublish && messages.massToolPublishDescription1 || messages.massToolArchiveTasksDescription1)}
                <br />
                {formatMessage(isMassPublish && messages.massToolPublishDescription2 || messages.massToolArchiveTasksDescription2)}
              </Paragraph>
            </div>
          </div>
        </div>
        <div className='col-lg-12 align-self-end'>
          <div className={styles.massToolConfirmWrap}>
            <MassToolConfirm />
          </div>
        </div>
      </div>
    </Fragment>
  )
}

MassToolSelectionTasksNotUpdated.propTypes = {
  intl: intlShape,
  massToolType: PropTypes.string
}

export default compose(
  injectIntl,
  formValues({ massToolType: 'type' }),
)(MassToolSelectionTasksNotUpdated)
