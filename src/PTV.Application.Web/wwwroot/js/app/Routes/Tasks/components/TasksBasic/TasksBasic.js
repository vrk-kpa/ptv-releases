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
import React, { PureComponent, createElement } from 'react'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { Accordion } from 'appComponents/Accordion'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { defineMessages, injectIntl } from 'util/react-intl'
import { taskTypes, taskTypesEnum, formTypesEnum } from 'enums'
import { mergeInUIState } from 'reducers/ui'
import styles from './styles.scss'
import {
  getTasksTypeCount,
  getNumbersIsFetching,
  getIsPostponeButtonVisible,
  getPostponeIsFetching
} from '../../selectors'
import { TasksSchemas } from 'schemas/tasks'
import { EntitySchemas } from 'schemas'
import { postponeTasksEntities, toggleBrokenLinksAccordion } from 'Routes/Tasks/actions'
import AccordionTitle from 'Routes/sharedComponents/AccordionTitle'
import SectionTitle from 'appComponents/SectionTitle'
import AccordionHeader from 'Routes/sharedComponents/AccordionHeader'
import TranslationArrived from 'Routes/Tasks/components/TranslationArrived'
import TranslationInProgress from 'Routes/Tasks/components/TranslationInProgress'
import ChannelsWithoutServices from 'Routes/Tasks/components/ChannelsWithoutServices'
import ServicesWithoutChannels from 'Routes/Tasks/components/ServicesWithoutChannels'
import OutdatedDraftChannels from 'Routes/Tasks/components/OutdatedDraftChannels'
import OutdatedDraftServices from 'Routes/Tasks/components/OutdatedDraftServices'
import OutdatedPublishedChannels from 'Routes/Tasks/components/OutdatedPublishedChannels'
import OutdatedPublishedServices from 'Routes/Tasks/components/OutdatedPublishedServices'
import MissingLanguageOrganizations from 'Routes/Tasks/components/MissingLanguageOrganizations'
import MassToolLink from 'Routes/Search/components/MassTool/MassToolLink'
import MassToolSelectionForm from 'Routes/Search/components/MassTool/MassToolSelectionForm'
import MassToolSelectionTasksContent from '../MassToolSelectionTasksContent'
import TimedPublishFailed from 'Routes/Tasks/components/TimedPublishFailed'
import LinksUnstable from 'Routes/Tasks/components/LinksUnstable'
import LinksException from 'Routes/Tasks/components/LinksException'
import withState from 'util/withState'
import withDirtyFormCheck from 'util/redux-form/HOC/withDirtyFormCheck'

const messages = defineMessages({
  outdatedDraftServices: {
    id: 'Routes.Tasks.OutdatedDraftServices.Main.Title',
    defaultMessage: 'Organisaatiosi vanhentuvat luonnostilaiset palvelut ({count})'
  },
  outdatedPublishedServices: {
    id: 'Routes.Tasks.OutdatedPublishedServices.Main.Title',
    defaultMessage: 'Organisaatiosi vanhentuvat julkaistut palvelut ({count})'
  },
  servicesWithoutChannels: {
    id: 'Routes.Tasks.ServicesWithoutChannels.Main.Title',
    defaultMessage: 'Organisaatiosi palvelut, joihin ei ole liitetty yhtään asiointikanavaa ({count})'
  },
  outdatedDraftChannels: {
    id: 'Routes.Tasks.OutdatedDraftChannels.Main.Title',
    defaultMessage: 'Organisaatiosi vanhentuvat luonnostilaiset kanavat ({count})'
  },
  outdatedPublishedChannels: {
    id: 'Routes.Tasks.OutdatedPublishedChannels.Main.Title',
    defaultMessage: 'Organisaatiosi vanhetuvat julkaistut kanavat ({count})'
  },
  channelsWithoutServices: {
    id: 'Routes.Tasks.ChannelsNotConnectedToAnyService.Main.Title',
    defaultMessage: 'Organisaatiosi asiointikanavat, joihin ei ole liitetty yhtään palvelua ({count})'
  },
  postponeButtonTitle: {
    id: 'Routes.Tasks.Accordion.PostponeButton.Title',
    defaultMessage: 'Lykkää tehtävät'
  },
  translationArrivedTitle: {
    id: 'Routes.Tasks.Notification.TranslationArrived.Title',
    defaultMessage: 'Saapuneet käännöstilaukset ({count})'
  },
  translationInProgressTitle: {
    id: 'Routes.Tasks.Notification.TranslationInProgress.Title',
    defaultMessage: 'Keskeneräiset käännöstilaukset ({count})'
  },
  missingLangugageOrganizationsTitle: {
    id: 'Routes.Tasks.MissingLangugageOrganizations.Title',
    defaultMessage: 'Organisaatiokuvauksen kieliversioita puuttuu ({count})'
  },
  outdatedInformationSection: {
    id: 'Tasks.OutdatedInformation.Title',
    defaultMessage: 'Vanhentuvat tiedot'
  },
  missingConnectionsSection: {
    id: 'Tasks.MissingConnections.Title',
    defaultMessage: 'Puuttuvat liitokset'
  },
  translationsSection: {
    id: 'Tasks.Translations.Title',
    defaultMessage: 'Käännökset'
  },
  missingInformationSection: {
    id: 'Tasks.MissingInformation.Title',
    defaultMessage: 'Puuttuvat tiedot'
  },
  timedPublishFailedTitle: {
    id: 'Routes.Tasks.TimedPublishFailed.Title',
    defaultMessage: 'Ajastettu julkaisu epäonnistui, koska pakollisia tietoja puuttuu ({count})'
  },
  brokenLinksSection: {
    id: 'Tasks.BrokenLinks.Title',
    defaultMessage: 'Linkkitarkistuksen nostamat linkit'
  },
  unstableLinks: {
    id: 'Routes.Tasks.UnstableLinks.Title',
    defaultMessage: 'Organisaatiosi linkkitarkistuksen nostamat linkit ({count})'
  },
  exceptionLinks: {
    id: 'Routes.Tasks.ExceptionLinks.Title',
    defaultMessage: 'Organisaatiosi poikkeustilaset linkit ({count})'
  }
})

const infoIconMessages = defineMessages({
  outdatedDraftServices: {
    id: 'Routes.Tasks.OutdatedDraftServices.Main.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  outdatedPublishedServices: {
    id: 'Routes.Tasks.OutdatedPublishedServices.Main.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  servicesWithoutChannels: {
    id: 'Routes.Tasks.ServicesWithoutChannels.Main.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  outdatedDraftChannels: {
    id: 'Routes.Tasks.OutdatedDraftChannels.Main.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  outdatedPublishedChannels: {
    id: 'Routes.Tasks.OutdatedPublishedChannels.Main.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  channelsWithoutServices: {
    id: 'Routes.Tasks.ChannelsNotConnectedToAnyService.Main.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  translationArrived: {
    id: 'Routes.Tasks.Notification.TranslationArrived.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  translationInProgress: {
    id: 'Routes.Tasks.Notification.TranslationInProgress.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  missingLangugageOrganizations: {
    id: 'Routes.Tasks.MissingLangugageOrganizations.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  timedPublishFailed: {
    id: 'Routes.Tasks.TimedPublishFailed.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  unstableLinks: {
    id: 'Routes.Tasks.UnstableLinks.InfoIcon',
    defaultMessage: 'Linkintarkistus tarkistaa kaikkien organisaatiosi sisällöissä olevien linkkien toimivuuden kerran vuorokaudessa. Tässä listassa näet linkit, jotka eivät ole toimineet linkintarkistuksen aikana. Käy läpi linkit ja korjaa rikkinäiset. Jos linkki toimii, sinun ei tarvitse tehdä sille mitään, vaan se poistuu listalta seuraavan päivän tarkistuksessa.'
  },
  exceptionLinks: {
    id: 'Routes.Tasks.ExceptionLinks.InfoIcon',
    defaultMessage: 'Näet listalla organisaatiosi poikkeukseksi merkityt linkit. Poikkeustilaiset linkit eivät enää nouse epävakaiden listaan. Muista tarkistaa säännöllisesti, toimivatko poikkeustilaiset linkit edelleen.'
  }
})

const getSchema = (taskType) => {
  let schema = EntitySchemas.SERVICE

  switch (taskType) {
    case taskTypesEnum.OUTDATEDDRAFTSERVICES:
    case taskTypesEnum.OUTDATEDPUBLISHEDSERVICES:
    case taskTypesEnum.CHANNELSWITHOUTSERVICES:
      schema = EntitySchemas.SERVICE
      break
    case taskTypesEnum.OUTDATEDDRAFTCHANNELS:
    case taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS:
    case taskTypesEnum.SERVICESWITHOUTCHANNELS:
      schema = EntitySchemas.CHANNEL
      break
    case taskTypesEnum.MISSINGLANGUAGEORGANIZATIONS:
      schema = EntitySchemas.ORGANIZATION
  }

  return schema
}

const TaskAccordionTitle = compose(
  injectIntl,
  connect((state, ownProps) => ({
    isPostponing: getPostponeIsFetching(state, { taskType: ownProps.taskType }),
    count: getTasksTypeCount(state,
      { taskType: taskTypes[ownProps.taskType] }),
    isButtonVisible: getIsPostponeButtonVisible(state,
      { taskType: taskTypes[ownProps.taskType] })
  }), { postponeTasksEntities }))(
  ({ label, count, tooltip, isPostponing, postponeTasksEntities, taskType, isButtonVisible }) => {
    const handleClick = (e) => {
      postponeTasksEntities(taskType,
        TasksSchemas.GET_ENTITIES(getSchema(taskType)))
    }

    return (
      <AccordionTitle
        label={label}
        count={count}
        tooltip={tooltip}
        isButtonVisible={isButtonVisible}
        isButtonLoading={isPostponing}
        isButtonDisabled={isPostponing}
        onButtonClick={handleClick}
        buttonTitle={messages.postponeButtonTitle}
      />
    )
  })

const TaskAccordionHeader =
  ({ onTabClick, taskType, activeType, label, tooltip, contentComponent, columnWidths }) => {
    const isActive = Array.isArray(activeType)
      ? activeType.includes(taskType)
      : activeType === taskType
    return (<AccordionHeader
      titleComponent={(<TaskAccordionTitle
        taskType={taskType}
        label={label}
        tooltip={tooltip}
      />)}
      contentComponent={createElement(contentComponent, { activeType, taskType, columnWidths })}
      active={isActive}
      validate={false}
      arrowSize={40}
      type={taskType}
      keyProp={taskType}
      onTabClick={onTabClick}
      stretchTitle
      contentClassName={styles.tasksAccContent}
      titleClassName={styles.tasksAccTitle}
    />
    )
  }

TaskAccordionHeader.propTypes = {
  onTabClick: PropTypes.func.isRequired,
  taskType: PropTypes.string.isRequired,
  label: PropTypes.object.isRequired,
  activeType: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.array
  ]),
  tooltip: PropTypes.object,
  contentComponent: PropTypes.any,
  columnWidths: PropTypes.array
}

class TasksBasic extends PureComponent {
  handleTabOnChange = (_, { type }) => {
    const indices = this.props.activeTypeOutdatedInfo
    const index = indices.findIndex(item => item === type)
    this.props.updateUI({
      activeTypeOutdatedInfo: index > -1
        ? [...indices.slice(0, index), ...indices.slice(index + 1)]
        : [...indices, type]
    })
  }

  handleMissingConnectionTabOnChange = (_, { type }) => {
    const isAlreadyOpen = this.props.activeTypeMissingConnections === type
    this.props.updateUI({
      activeTypeMissingConnections: isAlreadyOpen
        ? null
        : type
    })
  }

  handleBrokenLinksTabOnChange = (_, { type }) => {
    const indices = this.props.activeTypeBrokenLinks
    const index = indices.findIndex(item => item === type)
    const checkedForm = type === taskTypesEnum.UNSTABLELINKS
      ? formTypesEnum.UNSTABLELINKFORM
      : formTypesEnum.EXCEPTIONLINKFORM
    this.props.dirtyFormsMap.get(checkedForm)
      ? this.props.mergeInUIState({
        key: 'clearFormDialog',
        value: {
          isOpen: true,
          confirmAction: () => this.props.toggleBrokenLinksAccordion(index, indices, type),
          forms: [ checkedForm ]
        }
      })
      : this.props.toggleBrokenLinksAccordion(index, indices, type)
  }

  handleTranslationsTabOnChange = (_, { type }) => {
    const isAlreadyOpen = this.props.activeTypeTranslations === type
    this.props.updateUI({
      activeTypeTranslations: isAlreadyOpen
        ? null
        : type
    })
  }

  handleMissingInformationTabOnChange = (_, { type }) => {
    const isAlreadyOpen = this.props.activeTypeMissingInfo === type
    this.props.updateUI({
      activeTypeMissingInfo: isAlreadyOpen
        ? null
        : type
    })
  }

  render () {
    const {
      activeTypeOutdatedInfo,
      activeTypeMissingConnections,
      activeTypeTranslations,
      activeTypeMissingInfo,
      activeTypeBrokenLinks,
      timedPublishFailedCount
    } = this.props
    return (
      <div className={styles.tasks}>
        <div className={styles.taskSection}>
          <SectionTitle title={messages.outdatedInformationSection}>
            <MassToolLink />
          </SectionTitle>
          <MassToolSelectionForm
            content={MassToolSelectionTasksContent}
            className={styles.selectionForm}
          />
          <Accordion light activeIndex={-1} className={styles.stripe} isExclusive={false}>
            <TaskAccordionHeader
              taskType={taskTypesEnum.OUTDATEDDRAFTSERVICES}
              onTabClick={this.handleTabOnChange}
              label={messages.outdatedDraftServices}
              tooltip={infoIconMessages.outdatedDraftServices}
              activeType={activeTypeOutdatedInfo}
              contentComponent={OutdatedDraftServices}
            />
            <TaskAccordionHeader
              taskType={taskTypesEnum.OUTDATEDPUBLISHEDSERVICES}
              onTabClick={this.handleTabOnChange}
              label={messages.outdatedPublishedServices}
              tooltip={infoIconMessages.outdatedPublishedServices}
              activeType={activeTypeOutdatedInfo}
              contentComponent={OutdatedPublishedServices}
            />
            <TaskAccordionHeader
              taskType={taskTypesEnum.OUTDATEDDRAFTCHANNELS}
              onTabClick={this.handleTabOnChange}
              label={messages.outdatedDraftChannels}
              tooltip={infoIconMessages.outdatedDraftChannels}
              activeType={activeTypeOutdatedInfo}
              contentComponent={OutdatedDraftChannels}
            />
            <TaskAccordionHeader
              taskType={taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS}
              onTabClick={this.handleTabOnChange}
              label={messages.outdatedPublishedChannels}
              tooltip={infoIconMessages.outdatedPublishedChannels}
              activeType={activeTypeOutdatedInfo}
              contentComponent={OutdatedPublishedChannels}
            />
          </Accordion>
        </div>
        <div className={styles.taskSection}>
          <SectionTitle title={messages.brokenLinksSection} />
          <Accordion light activeIndex={-1} className={styles.stripe} isExclusive={false}>
            <TaskAccordionHeader
              taskType={taskTypesEnum.UNSTABLELINKS}
              onTabClick={this.handleBrokenLinksTabOnChange}
              label={messages.unstableLinks}
              tooltip={infoIconMessages.unstableLinks}
              activeType={activeTypeBrokenLinks}
              contentComponent={LinksUnstable}
            />
            <TaskAccordionHeader
              taskType={taskTypesEnum.EXCEPTIONLINKS}
              onTabClick={this.handleBrokenLinksTabOnChange}
              label={messages.exceptionLinks}
              tooltip={infoIconMessages.exceptionLinks}
              activeType={activeTypeBrokenLinks}
              contentComponent={LinksException}
            />
          </Accordion>
        </div>
        <div className={styles.taskSection}>
          <SectionTitle title={messages.missingConnectionsSection} />
          <Accordion light activeIndex={-1} className={styles.stripe}>
            <TaskAccordionHeader
              taskType={taskTypesEnum.SERVICESWITHOUTCHANNELS}
              onTabClick={this.handleMissingConnectionTabOnChange}
              label={messages.servicesWithoutChannels}
              tooltip={infoIconMessages.servicesWithoutChannels}
              activeType={activeTypeMissingConnections}
              contentComponent={ServicesWithoutChannels}
            />
            <TaskAccordionHeader
              taskType={taskTypesEnum.CHANNELSWITHOUTSERVICES}
              onTabClick={this.handleMissingConnectionTabOnChange}
              label={messages.channelsWithoutServices}
              tooltip={infoIconMessages.channelsWithoutServices}
              activeType={activeTypeMissingConnections}
              contentComponent={ChannelsWithoutServices}
            />
          </Accordion>
        </div>
        <div className={styles.taskSection}>
          <SectionTitle title={messages.translationsSection} />
          <Accordion light activeIndex={-1} className={styles.stripe}>
            <TaskAccordionHeader
              taskType={taskTypesEnum.TRANSLATIONARRIVED}
              onTabClick={this.handleTranslationsTabOnChange}
              label={messages.translationArrivedTitle}
              tooltip={infoIconMessages.translationArrived}
              activeType={activeTypeTranslations}
              contentComponent={TranslationArrived}
            />
            <TaskAccordionHeader
              taskType={taskTypesEnum.TRANSLATIONINPROGRESS}
              onTabClick={this.handleTranslationsTabOnChange}
              label={messages.translationInProgressTitle}
              tooltip={infoIconMessages.translationInProgress}
              activeType={activeTypeTranslations}
              contentComponent={TranslationInProgress}
              columnWidths={['18%', '30%', '30%', '22%']}
            />
          </Accordion>
        </div>
        <div className={styles.taskSection}>
          <SectionTitle title={messages.missingInformationSection} />
          <Accordion light activeIndex={-1} className={styles.stripe}>
            <TaskAccordionHeader
              taskType={taskTypesEnum.MISSINGLANGUAGEORGANIZATIONS}
              onTabClick={this.handleMissingInformationTabOnChange}
              label={messages.missingLangugageOrganizationsTitle}
              tooltip={infoIconMessages.missingLangugageOrganizations}
              activeType={activeTypeMissingInfo}
              contentComponent={MissingLanguageOrganizations}
              columnWidths={['20%', '30%', '25%', '25%']}
            />

            {timedPublishFailedCount > 0 && (
              <TaskAccordionHeader
                taskType={taskTypesEnum.TIMEDPUBLISHFAILED}
                onTabClick={this.handleMissingInformationTabOnChange}
                label={messages.timedPublishFailedTitle}
                tooltip={infoIconMessages.timedPublishFailed}
                activeType={activeTypeMissingInfo}
                contentComponent={TimedPublishFailed}
              />
            )}
          </Accordion>
        </div>
      </div>
    )
  }
}

TasksBasic.propTypes = {
  updateUI: PropTypes.func.isRequired,
  activeTypeOutdatedInfo: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.array
  ]),
  activeTypeMissingConnections: PropTypes.string,
  activeTypeTranslations: PropTypes.string,
  activeTypeMissingInfo: PropTypes.string,
  activeTypeBrokenLinks: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.array
  ]),
  timedPublishFailedCount: PropTypes.number,
  dirtyFormsMap: ImmutablePropTypes.map,
  mergeInUIState: PropTypes.func,
  toggleBrokenLinksAccordion: PropTypes.func
}

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      return {
        isLoading: getNumbersIsFetching(state),
        timedPublishFailedCount: getTasksTypeCount(state, { taskType: taskTypesEnum.TIMEDPUBLISHFAILED })
      }
    }, {
      mergeInUIState,
      toggleBrokenLinksAccordion
    }
  ),
  withState({
    redux: true,
    key: 'taskType',
    initialState: {
      activeTypeOutdatedInfo: [],
      activeTypeBrokenLinks: [],
      activeTypeMissingConnections: null,
      activeTypeTranslations: null,
      activeTypeMissingInfo: null
    }
  }),
  withDirtyFormCheck({
    forms: [
      formTypesEnum.UNSTABLELINKFORM,
      formTypesEnum.EXCEPTIONLINKFORM
    ]
  }),
  withBubbling()
)(TasksBasic)
