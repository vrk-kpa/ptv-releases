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
import React, { PropTypes } from 'react'
import { connect } from 'react-redux'

// actions
import * as channelActions from '../../Common/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// components
import { PTVUrlChecker } from '../../../../Components';

// selectors
import * as CommonSelectors from '../Selectors';

// schemas
import { CommonChannelsSchemas } from '../Schemas';

export const ChannelUrl = ({
  actions,
  channelId,
  language,
  keyToState,
  translationMode,
  readOnly,
  urlAddress,
  urlId,
  urlExists,
  isFetching,
  areDataValid,
  messages,
  validators
}) => {
  const onObjectChange = (input, isSet = false) => value => {
    actions.onChannelObjectChange(
      channelId,
      {
        ['webPage'] : { [input] : value }
      },
      isSet,
      language
    )
  }

  const checkUrl = value => {
    actions.channelsApiCall(
      [keyToState, 'webPage'],
      {
        endpoint: 'url/Check',
        data: { urlAddress: value }
      },
      null,
      CommonChannelsSchemas.URL
    )
  }

  const body = (
    <PTVUrlChecker
      messages={messages}
      value={urlAddress}
      id={urlId}
      inputValidators={validators}
      checkUrlCallback={checkUrl}
      blurCallback={onObjectChange('urlAddress')}
      showPreloader={isFetching}
      showMessage={areDataValid}
      urlExists={urlExists}
      readOnly={readOnly && translationMode === 'none'}
      disabled={translationMode === 'view'}
      maxLength={500}
      translationMode={translationMode}
    />
  )
  return (!readOnly || readOnly && urlAddress.length>0) && body
}

ChannelUrl.propTypes = {
  messages: PropTypes.object.isRequired
}

function mapStateToProps(state, ownProps) {

  return {
      urlAddress: CommonSelectors.getUrlAddress(state, ownProps),
      urlId: CommonSelectors.getUrlId(state, ownProps),
      urlExists: CommonSelectors.getUrlExists(state, ownProps),
      isFetching: CommonSelectors.getUrlIsFetching(state, ownProps),
      areDataValid: CommonSelectors.getUrlAreDataValid(state, ownProps),
      channelId: CommonSelectors.getChannelId(state, ownProps),
  }
}

const actions = [
    channelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(ChannelUrl);
