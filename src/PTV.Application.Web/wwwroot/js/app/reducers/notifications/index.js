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
import { handleActions, createAction } from 'redux-actions'
import { Map } from 'immutable'
import shortid from 'shortid'
import { notificationTypesEnum, notificationButtonsEnum } from 'enums/notification'

export const RESET_ALL_MESSAGES = 'RESET_ALL_MESSAGES'
export const resetAllMessages = createAction(RESET_ALL_MESSAGES)

export const SHOW_MESSAGE = 'SHOW_MESSAGE'
export const showMessage = createAction(SHOW_MESSAGE)

export const SHOW_MESSAGES = 'SHOW_MESSAGES'
export const showMessages = createAction(SHOW_MESSAGES)

export const HIDE_MESSAGE = 'HIDE_MESSAGE'
export const hideMessage = createAction(HIDE_MESSAGE)

export const HIDE_MESSAGES = 'HIDE_MESSAGES'
export const hideMessages = createAction(HIDE_MESSAGES)

export const HIDE_MESSAGE_BY_TYPE = 'HIDE_MESSAGE_BY_TYPE'
export const hideMessageByType = createAction(HIDE_MESSAGE_BY_TYPE)

export const KEEP_OPEN_MESSAGE = 'KEEP_OPEN_MESSAGE'
export const keepOpen = createAction(KEEP_OPEN_MESSAGE)

export const COMMON_NOTIFICATION_KEY = 'common'

const formatNotification = (message, type, key, buttons = notificationButtonsEnum.close) =>
  typeof message === 'string'
    ? Map({ code: message, type, buttons })
    : Map(message).merge({ type, key, buttons })

const createNotifications = (messages, type, key, buttons) =>
  Map(messages.reduce(
    (prev, curr) => {
      const notification = formatNotification(curr, type, key, buttons)
      return prev.set(notification.get('id') || shortid.generate(), notification)
    }, Map()
  ))

const getShowMessagesAction = (key, autohide, notifications) =>
  ({ dispatch }) => {
    autohide && setTimeout(() => dispatch(hideMessages({ key, ids: notifications.map((v, k) => k) })), 10000)
    dispatch(showMessages({
      key,
      notifications
    }))
  }

const createGetShowAction = type => (key, autohide, buttons) =>
  messages => getShowMessagesAction(key, autohide, createNotifications(messages, type, key, buttons))

export const getShowAction = (key, autohide, type) => createGetShowAction(type)(key, autohide)
export const getShowErrorsAction = createGetShowAction(notificationTypesEnum.error)
export const getShowInfosAction = createGetShowAction(notificationTypesEnum.info)
export const getShowWarningAction = createGetShowAction(notificationTypesEnum.warning)

export const initialState = Map()

const update = (id, updateFunc) =>
  Array.isArray(id)
    ? updateFunc(id)
    : updateFunc(id.split('.'))

export default handleActions({
  // Disabled //
  [RESET_ALL_MESSAGES]: (state, { payload }) =>
    Map(),
  [SHOW_MESSAGE]: (state, { payload }) =>
    state.setIn(payload.id, payload.message),
  [KEEP_OPEN_MESSAGE]: (state, { payload }) =>
    update(payload.id, key => state.mergeIn(key, { keepOpen: true })),
  [SHOW_MESSAGES]: (state, { payload }) => {
    return state.mergeIn([payload.key], payload.notifications)
  },
  [HIDE_MESSAGE]: (state, { payload : { id } }) =>
    update(id, key => state.deleteIn(key)),
  [HIDE_MESSAGES]: (state, { payload : { key, ids } }) =>
    state.update(key, n => n.filter((x, id) => !ids.get(id) || x.get('keepOpen'))),
  [HIDE_MESSAGE_BY_TYPE]: (state, { payload : { key, type } }) =>
    state.update(key, m => m.filter(x => x.get('type') !== type))
}, initialState)
