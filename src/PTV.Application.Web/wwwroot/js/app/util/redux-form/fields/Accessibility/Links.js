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
  getAccessibilityRegister
 } from './selectors'
import { getSelectedEntityId, getEntityUnificRoot } from 'selectors/entities/entities'
import { copyToClip } from 'util/helpers'
import { apiCall3 } from 'actions'

const linkStyle = { display: 'block' }
class Links extends Component {
  handleOnOpen = this.props.setLinkAsClicked
  handleOnCopy = () => copyToClip(this.props.url)
  handleOnLoad = this.props.loadAccessibility
  render () {
    return (
      <div>
        <a style={linkStyle} target='_blank' href={this.props.url} onClick={this.handleOnOpen}>
          Avaa esteettömyyssovellus
        </a>
        <a style={linkStyle} href='#' onClick={this.handleOnCopy}>
          Luo / päivitä linkki, jonka voi lähettää tiedon täyttävälle henkilölle
        </a>
        {this.props.hasAlreadyBeenClicked &&
          <a style={linkStyle} href='#' onClick={this.handleOnLoad}>
            Päivitä tämän osoitteen esteettömyystiedot
            <small>(Tiedot päivitetty viimeksi 14.12.2017 12:34)</small>
          </a>}
      </div>
    )
  }
}
Links.propTypes = {
  hasAlreadyBeenClicked: PropTypes.bool,
  setLinkAsClicked: PropTypes.func,
  loadAccessibility: PropTypes.func,
  url: PropTypes.string
}

export default compose(
  connect(state => ({
    hasAlreadyBeenClicked: getIsAccessibilityRegisterSet(state),
    url: getAccessibilityRegisterUrl(state),
    json: getAccessibilityRegister(state)
  }), {
    setLinkAsClicked: () => ({ dispatch, getState }) => {
      const state = getState()
      dispatch(
        apiCall3({
          keys: [
            'accessibility',
            'isSet'
          ],
          payload: {
            endpoint: 'channel/SetServiceLocationChannelAccessibility',
            data: {
              id: getSelectedEntityId(state),
              unificRootId: getEntityUnificRoot(state)
            }
          }
        })
      )
    },
    loadAccessibility: () => ({ dispatch, getState }) => {
      const state = getState()
      dispatch(
        apiCall3({
          keys: [
            'accessibility',
            'load'
          ],
          payload: {
            endpoint: 'channel/LoadServiceLocationChannelAccessibility',
            data: {
              id: getSelectedEntityId(state),
              unificRootId: getEntityUnificRoot(state)
            }
          }
        })
      )
    }
  })
)(Links)
