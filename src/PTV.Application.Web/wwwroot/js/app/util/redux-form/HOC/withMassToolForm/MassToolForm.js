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
import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { reduxForm, change } from 'redux-form/immutable'
import { clearMassRequestSent } from 'reducers/signalR'
import { formTypesEnum, massToolTypes, timingTypes, notificationEnums } from 'enums'
import withSignalRHub from 'util/redux-form/HOC/withSignalRHub'
// import MassDialog from 'util/redux-form/HOC/withMassDialog/MassDialog'
import { API_ROOT, getAuthToken } from 'Configuration/AppHelpers'
import {
  callMassPublish,
  callMassCopy,
  callMassArchive,
  cancelReview
} from './actions'
import { Map } from 'immutable'
import ReviewBar from './ReviewBar'
import withMassDialog from 'util/redux-form/HOC/withMassDialog'
import { branch } from 'recompose'
import { mergeInUIState } from 'reducers/ui'
import { getShowWarningAction } from 'reducers/notifications'

const MassToolForm = props => {
  return (
    <Fragment>
      <ReviewBar forwardedRef={props.forwardedRef} />
      {/* <MassDialog massToolType={massToolTypes.PUBLISH} /> */}
    </Fragment>
  )
}

MassToolForm.propTypes = {
  forwardedRef: PropTypes.oneOfType([PropTypes.func, PropTypes.object])
}

const progresInfoAction = getShowWarningAction(
  formTypesEnum.MASSTOOLFORM,
  false,
  notificationEnums.notificationButtonsEnum.toggle
)

const onSubmit = (values, dispatch, props) => {
  switch (values.get('type')) {
    case massToolTypes.PUBLISH:
      return dispatch(callMassPublish(values, props.hubConnection))
    case massToolTypes.ARCHIVE:
      return dispatch(callMassArchive(values, props.hubConnection))
    case massToolTypes.COPY:
      return dispatch(callMassCopy(values, props.hubConnection))
    default:
      return console.log('Type not handled')
  }
}

const onSubmitSuccess = (result, dispatch, props) => {
  // handleOnSubmitSuccess(result, dispatch, props, getService)
}

const onSubmitFail = (...args) => console.log(args)

export default compose(
  branch(() => getAuthToken(), withSignalRHub({ hubName: 'massTool',
    formName: formTypesEnum.MASSTOOLFORM,
    hubLink: new URL(API_ROOT).origin + '/massToolHub',
    actionDefinition: [{ type: 'ReceiveMessage',
      action: ({ dispatch }) => (message) => {
        dispatch(mergeInUIState({
          key: 'MassToolDialog',
          value: {
            isOpen: false
          }
        }))
        dispatch(mergeInUIState({
          key: 'MassToolBatchSelect',
          value: {
            checked: false
          }
        }))
        dispatch(clearMassRequestSent())
        dispatch(cancelReview())
        dispatch(change(formTypesEnum.MASSTOOLSELECTIONFORM, 'selected', Map()))
        if (message) {
          dispatch(progresInfoAction([ message ]))
        }
      }
    }]
  })),
  reduxForm({
    form: formTypesEnum.MASSTOOLFORM,
    initialValues: {
      timingType: timingTypes.NOW,
      type: massToolTypes.PUBLISH
    },
    destroyOnUnmount: false,
    onSubmit,
    onSubmitFail,
    onSubmitSuccess
  }),
  withMassDialog,
)(MassToolForm)
