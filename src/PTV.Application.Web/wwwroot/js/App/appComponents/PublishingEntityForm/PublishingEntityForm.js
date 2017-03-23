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

import React, { PureComponent, PropTypes } from 'react'
import { reduxForm } from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, defineMessages } from 'react-intl'
import { Button, SimpleTable } from 'sema-ui-components'
import LanguageVisibilityCell from './LanguageVisibilityCell'
import { getPublishingEntityRows,
  getAvailableLanguagesPair,
  getStepCommonIsFetching,
  getPublishingStatusPublished,
  getPublishingStatusModified } from 'Containers/Common/Selectors'
import { PTVPreloader } from 'Components'
import { keyToEntities } from 'Containers/Common/Enums'
// actions
import mapDispatchToProps from 'Configuration/MapDispatchToProps'
import { getServiceNames, publishService } from 'Containers/Services/Service/Actions'
import { getChannelNames, publishChannel } from 'Containers/Channels/Common/Actions'
import { getOrganizationNames, publishOrganization } from 'Containers/Manage/Organizations/Organization/Actions'

const formSetup = {
  services: {
    entitiesType: 'services',
    action: 'getServiceNames',
    publishAction: 'publishService'
  },
  channels: {
    entitiesType: 'channels',
    action: 'getChannelNames',
    publishAction: 'publishChannel'
  },
  organizations: {
    entitiesType: 'organizations',
    action: 'getOrganizationNames',
    publishAction: 'publishOrganization'
  }
}

const messages = defineMessages({
  tableHeaderName: {
    id: 'Components.PublishingEntityForm.Header.Name',
    defaultMessage: 'Nimi'
  },
  tableHeaderLanguage: {
    id: 'Components.PublishingEntityForm.Header.Language',
    defaultMessage: 'Kieli'
  },
  tableHeaderVisibility: {
    id: 'Components.PublishingEntityForm.Header.Visibility',
    defaultMessage: 'Näkyvyys'
  },
  submitTitle: {
    id: 'Components.PublishingEntityForm.Button.Submit.Title'/* "Components.Buttons.PublishButton" */,
    defaultMessage: 'Julkaise'
  },
  closeDialogButtonTitle: {
    id: 'Components.PublishingEntityForm.Button.Close.Title',
    defaultMessage: 'Peruuta'
  }
})

const getColumnDefinition = formatMessage => {
  return [
    {
      property: 'name',
      header: {
        label: formatMessage(messages.tableHeaderName)
      }
    },
    {
      property: 'language',
      header: {
        label: formatMessage(messages.tableHeaderLanguage)
      }
    },
    {
      property: 'id',
      header: {
        label: formatMessage(messages.tableHeaderVisibility)
      },
      cell: {
        formatters: [
          id => <LanguageVisibilityCell id={id} />
        ]
      }
    }
  ]
}


class PublishingEntityForm extends PureComponent {
  componentWillMount () {
    this.props.actions[formSetup[keyToEntities[this.props.type]].action](this.props.id, this.props.type)
  }

  render () {
    const {
      rows,
      onCancel,
      isLoading,
      handleSubmit,
      invalid ,
      submitting,
      intl: { formatMessage }
    } = this.props
    return (
      isLoading ? <PTVPreloader />
        : <form onSubmit={handleSubmit}>
        <SimpleTable columns={getColumnDefinition(formatMessage)}>
          <SimpleTable.Header />
          <SimpleTable.Body rows={rows} rowKey='id' />
        </SimpleTable>
        <div className='btn-group end'>
          <Button secondary small onClick={onCancel}>{formatMessage(messages.closeDialogButtonTitle)}</Button>
          <Button small disabled={ submitting || invalid }
            type='submit'>{submitting ? <PTVPreloader small /> : formatMessage(messages.submitTitle)}</Button>
        </div>
      </form>
    )
  }
}

const onSubmit = (formValues, dispatch, ownProps) => {
  return new Promise((resolve, reject) => {
    ownProps.actions[formSetup[keyToEntities[ownProps.type]].publishAction](ownProps.id, formValues.map(v => {
      if (v === ownProps.publishingStatusModified) {
        return ownProps.publishingStatusPublished
      }
      return v
    }), ownProps.type)
    resolve()
  }).then(() =>
      ownProps.onCancel()
    )
}

const validate = (values, ownProps) => {
  const errors = {}
  let isValid = false
  values.map((value, key) => {
    if (value !== ownProps.publishingStatusModified && value !== ownProps.publishingStatusPublished) {
      errors[key] = 'isNeededAny'
    } else {
      isValid = true
    }
  })

  return isValid ? {} : errors
}

PublishingEntityForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  rows: PropTypes.array,
  id: PropTypes.string.isRequired,
  actions: PropTypes.object.isRequired,
  intl: PropTypes.object.isRequired,
  onCancel: PropTypes.func.isRequired,
  type: PropTypes.string.isRequired,
  isLoading: PropTypes.bool.isRequired,
  isValid: PropTypes.bool.isRequired,
  submitting: PropTypes.bool.isRequired
}
export default compose(
  connect((state, ownProps) => ({
    rows: getPublishingEntityRows(state, { entitiesType: formSetup[keyToEntities[ownProps.type]].entitiesType, ...ownProps }),
    isLoading: getStepCommonIsFetching(state, { keyToState: ownProps.type }),
    publishingStatusModified: getPublishingStatusModified(state).get('id'),
    publishingStatusPublished: getPublishingStatusPublished(state).get('id'),
    initialValues: getAvailableLanguagesPair(state, { entitiesType: formSetup[keyToEntities[ownProps.type]].entitiesType, ...ownProps })
  }),
  mapDispatchToProps([{ getServiceNames, publishService, getChannelNames, publishChannel, getOrganizationNames, publishOrganization }])),
  reduxForm({
    form: 'publishingEntityForm',
    enableReinitialize: true,
    onSubmit,
    validate
  }),
  injectIntl
)(PublishingEntityForm)
