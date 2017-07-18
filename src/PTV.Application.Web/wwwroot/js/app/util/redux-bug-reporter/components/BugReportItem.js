import React, { PureComponent, PropTypes } from 'react'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'
import {
  overloadStore,
  initializePlayback,
  finishPlayback,
  playbackFlag
} from '../store-enhancer'
import './BugReportItem.scss'
import { fromJS } from 'immutable'

class BugReportItem extends PureComponent {
  state = {
    playing: false,
    actionIndex: 0,
    actionsCount: 0,
    progress: 0
  }
  handleOnPlayActions = () => {
    const {
      dispatch,
      overloadStore,
      initializePlayback,
      finishPlayback,
      actions
    } = this.props
    initializePlayback()
    const initialState = fromJS(this.props.initialState)
    overloadStore(initialState)
    let actionIndex = 0
    const performNextAction = () => {
      const action = actions[actionIndex++]
      this.setState({
        actionIndex,
        progress: Math.floor((actionIndex / actions.length) * 100)
      })
      action[playbackFlag] = true
      dispatch(action)
      const isLastAction = actionIndex > actions.length - 1
      if (!isLastAction) {
        setTimeout(performNextAction, 100)
      } else {
        finishPlayback()
        this.setState({
          playing: false
        })
      }
    }
    this.setState({
      playing: true,
      actionsCount: actions.length
    })
    performNextAction()
  }
  handleJumpToFinalState = () => {
    this.props.overloadStore(fromJS(this.props.finalState))
    this.setState({
      playing: true,
      progress: 100
    })
  }
  render () {
    const {
      id,
      name,
      description,
      loading,
      loaded,
      onLoad
    } = this.props
    const playButton = (
      <div
        onClick={this.handleOnPlayActions}
        className='bug-report-item-play-button'
      >
        ▶
      </div>
    )
    const skipToEndButton = (
      <div
        onClick={this.handleJumpToFinalState}
        className='bug-report-item-skip-to-end-button'
      >
        SKIP TO END
      </div>
    )
    const progressBar = (
      <div className='bug-report-item-progress-bar-wrapp'>
        <div
          className='bug-report-item-progress-bar'
          style={{
            width: `${this.state.progress}%`
          }}
        />
      </div>
    )
    const downloadButton = <div className='bug-report-download-icon' onClick={() => onLoad(id)}>⭳</div>
    const loadingIcon = <div className='bug-report-loading-icon'>↻</div>
    return (
      <div className='bug-report-item'>
        <div className='bug-report-item-header-wrapp'>
          <b>{name}</b>
          <div>{description}</div>
        </div>
        <div className='bug-report-item-progress-controls-wrapp'>
          {progressBar}
          <div className='bug-report-item-progress-controls-buttons'>
            {!loading && loaded && playButton}
            {!loading && !loaded && downloadButton}
            {loading && loadingIcon}
            {skipToEndButton}
          </div>
        </div>
      </div>
    )
  }
}
BugReportItem.propTypes = {
  loading: PropTypes.bool,
  loaded: PropTypes.bool,
  onLoad: PropTypes.func,
  id: PropTypes.string.isRequired,
  name: PropTypes.string.isRequired,
  description: PropTypes.string.isRequired,
  actions: PropTypes.array,
  initialState: PropTypes.object,
  finalState: PropTypes.object,
  dispatch: PropTypes.func.isRequired,
  overloadStore: PropTypes.func.isRequired,
  initializePlayback: PropTypes.func.isRequired,
  finishPlayback: PropTypes.func.isRequired
}

export default connect(
  state => ({
    storeState: state
  }),
  dispatch => {
    const boundActions = bindActionCreators({
      overloadStore,
      initializePlayback,
      finishPlayback
    }, dispatch)
    return {
      dispatch,
      ...boundActions
    }
  }
)(BugReportItem)
