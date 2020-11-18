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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { injectIntl, intlShape, FormattedMessage } from 'util/react-intl'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withNotificationIcon from 'util/redux-form/HOC/withNotificationIcon'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { Button, Spinner } from 'sema-ui-components'
import {
  getAnnotationData,
  getAnnotationIsFetching,
  getHasAllRequiredAnnotationsFiedsFilled
} from './selectors'
import { FintoSchemas } from 'schemas'
import { messages } from './messages'
import { apiCall3 } from 'actions'
import { change } from 'redux-form/immutable'
import { List } from 'immutable'
import styles from './styles.scss'
import { property } from 'lodash'

const GetAnnotationButton = ({
  isFetching,
  intl: { formatMessage },
  fetchAnotations,
  isValid
}) => (
  <Button
    type='button'
    children={isFetching
      ? <Spinner />
      : formatMessage(messages.annotationButton)
    }
    onClick={fetchAnotations}
    secondary={isFetching}
    disabled={isValid}
    medium
  />
)
GetAnnotationButton.propTypes = {
  isFetching: PropTypes.bool,
  intl: intlShape,
  fetchAnotations: PropTypes.func,
  isValid: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  withFormStates,
  connect(
    (state, { formName, isDisabled }) => {
      const isValid = !getHasAllRequiredAnnotationsFiedsFilled(state, { formName })
      return {
        isFetching: getAnnotationIsFetching(state, { formName }),
        isValid,
        shouldShowNotificationIcon: isValid && !isDisabled,
        notificationText: <FormattedMessage {...messages.validationText} />,
        iconClass: styles.searchNotifyIcon
      }
    },
    (dispatch, { formName }) => {
      return {
        fetchAnotations: () => dispatch(({ getState, dispatch }) => {
          const state = getState()
          const data = getAnnotationData(state, { formName })
          dispatch(
            apiCall3({
              keys: ['OntologyTerm', 'searchAnnotations'],
              payload: {
                endpoint: 'common/getAnnotations',
                data: data
              },
              schemas: FintoSchemas.ANNOTATION_ONTOLOGY_TERMS,
              successCallback: (data, messages, original, store) => {
                const result = property('result.result.annotationOntologyTerms')(data)
                if (result) {
                  dispatch(
                    change(
                      formName,
                      'annotationTerms',
                      List(result)
                    )
                  )
                }
              },
              formName
            })
          )
        })
      }
    }
  ),
  withNotificationIcon
)(GetAnnotationButton)
