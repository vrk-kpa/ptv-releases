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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { connect } from 'react-redux'
import { compose } from 'redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getFormValue } from 'selectors/base'
import {
  getlinkToPublished,
  getlinkToModified
} from './selectors'
import { localizeProps } from 'appComponents/Localize'
import { withRouter } from 'react-router'
import { getSelectedEntityConcreteType } from 'selectors/entities/entities'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { entityConcreteTexts } from 'util/redux-form/messages'
import { injectIntl, intlShape } from 'util/react-intl'
import { Button, Spinner } from 'sema-ui-components'
import { formPaths } from 'enums'
import messages from './messages'
import styles from './styles.scss'

const EntityLabel = ({
  intl: { formatMessage },
  concreteEntityType,
  serviceType,
  linkToPublished,
  linkToModified,
  isLockingUnlocking,
  history,
  formName,
  inReview
}) => {
  const concreteType = formatMessage(entityConcreteTexts[concreteEntityType])
  const handleLinkToPublish = () => {
    history.push(`${formPaths[formName]}/${linkToPublished.get('idToEntity')}`)
  }
  const handleLinkToModified = () => {
    history.push(`${formPaths[formName]}/${linkToModified.get('idToEntity')}`)
  }
  return (
    <div className={styles.entityLabel}>
      <span>{concreteType}{serviceType && (' (' + serviceType + ')')}</span>
      {(!!linkToPublished || !!linkToModified) && isLockingUnlocking && <Spinner className={styles.spinner} />}
      {!!linkToPublished && !inReview &&
        <span>
          (<Button link disabled={isLockingUnlocking} onClick={handleLinkToPublish}>
            {(formatMessage(messages.linkToPublish, { date: linkToPublished.get('dateOfEntity') }))}
          </Button>)
        </span>
      }
      {!!linkToModified && !inReview &&
        <span>
          (<Button link disabled={isLockingUnlocking} onClick={handleLinkToModified}>
            {(formatMessage(messages.linkToModified, { date: linkToModified.get('dateOfEntity') }))}
          </Button>)
        </span>
      }
    </div>
  )
}

EntityLabel.propTypes = {
  concreteEntityType: PropTypes.string.isRequired,
  intl: intlShape,
  serviceType: PropTypes.string,
  linkToModified: ImmutablePropTypes.map,
  linkToPublished: ImmutablePropTypes.map,
  isLockingUnlocking: PropTypes.bool,
  history: PropTypes.object.isRequired,
  formName: PropTypes.string,
  inReview: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  withRouter,
  connect((state, { formName }) => ({
    concreteEntityType: getSelectedEntityConcreteType(state),
    type: getFormValue('serviceType')(state, { formName }),
    linkToPublished: getlinkToPublished(state),
    linkToModified: getlinkToModified(state),
    formName,
    inReview: getShowReviewBar(state)
  })),
  localizeProps({
    nameAttribute: 'serviceType',
    idAttribute: 'type'
  })
)(EntityLabel)
