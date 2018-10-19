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
import ServiceChannelInCommonUse from 'Routes/Tasks/components/ServiceChannelInCommonUse'
import ContentUpdated from 'Routes/Tasks/components/ContentUpdated'
import ContentArchived from 'Routes/Tasks/components/ContentArchived'
import TranslationArrived from 'Routes/Tasks/components/TranslationArrived'
import GeneralDescriptionCreated from 'Routes/Tasks/components/GeneralDescriptionCreated'
import GeneralDescriptionUpdated from 'Routes/Tasks/components/GeneralDescriptionUpdated'
import {
  getServiceChannelInCommonUseCount,
  getContentUpdatedCount,
  getContentArchivedCount,
  getTranslationArrivedCount,
  getGeneralDescriptionCreatedCount,
  getGeneralDescriptionUpdatedCount
} from 'Routes/Tasks/selectors/notifications'
import withState from 'util/withState'
import { apiCall3 } from 'actions'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { Accordion } from 'appComponents/Accordion'
import { NotificationSchemas } from 'schemas/notifications'
import { notificationTypesEnum } from 'enums'
import styles from './styles.scss'
import NotificationAccordionTitle from './NotificationAccordionTitle'
import { API_CALL_CLEAN } from 'Containers/Common/Actions'

const messages = defineMessages({
  serviceChannelInCommonUseTitle: {
    id: 'Routes.Tasks.Notification.ServiceChannelInCommonUse.Title',
    defaultMessage: 'Palvelut, joiden liitos yhteiskäyttöiseen kanavaan on poistettu omistajaorganisaation toimesta ({count})'
  },
  entityArchiveTitle: {
    id: 'Routes.Tasks.Notification.ContentArchived.Title',
    defaultMessage: 'Arkistoidut sisällöt ({count})'
  },
  entityChangeTitle: {
    id: 'Routes.Tasks.Notification.ContentUpdated.Title',
    defaultMessage: 'Kanavat, joiden metatietoja on muutettu ({count})'
  },
  generalDescriptionChangeTitle: {
    id: 'Routes.Tasks.Notification.GeneralDescriptionUpdated.Title',
    defaultMessage: 'Muuttuneet pohjakuvaukset ({count})'
  },
  newGeneralDescriptionTitle: {
    id: 'Routes.Tasks.Notification.GeneralDescriptionCreated.Title',
    defaultMessage: 'Uudet pohjakuvaukset ({count})'
  },
  translationArrivedTitle: {
    id: 'Routes.Tasks.Notification.TranslationArrived.Title',
    defaultMessage: 'Saapuneet käännöstilaukset ({count})'
  }
})

const infoIconMessages = defineMessages({
  serviceChannelInCommonUse: {
    id: 'Routes.Tasks.Notification.ServiceChannelInCommonUse.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  entityArchive: {
    id: 'Routes.Tasks.Notification.ContentArchived.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  entityChange: {
    id: 'Routes.Tasks.Notification.ContentUpdated.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  generalDescriptionChange: {
    id: 'Routes.Tasks.Notification.GeneralDescriptionUpdated.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  newGeneralDescription: {
    id: 'Routes.Tasks.Notification.GeneralDescriptionCreated.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  translationArrived: {
    id: 'Routes.Tasks.Notification.TranslationArrived.InfoIcon',
    defaultMessage: 'Info icon text'
  }
})

class NotificationRoute extends Component {
  handleTabOnChange = (_, index) => {
    const isAlreadOpened = this.props.activeIndex === index
    this.props.updateUI({
      activeIndex: isAlreadOpened
        ? -1
        : index
    })
    if (!isAlreadOpened) {
      const clickedTabName = [
        notificationTypesEnum.SERVICECHANNELINCOMMONUSE,
        notificationTypesEnum.CONTENTUPDATED,
        notificationTypesEnum.CONTENTARCHIVED,
        notificationTypesEnum.TRANSLATIONARRIVED,
        notificationTypesEnum.GENERALDESCRIPTIONCREATED,
        notificationTypesEnum.GENERALDESCRIPTIONUPDATED
      ][index]
      this.props.loadTabByName(clickedTabName)
    }
  }
  renderAccordionTitle = ({ label, count }) => {
    return (
      <div className={styles.tasksTitle}>
        <div>{this.props.intl.formatMessage(
          label,
          { count }
        )}</div>
      </div>
    )
  }
  render () {
    const {
      intl: { formatMessage },
      activeIndex,
      serviceChannelInCommonUseCount,
      contentUpdatedCount,
      contentArchivedCount,
      translationArrivedCount,
      generalDescriptionCreatedCount,
      generalDescriptionUpdatedCount
    } = this.props
    return (
      <div className={styles.tasks}>
        <Accordion light activeIndex={activeIndex}>
          <Accordion.Title
            title={(
              <NotificationAccordionTitle
                label={messages.serviceChannelInCommonUseTitle}
                count={serviceChannelInCommonUseCount}
                tooltip={infoIconMessages.serviceChannelInCommonUse}
              />
            )}
            validate={false}
            onClick={this.handleTabOnChange}
            className={styles.tasksAccTitle}
            arrowSize={40}
          />
          <Accordion.Content className={styles.tasksAccContent}>
            <ServiceChannelInCommonUse />
          </Accordion.Content>

          <Accordion.Title
            title={(
              <NotificationAccordionTitle
                label={messages.entityChangeTitle}
                count={contentUpdatedCount}
                tooltip={infoIconMessages.entityChange}
              />
            )}
            validate={false}
            onClick={this.handleTabOnChange}
            className={styles.tasksAccTitle}
            arrowSize={40}
          />
          <Accordion.Content className={styles.tasksAccContent}>
            <ContentUpdated />
          </Accordion.Content>

          <Accordion.Title
            title={(
              <NotificationAccordionTitle
                label={messages.entityArchiveTitle}
                count={contentArchivedCount}
                tooltip={infoIconMessages.entityArchive}
              />
            )}
            validate={false}
            onClick={this.handleTabOnChange}
            className={styles.tasksAccTitle}
            arrowSize={40}
          />
          <Accordion.Content className={styles.tasksAccContent}>
            <ContentArchived />
          </Accordion.Content>

          <Accordion.Title
            title={(
              <NotificationAccordionTitle
                label={messages.translationArrivedTitle}
                count={translationArrivedCount}
                tooltip={infoIconMessages.translationArrived}
              />
            )}
            validate={false}
            onClick={this.handleTabOnChange}
            className={styles.tasksAccTitle}
            arrowSize={40}
          />
          <Accordion.Content className={styles.tasksAccContent}>
            <TranslationArrived />
          </Accordion.Content>

          <Accordion.Title
            title={(
              <NotificationAccordionTitle
                label={messages.newGeneralDescriptionTitle}
                count={generalDescriptionCreatedCount}
                tooltip={infoIconMessages.newGeneralDescription}
              />
            )}
            validate={false}
            onClick={this.handleTabOnChange}
            className={styles.tasksAccTitle}
            arrowSize={40}
          />
          <Accordion.Content className={styles.tasksAccContent}>
            <GeneralDescriptionCreated />
          </Accordion.Content>

          <Accordion.Title
            title={(
              <NotificationAccordionTitle
                label={messages.generalDescriptionChangeTitle}
                count={generalDescriptionUpdatedCount}
                tooltip={infoIconMessages.generalDescriptionChange}
              />
            )}
            validate={false}
            onClick={this.handleTabOnChange}
            className={styles.tasksAccTitle}
            arrowSize={40}
          />
          <Accordion.Content className={styles.tasksAccContent}>
            <GeneralDescriptionUpdated />
          </Accordion.Content>
        </Accordion>
      </div>
    )
  }
}
NotificationRoute.propTypes = {
  intl: intlShape,
  activeIndex: PropTypes.number,
  updateUI: PropTypes.func,
  loadTabByName: PropTypes.func,
  serviceChannelInCommonUseCount: PropTypes.number,
  contentUpdatedCount: PropTypes.number,
  contentArchivedCount: PropTypes.number,
  translationArrivedCount: PropTypes.number,
  generalDescriptionCreatedCount: PropTypes.number,
  generalDescriptionUpdatedCount: PropTypes.number
}

export default compose(
  connect(
    state => {
      return {
        serviceChannelInCommonUseCount: getServiceChannelInCommonUseCount(state),
        contentUpdatedCount: getContentUpdatedCount(state),
        contentArchivedCount: getContentArchivedCount(state),
        translationArrivedCount: getTranslationArrivedCount(state),
        generalDescriptionCreatedCount: getGeneralDescriptionCreatedCount(state),
        generalDescriptionUpdatedCount: getGeneralDescriptionUpdatedCount(state)

      }
    }, {
      loadTabByName: name => ({ dispatch }) => {
        dispatch({
          type: API_CALL_CLEAN,
          keys: [
            'notifications',
            name,
            'load',
            'prevEntities'
          ]
        })
        dispatch(
          apiCall3({
            keys: ['notifications', name, 'load'],
            payload: {
              endpoint: `notifications/${name}`,
              data: {}
            },
            saveRequestData: true,
            schemas: NotificationSchemas.NOTIFICATION_GROUP
          })
        )
      }
    }
  ),
  withState({
    initialState: {
      activeIndex: -1
    }
  }),
  injectIntl
)(NotificationRoute)
