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
import React from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import {
  getIsProposedChannel,
  getIsNewConnection
} from './selectors'
import { getSelectedEntityType } from 'selectors/entities/entities'
import {
  getIsServiceAttachedAsASTI,
  getIsChannelAttachedAsASTI
} from '../selectors'
import { entityTypesEnum } from 'enums'
import Tag from 'appComponents/Tag'
import Tooltip from 'appComponents/Tooltip'
import styles from './styles.scss'

const messages = defineMessages({
  proposedChannelTitle: {
    id: 'AppComponents.ConnectionsStep.ProposedChannel.Title',
    defaultMessage: 'Pohjakuvauksessa ehdotettu kanava'
  },
  newConnectionTitle: {
    id: 'AppComponents.ConnectionsStep.NewConnection.Title',
    defaultMessage: 'Uusi'
  },
  attachedAsASTITitle: {
    id: 'AppComponents.ConnectionsStep.AttachedToASTI.Title',
    defaultMessage: 'Liitetty ASTI-järjestelmässä'
  },
  attachedAsASTITooltip: {
    id: 'AppComponents.ConnectionsStep.AttachedToASTI.Tooltip',
    defaultMessage: 'Palvelupaikka on liitetty palveluun ASTI-järjestelmässä. Et voi poistaa liitosta PTV:ssä. Voit lisätä liitostietoja ASTI-järjestelmässä liitetyt palvelupaikat -listan kautta.'
  }
})

const ConnectionTags = ({
  isProposedChannel,
  isNewConnection,
  isAttachedAsASTI,
  location,
  intl: { formatMessage }
}) => {
  if (!isAttachedAsASTI && !isProposedChannel && !isNewConnection) return null
  return <div>
    {isAttachedAsASTI &&
      <div className={styles.tagWrap}>
        <Tag
          message={formatMessage(messages.attachedAsASTITitle)}
          isRemovable={false} />
        <Tooltip tooltip={formatMessage(messages.attachedAsASTITooltip)} />
      </div>

    }
    {isProposedChannel &&
      <Tag
        message={formatMessage(messages.proposedChannelTitle)}
        isRemovable={false} />
    }
    {isNewConnection &&
      <Tag
        message={formatMessage(messages.newConnectionTitle)}
        isRemovable={false}
        bgColor='#00B38A'
        textColor='#FFFFFF' />
    }
  </div>
}

ConnectionTags.propTypes = {
  intl: intlShape,
  isProposedChannel: PropTypes.bool,
  isNewConnection: PropTypes.bool,
  isAttachedAsASTI: PropTypes.bool,
  location: PropTypes.string
}

export default compose(
  injectIntl,
  connect((state, ownProps) => {
    const entityType = getSelectedEntityType(state)
    const isAttachedAsASTI = {
      [entityTypesEnum.CHANNELS]: getIsServiceAttachedAsASTI(ownProps.entityId)(state),
      [entityTypesEnum.SERVICES]: getIsChannelAttachedAsASTI(ownProps.entityId)(state),
      [entityTypesEnum.GENERALDESCRIPTIONS]: false
    }[entityType]
    return {
      isProposedChannel: getIsProposedChannel(state, ownProps),
      isNewConnection: ownProps.location !== 'search' && getIsNewConnection(state, ownProps) || false,
      isAttachedAsASTI
    }
  })
)(ConnectionTags)
