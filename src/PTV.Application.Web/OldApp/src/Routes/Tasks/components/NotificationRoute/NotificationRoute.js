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
import React, { Component, createElement } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import ServiceChannelInCommonUse from 'Routes/Tasks/components/ServiceChannelInCommonUse'
import ContentUpdated from 'Routes/Tasks/components/ContentUpdated'
import GeneralDescriptionCreated from 'Routes/Tasks/components/GeneralDescriptionCreated'
import GeneralDescriptionUpdated from 'Routes/Tasks/components/GeneralDescriptionUpdated'
import ConnectionChanges from 'Routes/Tasks/components/ConnectionChanges'
import {
  getNotificationCount
} from 'Routes/Tasks/selectors/notifications'
import AccordionTitle from 'Routes/sharedComponents/AccordionTitle'
import AccordionHeader from 'Routes/sharedComponents/AccordionHeader'
import withState from 'util/withState'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { Accordion } from 'appComponents/Accordion'
import { notificationTypesEnum } from 'enums'
import styles from './styles.scss'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import SectionTitle from 'appComponents/SectionTitle'

const messages = defineMessages({
  serviceChannelInCommonUseTitle: {
    id: 'Routes.Tasks.Notification.ServiceChannelInCommonUse.Title',
    defaultMessage: 'Palvelut, joiden liitos yhteiskäyttöiseen kanavaan on poistettu omistajaorganisaation toimesta ({count})'
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
  connectionChangesTitle: {
    id: 'Routes.Tasks.Notification.ConnectionChanges.Title',
    defaultMessage: 'Liitoksissa tapahtuneet muutokset ({count})'
  },
  generalDescriptionSectionTitle: {
    id: 'Routes.Tasks.Notification.GeneralDescription.Section.Title',
    defaultMessage: 'Pohjakuvausten muutokset'
  },
  metaDataChangedSectionTitle: {
    id: 'Routes.Tasks.Notification.MetaDataChange.Section.Title',
    defaultMessage: 'Muuttuneet luokittelutiedot'
  },
  connectionSectionTitle: {
    id: 'Routes.Tasks.Notification.Connection.Section.Title',
    defaultMessage: 'Liitosten muutokset'
  }
})

const infoIconMessages = defineMessages({
  serviceChannelInCommonUse: {
    id: 'Routes.Tasks.Notification.ServiceChannelInCommonUse.InfoIcon',
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
  connectionChanges: {
    id: 'Routes.Tasks.Notification.ConnectionChanges.InfoIcon',
    defaultMessage: 'Info icon text'
  }
})

const NotificationAccordionTitle = compose(
  injectIntl,
  connect((state, ownProps) => ({
    count: getNotificationCount(state,
      { notificationType: ownProps.notificationType })
  })))(
  ({ label, count, tooltip }) => {
    return (
      <AccordionTitle
        label={label}
        count={count}
        tooltip={tooltip}
      />
    )
  })

const NotificationAccordionHeader =
  ({ onTabClick, notificationType, activeType, label, tooltip, contentComponent, columnWidths }) => {
    return (<AccordionHeader
      titleComponent={(<NotificationAccordionTitle
        notificationType={notificationType}
        label={label}
        tooltip={tooltip}
      />)}
      contentComponent={createElement(contentComponent, { activeType, notificationType, columnWidths })}
      active={activeType === notificationType}
      type={notificationType}
      onTabClick={onTabClick}
      contentClassName={styles.tasksAccContent}
      titleClassName={styles.tasksAccTitle}
      validate={false}
      arrowSize={40}
      stretchTitle
    />
    )
  }

NotificationAccordionHeader.propTypes = {
  onTabClick: PropTypes.func.isRequired,
  notificationType: PropTypes.string.isRequired,
  label: PropTypes.string.isRequired,
  activeType: PropTypes.string.isRequired,
  tooltip: PropTypes.string.isRequired,
  contentComponent: PropTypes.object,
  columnWidths: PropTypes.array
}

class NotificationRoute extends Component {
  handleTabOnChange = (_, { type }) => {
    const isAlreadOpened = this.props.activeType === type
    this.props.updateUI({
      activeType: isAlreadOpened
        ? null
        : type
    })
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
      activeType
    } = this.props
    return (
      <div className={styles.tasks}>
        <div className={styles.taskSection}>
          <SectionTitle title={messages.connectionSectionTitle} />
          <Accordion light activeIndex={-1} className={styles.stripe}>
            <NotificationAccordionHeader
              notificationType={notificationTypesEnum.CONNECTIONCHANGES}
              onTabClick={this.handleTabOnChange}
              label={messages.connectionChangesTitle}
              tooltip={infoIconMessages.connectionChanges}
              activeType={activeType}
              contentComponent={ConnectionChanges}
              columnWidths={['15%', '20%', '35%', '20%', '10%']}
            />
            <NotificationAccordionHeader
              notificationType={notificationTypesEnum.SERVICECHANNELINCOMMONUSE}
              onTabClick={this.handleTabOnChange}
              label={messages.serviceChannelInCommonUseTitle}
              tooltip={infoIconMessages.serviceChannelInCommonUse}
              activeType={activeType}
              contentComponent={ServiceChannelInCommonUse}
            />
          </Accordion>
        </div>
        <div className={styles.taskSection}>
          <SectionTitle title={messages.generalDescriptionSectionTitle} />
          <Accordion light activeIndex={-1} className={styles.stripe}>
            <NotificationAccordionHeader
              notificationType={notificationTypesEnum.GENERALDESCRIPTIONCREATED}
              onTabClick={this.handleTabOnChange}
              label={messages.newGeneralDescriptionTitle}
              tooltip={infoIconMessages.newGeneralDescription}
              activeType={activeType}
              contentComponent={GeneralDescriptionCreated}
            />
            <NotificationAccordionHeader
              notificationType={notificationTypesEnum.GENERALDESCRIPTIONUPDATED}
              onTabClick={this.handleTabOnChange}
              label={messages.generalDescriptionChangeTitle}
              tooltip={infoIconMessages.generalDescriptionChange}
              activeType={activeType}
              contentComponent={GeneralDescriptionUpdated}
            />
          </Accordion>
        </div>
        <div className={styles.taskSection}>
          <SectionTitle title={messages.metaDataChangedSectionTitle} />
          <Accordion light activeIndex={-1} className={styles.stripe}>
            <NotificationAccordionHeader
              notificationType={notificationTypesEnum.CONTENTUPDATED}
              onTabClick={this.handleTabOnChange}
              label={messages.entityChangeTitle}
              tooltip={infoIconMessages.entityChange}
              activeType={activeType}
              contentComponent={ContentUpdated}
            />
          </Accordion>
        </div>
      </div>
    )
  }
}
NotificationRoute.propTypes = {
  intl: intlShape,
  updateUI: PropTypes.func,
  activeType: PropTypes.func
}

export default compose(
  injectIntl,
  withState({
    redux: true,
    key: 'notificationType',
    initialState: {
      activeType: null
    }
  }),
  withBubbling()
)(NotificationRoute)
