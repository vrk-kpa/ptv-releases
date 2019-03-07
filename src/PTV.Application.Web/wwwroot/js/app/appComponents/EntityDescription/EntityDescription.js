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
import PropTypes from 'prop-types'
import { PTVIcon } from 'Components'
import { connect } from 'react-redux'
import { compose } from 'redux'
import NameCell from 'appComponents/Cells/NameCell'
import OrganizationCell from 'appComponents/Cells/OrganizationCell'
import Tag from 'appComponents/Tag'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { mergeInUIState } from 'reducers/ui'
import { createGetEntityAction } from 'actions'
import { getPreviewEntityFormName } from './selectors'
import {
  getIsProposedChannelConnectionByConnectionIndex,
  getIsProposedChannelSearchResults
} from 'Routes/Connections/selectors'
import styles from './styles.scss'
import { getSelectedLanguage } from 'Intl/Selectors'

const messages = defineMessages({
  newConnectionTitle: {
    id: 'AppComponents.ConnectionsStep.NewConnection.Title',
    defaultMessage: 'Uusi'
  },
  proposedChannelTitle: {
    id: 'AppComponents.ConnectionsStep.ProposedChannel.Title',
    defaultMessage: 'Pohjakuvauksessa ehdotettu kanava'
  }
})

const EntityDescription = ({
  name,
  entityId,
  organizationId,
  entityConcreteType,
  mergeInUIState,
  loadPreviewEntity,
  previewEntityformName,
  isNewConnection,
  isProposedChannel,
  preview,
  languageCode,
  intl: { formatMessage }
}) => {
  const handleOnPreviewClick = () => {
    mergeInUIState({
      key: 'entityPreviewDialog',
      value: {
        sourceForm: previewEntityformName,
        isOpen: true,
        entityId: null
      }
    })
    loadPreviewEntity(entityId, previewEntityformName)
  }
  return (
    <div className={styles.entityDescription}>
      <div className={styles.previewIcon}>
        { preview && <PTVIcon name='icon-eye' onClick={handleOnPreviewClick} /> }
      </div>
      <div className={styles.content}>
        <NameCell
          name={name}
          languageCode={languageCode}
        />
        <OrganizationCell organizationId={organizationId} compact />
        {isNewConnection &&
          <Tag
            message={formatMessage(messages.newConnectionTitle)}
            isRemovable={false}
            bgColor='#00B38A'
            textColor='#FFFFFF'
          />
        }
        {isProposedChannel &&
          <Tag
            message={formatMessage(messages.proposedChannelTitle)}
            isRemovable={false}
          />
        }
      </div>
    </div>
  )
}

EntityDescription.propTypes = {
  name: PropTypes.any,
  entityId: PropTypes.string,
  organizationId: PropTypes.string,
  entityConcreteType: PropTypes.string,
  mergeInUIState: PropTypes.func,
  loadPreviewEntity: PropTypes.func,
  previewEntityformName: PropTypes.string,
  isNewConnection: PropTypes.bool,
  isProposedChannel: PropTypes.bool,
  preview: PropTypes.bool,
  languageCode: PropTypes.string,
  intl: intlShape
}
EntityDescription.defaultProps = {
  preview: true
}
export default compose(
  injectIntl,
  connect((state, ownProps) => ({
    previewEntityformName: getPreviewEntityFormName(state, ownProps),
    isProposedChannel: typeof ownProps.connectionIndex !== 'undefined'
      ? getIsProposedChannelConnectionByConnectionIndex(state, ownProps)
      : getIsProposedChannelSearchResults(state, ownProps),
    languageCode: getSelectedLanguage(state, ownProps)
  }), {
    mergeInUIState,
    loadPreviewEntity: createGetEntityAction
  })
)(EntityDescription)
