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
import { openAccessibility, loadAccessibility, copyARLink } from './actions'
import {
  getIsAccessibilityRegisterSet,
  getCanSetAccessibility,
  getIsLoadingSentences
} from './selectors'
import { getSelectedEntityId } from 'selectors/entities/entities'
import { Button, Spinner } from 'sema-ui-components'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import Tooltip from 'appComponents/Tooltip'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import cx from 'classnames'
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
  },
  copyLinkTitle: {
    id: 'Accessibility.GenerateLink.Title',
    defaultMessage: 'Kopioi',
    description: 'Util.ReduxForm.HOC.WithEntityHeader.VersionLabel.CopyLinkTitle'
  }
})

class Links extends Component {
  handleOnOpen = this.props.openAccessibility
  handleOnLoad = this.props.loadAccessibility
  handleOnCopy = this.props.copyARLink
  render () {
    const {
      isAccessibilityRegisterSet,
      canSetAccessibility,
      isLoadingSentences,
      intl: { formatMessage },
      className
    } = this.props
    const linksClass = cx(
      styles.links,
      className
    )
    return (
      <div className={linksClass}>
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
                children={formatMessage(messages.copyLinkTitle)}
                type='button'
                className={styles.copyLink}
              />
              <Tooltip
                tooltip={formatMessage(messages.copyTooltip)}
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
  copyARLink: PropTypes.func,
  loadAccessibility: PropTypes.func,
  canSetAccessibility: PropTypes.bool,
  isLoadingSentences: PropTypes.bool,
  isGeneratingLink: PropTypes.bool,
  intl: intlShape,
  className: PropTypes.string
}

export default compose(
  injectIntl,
  injectFormName,
  connect(state => {
    return {
      isAccessibilityRegisterSet: getIsAccessibilityRegisterSet(state),
      serviceChannelVersionedId: getSelectedEntityId(state),
      isLoadingSentences: getIsLoadingSentences(state),
      canSetAccessibility: getCanSetAccessibility(state)
    }
  }, (dispatch, ownProps) => {
    return {
      openAccessibility: () => dispatch(openAccessibility(ownProps)),
      loadAccessibility: () => dispatch(loadAccessibility()),
      copyARLink: () => dispatch(copyARLink()),
    }
  })
)(Links)
