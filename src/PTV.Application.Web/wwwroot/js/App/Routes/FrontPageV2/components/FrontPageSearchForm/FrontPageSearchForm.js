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
import { reduxForm, getFormValues } from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import Divider from 'Components/Divider/Divider'
import { getSelectedPublishingStatusesIds,
  getUserOrganization,
  getFrontPageSearchFormIsFetching,
  getTranslatableLanguagesCodeArray
} from 'Routes/FrontPageV2/selectors'
import { Button } from 'sema-ui-components'
import { getSearchDomain } from 'Containers/Common/Selectors'
import { PTVPreloader } from 'Components'
import { defineMessages, injectIntl } from 'react-intl'

const messages = defineMessages({
  buttonSearch: {
    id: 'Components.Button.Search.Title',
    defaultMessage: 'Hae'
  }
})

import {
  CommonSearchFields,
  ServiceSearchFields,
  GeneralDescriptionSearchFields,
  OrganizationSearchFields
} from './partial'
import {
  fetchServices,
  fetchChannels,
  fetchGeneralDescriptions,
  fetchOrganizations,
  untouchAll
} from 'Routes/FrontPageV2/actions'

const FrontPageSearchForm = ({
  handleSubmit,
  searchDomain,
  searchFormIsFetching,
  intl: { formatMessage }
}) => {
  let domainSearchFields = null
  switch (searchDomain) {
    case 'services':
      domainSearchFields = <ServiceSearchFields />
      break
    case 'generalDescriptions':
      domainSearchFields = <GeneralDescriptionSearchFields />
      break
    case 'organizations':
      domainSearchFields = <OrganizationSearchFields />
      break
  }
  return (
    <form onSubmit={handleSubmit} className='ptv-toolbox-container'>
      {searchFormIsFetching ? <PTVPreloader />
      : <div>
        <CommonSearchFields />
        <Divider />
        {domainSearchFields}
        {(searchDomain === 'services' || searchDomain === 'generalDescriptions') && <Divider />}
        <div className='form-group'>
          <Button
            small
            type='submit'
            children={formatMessage(messages.buttonSearch)}
          />
        </div>
      </div>
      }
    </form>
  )
}
FrontPageSearchForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  searchFormIsFetching: PropTypes.bool.isRequired,
  searchDomain: PropTypes.string.isRequired,
  intl: PropTypes.object.isRequired
}

const onSubmit = (_, dispatch, { formValues, searchDomain }) => {
  switch (searchDomain) {
    case 'services':
      dispatch(fetchServices(formValues.toJS()))
      break
    case 'eChannel':
    case 'webPage':
    case 'printableForm':
    case 'phone':
    case 'serviceLocation':
      dispatch(fetchChannels({
        ...formValues.toJS(),
        channelType: searchDomain
      }))
      break
    case 'generalDescriptions':
      dispatch(fetchGeneralDescriptions(formValues.toJS()))
      break
    case 'organizations':
      dispatch(fetchOrganizations(formValues.toJS()))
      break
  }
  dispatch(untouchAll())
}

export default compose(
  injectIntl,
  connect(state => ({
    searchFormIsFetching: getFrontPageSearchFormIsFetching(state),
    initialValues: {
      selectedPublishingStatuses: getSelectedPublishingStatusesIds(state),
      organizationId: getUserOrganization(state),
      languages: getTranslatableLanguagesCodeArray(state)
    },
    searchDomain: getSearchDomain(state),
    formValues:getFormValues('frontPageSearch')(state)
  })),
  reduxForm({
    form: 'frontPageSearch',
    destroyOnUnmount : false,
    enableReinitialize: true,
    onSubmit
  })
)(FrontPageSearchForm)
