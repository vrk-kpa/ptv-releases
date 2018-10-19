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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  getAccessibilityRegisterUrl,
  getIsAccessibilityRegisterSet,
  getMainAccessibilityRegister,
  getAccessibilityRegisterSetAt,
  getAccessibilityRegisterId,
  getIsAccessibilityRegisterValid,
  getCanSetAccessibility,
  getIsLoadingSentences
} from './selectors'
import { getSelectedEntityId } from 'selectors/entities/entities'
import { copyToClip } from 'util/helpers'
import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'
import { Button, Spinner } from 'sema-ui-components'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { getAddressId } from 'selectors/addresses'
import Tooltip from 'appComponents/Tooltip'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import styles from './styles.scss'

const messages = defineMessages({
  open: {
    id: 'Accessibility.Links.Open',
    defaultMessage: 'Avaa esteettömyyssovellus'
  },
  copy: {
    id: 'Accessibility.Links.Copy',
    defaultMessage: 'Luo / päivitä linkki, jonka voi lähettää tiedon täyttävälle henkilölle'
  },
  load: {
    id: 'Accessibility.Links.Load',
    defaultMessage: 'Päivitä tämän osoitteen esteettömyystiedot'
  },
  openTooltip: {
    id: 'Accessibility.Links.Open.Tooltip',
    defaultMessage: 'Open tooltip placeholder'
  },
  copyTooltip: {
    id: 'Accessibility.Links.Copy.Tooltip',
    defaultMessage: 'Copy tooltip placeholder'
  },
  updateTooltip: {
    id: 'Accessibility.Links.Update.Tooltip',
    defaultMessage: 'Update tooltip placeholder'
  }
})

class Links extends Component {
  handleOnOpen = this.props.openAccessibility
  handleOnCopy = () => copyToClip(this.props.url)
  handleOnLoad = this.props.loadAccessibility
  render () {
    const {
      isAccessibilityRegisterSet,
      canSetAccessibility,
      isLoadingSentences,
      intl: { formatMessage }
    } = this.props
    return (
      <div className={styles.links}>
        <div className={styles.linkWrap}>
          <Button
            disabled={!canSetAccessibility}
            link
            onClick={this.handleOnOpen}
            children={formatMessage(messages.open)}
            type='button'
          />
          <Tooltip
            tooltip={formatMessage(messages.openTooltip)}
            showInReadOnly
            indent='i5' />
        </div>
        {isAccessibilityRegisterSet && (
          <div>
            <div className={styles.linkWrap}>
              <span>{formatMessage(messages.copy)}</span>
              <Button
                disabled={!canSetAccessibility}
                link
                onClick={this.handleOnCopy}
                children={formatMessage(CellHeaders.copyTitle)}
                type='button'
                className={styles.copyLink}
              />
              <Tooltip
                tooltip={formatMessage(messages.copyTooltip)}
                showInReadOnly
                indent='i5' />
            </div>
            <div className={styles.linkWrap}>
              <Button
                link
                onClick={this.handleOnLoad}

                type='button'
              >
                {isLoadingSentences
                  ? <Spinner />
                  : <div>{formatMessage(messages.load)}</div>}
              </Button>
              <Tooltip
                tooltip={formatMessage(messages.updateTooltip)}
                showInReadOnly
                indent='i5' />
            </div>
          </div>
        )}
      </div>
    )
  }
}
Links.propTypes = {
  isAccessibilityRegisterSet: PropTypes.bool,
  openAccessibility: PropTypes.func,
  loadAccessibility: PropTypes.func,
  canSetAccessibility: PropTypes.bool,
  isLoadingSentences: PropTypes.bool,
  url: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => {
    return {
      isAccessibilityRegisterSet: getIsAccessibilityRegisterSet(state),
      url: getAccessibilityRegisterUrl(state),
      json: getMainAccessibilityRegister(state),
      setAt: getAccessibilityRegisterSetAt(state),
      serviceChannelVersionedId: getSelectedEntityId(state),
      isLoadingSentences: getIsLoadingSentences(state),
      canSetAccessibility: getCanSetAccessibility(state)
    }
  }, (dispatch, ownProps) => {
    return {
      openAccessibility: () => dispatch(({ getState }) => {
        const state = getState()
        dispatch(
          apiCall3({
            schemas: EntitySchemas.ADDRESS,
            keys: [
              'accessibility',
              'isSet'
            ],
            payload: {
              endpoint: 'channel/SetServiceLocationChannelAccessibility',
              data: {
                id: getAccessibilityRegisterId(state),
                serviceChannelVersionedId: getSelectedEntityId(state),
                addressId: getAddressId(state, ownProps)
              }
            },
            successNextAction: () => {
              const state = getState()
              const url = getAccessibilityRegisterUrl(state)
              window.open(url, '_blank')
            }
          })
        )
      }),
      loadAccessibility: () => dispatch(({ dispatch, getState }) => {
        const state = getState()
        dispatch(
          apiCall3({
            schemas: EntitySchemas.CHANNEL,
            keys: [
              'accessibility',
              'load'
            ],
            payload: {
              endpoint: 'channel/LoadServiceLocationChannelAccessibility',
              data: {
                serviceChannelVersionedId: getSelectedEntityId(state),
                accessibilityRegisterId: getAccessibilityRegisterId(state)
              }
            }
          })
        )
      })
    }
  })
)(Links)
