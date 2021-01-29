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
import { getSelectedEntityId } from 'selectors/entities/entities'
import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'
import {
  getAccessibilityRegisterUrl,
  getAccessibilityRegisterId,
  getAccessibilityRegisterLanguageReplace
} from './selectors'
import { getAddressId } from 'selectors/addresses'
import { getContentLanguageCode } from 'selectors/selections'
import { getIntlLocale } from 'Intl/Selectors'
import { copyToClip } from 'util/helpers'
import { getShowInfosAction } from 'reducers/notifications'
import { messages } from 'util/redux-form/HOC/withCopyToClipboard/withCopyToClipboard'
import shortId from 'shortid'

const getARLink = (url, uiReplace, uiLanguage) => {
  return  url.replace(uiReplace, uiLanguage)
}

export const openAccessibility = ownProps => ({ dispatch, getState }) => {
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
          addressId: getAddressId(state, ownProps),
          languageCode: getContentLanguageCode(state)
        }
      },
      successNextAction: () => {
        const state = getState()
        const uiLanguage = getIntlLocale(state)
        const arUrl = getAccessibilityRegisterUrl(state)
        const uiReplace = getAccessibilityRegisterLanguageReplace(state)
        const url = getARLink(arUrl, uiReplace, uiLanguage)
        window.open(url, '_blank')
      }
    })
  )
}

export const loadAccessibility = () => ({ dispatch, getState }) => {
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
}

export const copyARLink = () => ({ dispatch, getState }) => {
  const state = getState()
  const uiLanguage = getIntlLocale(state)
  const arUrl = getAccessibilityRegisterUrl(state)
  const uiReplace = getAccessibilityRegisterLanguageReplace(state)
  const url = getARLink(arUrl, uiReplace, uiLanguage)
  copyToClip(url)
  dispatch(getShowInfosAction(shortId.generate(), true)([{ code: messages.copyFeedback }]))
}
