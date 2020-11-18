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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import LanguagesTable from 'util/redux-form/fields/LanguagesTable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import styles from '../styles.scss'
import { getPublishColumnsDefinition } from './columns'
import { getFormValue } from 'selectors/base'
import { getFormInitialValues, change } from 'redux-form/immutable'
import {
  getPublishingStatusPublishedId,
  getPublishingStatusDeletedId
} from 'selectors/common'
import {
  getSelectedEntityId,
  getEntity,
  getPreviousInfoVersion
} from 'selectors/entities/entities'
import {
  formActions,
  formActionsTypesEnum,
  formEntityTypes,
  formTypesEnum
} from 'enums'
import { EntitySchemas } from 'schemas'
import { apiCall3 } from 'actions'
import { List } from 'immutable'
import { getShowErrorsAction } from 'reducers/notifications'
import { mergeInUIState } from 'reducers/ui'
import { getIsEntityAccessible } from './selectors'
import { getExpireOn } from 'selectors/common'
import { API_CALL_CLEAN } from 'actions'
import { canArchiveAstiEntity } from 'Routes/Service/selectors'

const formSchemas = {
  [formTypesEnum.SERVICEFORM]: EntitySchemas.SERVICE_FORM,
  [formTypesEnum.GENERALDESCRIPTIONFORM]: EntitySchemas.GENERAL_DESCRIPTION,
  [formTypesEnum.ORGANIZATIONFORM]: EntitySchemas.ORGANIZATION,
  [formTypesEnum.ELECTRONICCHANNELFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.PHONECHANNELFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.PRINTABLEFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.SERVICELOCATIONFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.WEBPAGEFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.GENERALDESCRIPTIONSEARCHFORM]: '/noschema',
  [formTypesEnum.SERVICECOLLECTIONFORM]: EntitySchemas.SERVICECOLLECTION
}

class PublishLanguage extends Component {
  componentDidMount () {
    this.props.clearUIState()
  }
  componentWillUnmount () {
    this.props.clearUIState()
  }
  render () {
    const {
      isArchived,
      publishStatusId,
      archiveStatusId,
      onSubmit,
      disableEdit,
      expireOn,
      canArchiveAsti,
      intl: { formatMessage }
    } = this.props
    return (
      <div className={styles.entityLanguageTable}>
        <LanguagesTable
          isArchived={isArchived}
          borderless
          noAuditing
          noConnections
          appendColumns={getPublishColumnsDefinition(formatMessage,
            publishStatusId,
            archiveStatusId,
            isArchived || disableEdit,
            onSubmit,
            expireOn,
            canArchiveAsti)} />
      </div>
    )
  }
}

PublishLanguage.propTypes = {
  isArchived: PropTypes.bool,
  intl: intlShape.isRequired,
  publishStatusId: PropTypes.any.isRequired,
  disableEdit: PropTypes.bool,
  expireOn: PropTypes.number,
  clearUIState: PropTypes.func,
  archiveStatusId: PropTypes.string,
  onSubmit: PropTypes.func,
  canArchiveAsti: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, { formName }) => {
    const entity = getEntity(state)
    const entityPS = entity.get('publishingStatus')
    const previousInfo = getPreviousInfoVersion(state)
    const publishStatusId = getPublishingStatusPublishedId(state)
    const archiveStatusId = getPublishingStatusDeletedId(state)
    const expireOn = getExpireOn(state)
    const modifiedExist = !!(publishStatusId === entityPS && previousInfo.get('lastModifiedId'))
    const isAccesible = getIsEntityAccessible(state, { formName })
    const canArchiveAsti = canArchiveAstiEntity(state)
    return {
      publishStatusId,
      disableEdit: modifiedExist || !entityPS || !isAccesible,
      formName,
      archiveStatusId,
      expireOn,
      canArchiveAsti
    }
  },
  (dispatch, { formName }) => ({
    onSubmit : (index, action) => dispatch(({ getState, dispatch }) => {
      const state = getState()
      const publishStatusId = getPublishingStatusPublishedId(state)
      const entityId = getSelectedEntityId(state)
      const languagesAvailabilities = getFormValue('languagesAvailabilities')(state, { formName }) || List()
      let languages = languagesAvailabilities.filter((_, langIndex) => langIndex === index)
      languages = languages.update(0, lang => lang.set('statusId', publishStatusId))
      const field = action === 'SchedulePublish' ? 'validFrom' : 'validTo'
      const errorAction = (data) => () => {
        if (data && data[0] && data[0].validationMessages !== null) {
          const initial = getFormInitialValues(formName)(state)
          const initialDateFrom = initial && initial.getIn(['languagesAvailabilities', index, field]) || null
          dispatch(change(formName, `languagesAvailabilities[${index}].${field}`, initialDateFrom))
        }
        dispatch(getShowErrorsAction(formName, true)(data))
      }
      const successAction = () => {
        dispatch({
          type: API_CALL_CLEAN,
          keys: ['entityHistory']
        })
      }
      dispatch(
        apiCall3({
          keys: [formEntityTypes[formName], 'publishEntity'],
          payload: {
            endpoint: formActions[formActionsTypesEnum.SCHEDULEENTITY][formName],
            data: { id: entityId, languagesAvailabilities:languages, publishAction:action }
          },
          schemas: formSchemas[formName],
          onErrorAction: errorAction,
          successNextAction: successAction,
          formName
        })
      )
    }),
    clearUIState: () => dispatch(
      mergeInUIState({
        key: formName + 'InputCell',
        value: { active:false }
      }))
  }))
)(PublishLanguage)
