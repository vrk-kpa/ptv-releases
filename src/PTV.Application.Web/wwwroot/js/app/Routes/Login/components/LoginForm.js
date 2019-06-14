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
import { Field, reduxForm } from 'redux-form/immutable'
import { compose } from 'redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { Spinner, Label, Button } from 'sema-ui-components'
import { handleOnSubmit } from 'util/redux-form/util'

import messages from '../messages'
import { loadEnums, deleteUserInfo } from 'actions/init'
import { RenderTextField, RenderRadioButtonGroup } from 'util/redux-form/renders'
import { ptvCookieTokenName, pahaCookieTokenName } from 'enums'
import { setCookie, deleteCookie, getPahaAuthToken, isBasicLoginEnabled } from 'Configuration/AppHelpers'
import jwt_decode from 'jwt-decode'
import { connect } from 'react-redux'
import { Organization } from 'util/redux-form/fields'
import { createSelector } from 'reselect'
import { getUserAccessRightsGroupTypesObjectArray } from 'selectors/common'
import { localizeList, languageTranslationTypes } from 'appComponents/Localize'
import styles from '../styles.scss'

const isStandardBasicForm = isBasicLoginEnabled()

const parseToken = () => {
  const pahaToken = getPahaAuthToken()
  return pahaToken && jwt_decode(pahaToken) || pahaToken
}

const UserAccessRightsGroup = compose(
  localizeList({
    input:'options',
    idAttribute: 'value',
    nameAttribute: 'label',
    languageTranslationType: languageTranslationTypes.locale
  }),
)(
  ({ options, label }) => (
    <div className={styles.accessRights}>
      <Field
        name='userAccessRightsGroup'
        component={RenderRadioButtonGroup}
        label={label}
        options={options}
        required
      />
    </div>
  )
)
const getOptions = createSelector(
  getUserAccessRightsGroupTypesObjectArray,
  userAccessRightsGroupTypes => userAccessRightsGroupTypes.map(option => ({
    label: option.name,
    value: option.id
  }))
)

const LoginForm = ({
  handleSubmit,
  intl: { formatMessage },
  submitting,
  isNameDisabled,
  options,
  valid
}) => {
  return (
    <form onSubmit={handleSubmit} className={styles.loginForm}>
      <p>{formatMessage(messages.welcomeMessage1)}</p>
      <p>{formatMessage(messages.welcomeMessage2)}</p>
      <p>{formatMessage(messages.welcomeMessage3)}</p>
      <h1 className={styles.formHeadline}>{formatMessage(messages.loginHeader)}</h1>
      <div className='mb-3'>
        <Field
          name='name'
          component={RenderTextField}
          label={formatMessage(messages.loginNameTitle)}
          labelPosition=''
          placeholder={formatMessage(messages.loginNamePlaceholder)}
          maxLength={100}
          required
          disabled={isNameDisabled}
          fieldClass='row'
          labelClass='col-md-4'
          inputClass='col-md-8'
        />
      </div>
      { isStandardBasicForm
        ? <div className='mb-3'>
          <Field
            name='password'
            component={RenderTextField}
            label={formatMessage(messages.loginPasswordTitle)}
            labelPosition=''
            placeholder={formatMessage(messages.loginPasswordPlaceholder)}
            maxLength={100}
            required
            type='password'
            fieldClass='row'
            labelClass='col-md-4'
            inputClass='col-md-8'
          />
        </div> : <div>
          <div className='mb-3'>
            <Organization
              required
              showAll
              labelPosition=''
              fieldClass='row'
              labelClass='col-md-4'
              inputClass='col-md-8'
            />
          </div>
          <div className='mb-3'>
            <UserAccessRightsGroup
              label={formatMessage(messages.userAccessRigthsGroup)}
              options={options}
            />
          </div>
        </div> }
      <div>
        <Button small
          disabled={!isStandardBasicForm && !valid}>
          {submitting && <Spinner inverse /> ||
            isStandardBasicForm
            ? formatMessage(messages.loginSubmitButton)
            : formatMessage(messages.loginSubmitCreateConfigurationButton)}
        </Button>
      </div>
    </form>
  )
}

const onSubmit = handleOnSubmit({
  url: 'auth/CreateConfiguration',
  withAuth: false
})

const onSubmitSuccess = (data, dispatch, props) => {
  setCookie(ptvCookieTokenName, data.token)
  dispatch(loadEnums())
  props.history.push(props.location.state &&
    props.location.state.from &&
    props.location.state.from.pathname || '/frontpage')
}

const onSubmitFail = (data, dispatch, props) => {
  deleteCookie(ptvCookieTokenName)
  deleteCookie(pahaCookieTokenName)
  deleteUserInfo()
  // props.history.push('/login')
}

LoginForm.propTypes = {
  handleSubmit: PropTypes.func,
  options: PropTypes.array,
  submitting: PropTypes.bool,
  isNameDisabled: PropTypes.bool,
  valid: PropTypes.bool,
  intl: intlShape.isRequired
}

const validate = values => {
  let errors = {}
  if (isStandardBasicForm) {
    if (!values.get('name')) {
      errors.name = 'Required'
    }

    if (!values.get('password')) {
      errors.password = 'Required'
    }

    return errors
  }

  if (!values.get('name')) {
    errors.name = 'Required'
  }

  return errors
}

export default compose(
  connect(
    (state, ownProps) => {
      const options = getOptions(state, ownProps)
      const token = parseToken()
      return {
        initialValues: {
          name: token && token.username || '',
          password: '',
          userAccessRightsGroup: options[0] && options[0].value
        },
        options,
        isNameDisabled: !!(token && token.username)
      }
    }
  ),
  reduxForm({
    form: 'loginForm',
    enableReinitialize: true,
    onSubmit,
    onSubmitSuccess,
    onSubmitFail,
    validate
  }),
  injectIntl
)(LoginForm)
