import Relations from '../components/Relations'
import { connect } from 'react-redux'
import { compose } from 'redux'
import * as ServiceAndChannelSelectors from 'Containers/Relations/ServiceAndChannels/ServiceAndChannel/Selectors'
import * as CommonServiceAndChannelsSelectors from 'Containers/Relations/ServiceAndChannels/Common/Selectors'
import * as CommonSelectors from 'Containers/Common/Selectors'
import * as serviceAndChannelActions from 'Containers/Relations/ServiceAndChannels/ServiceAndChannel/Actions'
import * as commonServiceAndChannelActions from 'Containers/Common/Actions'
import * as commonActions from 'Containers/Relations/ServiceAndChannels/Common/Actions'
import * as channelSearchAction from 'Containers/Relations/ServiceAndChannels/ChannelSearch/Actions'
import * as serviceSearchAction from 'Containers/Relations/ServiceAndChannels/ServiceSearch/Actions'
import mapDispatchToProps from 'Configuration/MapDispatchToProps'
import { injectIntl } from 'react-intl'

const actions = [
  serviceAndChannelActions,
  commonServiceAndChannelActions,
  commonActions,
  serviceSearchAction,
  channelSearchAction
]
export default compose(
  connect(
    (state, ownProps) => {
      const keyToState = 'serviceAndChannel'
      const editedEntityId = ownProps.location.state
        ? ownProps.location.state.serviceId || ownProps.location.state.channelId
        : null
      const language = CommonServiceAndChannelsSelectors.getLanguageToCodeForServiceAndChannel(state)
      return {
        readOnly: ServiceAndChannelSelectors.getRelationsReadOnly(state, { keyToState }),
        hideRightSearchPannel: CommonServiceAndChannelsSelectors.getRelationConnectedIsAnyOnShowingAdditionalData(state, ownProps),
        countOfConnectedServices: CommonServiceAndChannelsSelectors.getRelationConnectedServicesIdsSize(state, ownProps),
        isAnyRelation: CommonServiceAndChannelsSelectors.getIsAnyRelation(state, ownProps),
        isFetching: CommonSelectors.getStepCommonIsFetching(state, { keyToState }),
        areDataValid: CommonSelectors.getStepCommonAreDataValid(state, { keyToState }),
        newConnectedChannelIds: CommonServiceAndChannelsSelectors.getRelationNewChannelRelationsIds(state, ownProps),
        searchIsFetching: CommonSelectors.getSearchResultsIsFetching(state, { keyToState: 'serviceAndChannelServiceSearch', keyToEntities: 'services' }),
        searchAreDataValid: CommonSelectors.getSearchResultsAreDataValid(state, { keyToState: 'serviceAndChannelServiceSearch', keyToEntities: 'services' }),
        searchedServices: CommonServiceAndChannelsSelectors.getSearchedServiceEntities(state, { keyToState: 'serviceAndChannelServiceSearch', keyToEntities: 'services', language }),
        editedEntity : editedEntityId != null,
        editedService: CommonServiceAndChannelsSelectors.getService(state, { id : ownProps.location.state ? ownProps.location.state.entityId : null }),
        confirmationData: CommonServiceAndChannelsSelectors.getConfirmation(state, { keyToState:'serviceAndChannelConfirmation' }),
        language,
        currentTabIndex: CommonServiceAndChannelsSelectors.getCurrentTabIndex(state, { keyToState })
      }
    },
    mapDispatchToProps(actions)
  ),
  injectIntl
)(Relations)

