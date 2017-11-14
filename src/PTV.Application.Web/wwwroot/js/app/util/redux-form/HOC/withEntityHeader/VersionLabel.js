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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { injectFormName } from 'util/redux-form/HOC'
import { localizeProps } from 'appComponents/Localize'
import { Popup } from 'appComponents'
import { Map } from 'immutable'
import {
  getEntity,
  getSelectedEntityId,
  getEntityUnificRoot
} from 'selectors/entities/entities'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { Button } from 'sema-ui-components'
import styles from './styles.scss'
import { EntitySelectors } from 'selectors'

export const messages = defineMessages({
  linkTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.VersionLabel.Title',
    defaultMessage: 'Tunnistetiedot'
  },
  entityIdTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.VersionLabel.EntityIdTitle',
    defaultMessage: 'Tunniste'
  },
  entityUnificRootIdTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.VersionLabel.EntityUnificRootIdTitle',
    defaultMessage: 'Versiotunniste'
  },
  entitySahaIdTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.VersionLabel.EntitySahaIdTitle',
    defaultMessage: 'Lähdejärjestelmätunniste'
  },
  copyLinkTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.VersionLabel.CopyLinkTitle',
    defaultMessage: 'Kopioi'
  }
})

const VersionInfo = compose(
  injectIntl,
  injectFormName,
  connect((state, { formName }) => {
    const entity = getEntity(state)
    return {
      pStatus: (EntitySelectors.publishingStatuses.getEntities(state) ||
        Map()).get(entity.get('publishingStatus') || Map()).get('code') || 'N',
      version: (entity.get('version') || Map()).get('value') || '0.0',
      entityID: getSelectedEntityId(state),
      unificRootId: getEntityUnificRoot(state)
    }
  }),
  localizeProps({
    nameAttribute: 'serviceType',
    idAttribute: 'type'
  })
)(({
  intl: { formatMessage },
  entityID,
  unificRootId,
  version,
  pStatus
}) => {
  const handleCopyEntityId = () => copyToClip(entityID)
  const handleCopyUnificRootId = () => copyToClip(unificRootId)
  const copyToClip = data => {
    var id = 'mycustom-clipboard-textarea-hidden-id'
    var existsTextarea = document.getElementById(id)
    if (!existsTextarea) {
      var textarea = document.createElement('textarea')
      textarea.id = id
      // Place in top-left corner of screen regardless of scroll position.
      textarea.style.position = 'fixed'
      textarea.style.top = 0
      textarea.style.left = 0
      // Ensure it has a small width and height. Setting to 1px / 1em
      // doesn't work as this gives a negative w/h on some browsers.
      textarea.style.width = '1px'
      textarea.style.height = '1px'
      // We don't need padding, reducing the size if it does flash render.
      textarea.style.padding = 0
      // Clean up any borders.
      textarea.style.border = 'none'
      textarea.style.outline = 'none'
      textarea.style.boxShadow = 'none'
      // Avoid flash of white box if rendered for any reason.
      textarea.style.background = 'transparent'
      document.querySelector('body').appendChild(textarea)
      existsTextarea = document.getElementById(id)
    }
    existsTextarea.value = data
    existsTextarea.select()
    document.execCommand('copy')
  }

  return (
    <div>
      <div className={styles.versionLabelItem}>
        <em>{formatMessage(messages.entityUnificRootIdTitle)}</em><span>{unificRootId}</span>
        <Button link onClick={handleCopyUnificRootId}>
          {formatMessage(messages.copyLinkTitle)}
        </Button>
      </div>
      <div className={styles.versionLabelItem}>
        <em>{formatMessage(messages.entityIdTitle)}</em><span>{entityID}</span>
        <Button link onClick={handleCopyEntityId}>
          {formatMessage(messages.copyLinkTitle)}
        </Button>
      </div>
      <span>{pStatus[0].toUpperCase() + ' ' + version}</span>
    </div>
  )
})
const VersionLabel = ({
  version,
  entityID,
  intl: { formatMessage }
}) => (
  <div>
    <Popup
      position={'top right'}
      trigger={
        <Button link onClick={() => {}}>
          {formatMessage(messages.linkTitle)}
        </Button>}
      maxWidth='mW500'
    >
      <VersionInfo />
    </Popup>
  </div>
)

VersionLabel.propTypes = {
  version: PropTypes.string.isRequired,
  entityID: PropTypes.string.isRequired,
  intl: intlShape
}

export default compose(
  injectIntl
)(VersionLabel)
