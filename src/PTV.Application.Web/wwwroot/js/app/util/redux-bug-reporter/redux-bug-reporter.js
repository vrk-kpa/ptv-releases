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
import { connect } from 'react-redux'
import { middlewareData } from './store-enhancer'
import { listenToErrors, errorData } from './utils'
import BugReportItem from './components/BugReportItem'
import { API_WEB_ROOT } from 'Configuration/AppHelpers'

const submitToPtv = newBug => {
  const {
    name,
    description,
    actions,
    state,
    initialState,
    createdBy
  } = newBug
  return fetch(`${API_WEB_ROOT}bugreport/SaveBugReport`, {
    method: 'post',
    headers: {
      'Content-Type': 'application/json',
      'authorization': `Bearer ${window.getAccessToken()}`
    },
    body: JSON.stringify({
      name,
      description,
      actions: JSON.stringify(actions),
      initialState: JSON.stringify(initialState),
      finalState: JSON.stringify(state),
      version: window.getAppVersion(),
      createdBy
    })
  })
}

class BugReporter extends PureComponent {
  state = {
    expanded: false,
    mounted: false,
    loading: false,
    bugFiled: false,
    messageShown: false,
    reporter: this.props.name || '',
    name: '',
    createdBy: '',
    description: '',
    error: '',
    bugReports: []
  }
  componentDidMount () {
    this.setState({ mounted: true })
    listenToErrors()
    this.fetchAllBugReports()
  }
  fetchAllBugReports = async () => {
    this.setState({ loading: true })
    const response = await fetch(
      `${API_WEB_ROOT}bugreport/GetAllBugs`, {
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json',
          'authorization': `Bearer ${window.getAccessToken()}`
        }
      }
    )
    const bugReports = await response.json()
    if (bugReports) {
      const bugReportHashMap = bugReports
        .map(bugReport => ({
          ...bugReport,
          loading: false,
          loaded: false
        }))
        .reduce((acc, val) => {
          acc[val.id] = val
          return acc
        }, {})
      this.setState({
        bugReports: bugReportHashMap
      })
    }
    this.setState({ loading: false })
  }
  fetchBugReport = async id => {
    this.setState({
      bugReports: {
        ...this.state.bugReports,
        [id]: {
          ...this.state.bugReports[id],
          loading: true
        }
      }
    })
    try {
      const response = await fetch(
        `${API_WEB_ROOT}bugreport/GetBugReportById?id=${id}`, {
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'authorization': `Bearer ${window.getAccessToken()}`
          }
        }
      )
      const bugReport = await response.json()
      this.setState({
        bugReports: {
          ...this.state.bugReports,
          [bugReport.id]: {
            ...bugReport,
            loaded: true,
            actions: JSON.parse(bugReport.actions),
            initialState: JSON.parse(bugReport.initialState),
            finalState: JSON.parse(bugReport.finalState)
          }
        }
      })
    } catch (err) {
      console.log(`Error occured when fetching bug report with id: ${id}`, err)
    } finally {
      this.setState({
        bugReports: {
          ...this.state.bugReports,
          [id]: {
            ...this.state.bugReports[id],
            loading: false
          }
        }
      })
    }
  }
  toggleExpanded = () => this.setState({ expanded: !this.state.expanded })
  submit = async e => {
    e.preventDefault()
    const { storeState } = this.props
    const {
      name,
      createdBy,
      description
    } = this.state

    this.setState({ loading: true })
    const initialState = middlewareData.getBugReporterInitialState()

    const newBug = {
      state: storeState.toJS(),
      initialState: initialState.toJS(),
      actions: middlewareData.getActions(),
      consoleErrors: errorData.getErrors(),
      name,
      createdBy,
      description
    }

    try {
      await submitToPtv(newBug)
      this.setState({
        loading: false,
        bugFiled: true,
        expanded: true
      })
    } catch (error) {
      this.setState({
        loading: false,
        bugFiled: true,
        expanded: true,
        error
      })
    } finally {
      setTimeout(() => this.setState({
        bugFiled: false,
        expanded: false
      }), 2000)
    }
  }

  dismiss = e => {
    e.preventDefault()
    this.setState({
      bugFiled: false,
      expanded: false
    })
  }

  handleChange = field => e => this.setState({ [field]: e.target.value })

  render () {
    let {
      name,
      createdBy,
      description,
      mounted,
      loading,
      bugFiled,
      error,
      messageShown,
      expanded
    } = this.state
    if (!mounted) {
      return false
    }
    if (loading) {
      return (
        <div className='Redux-Bug-Reporter'>
          <div className='Redux-Bug-Reporter__loading-container'>
            <span className='Redux-Bug-Reporter__loading' />
          </div>
        </div>
      )
    }
    if (bugFiled) {
      return (
        <div className='Redux-Bug-Reporter'>
          <div className={`Redux-Bug-Reporter__form Redux-Bug-Reporter__form--${error ? 'fail' : 'success'}`}>
            {error ? (
              <div>
                <div>Oops, something went wrong!</div>
                <div>Please refresh application and try again later</div>
              </div>
            ) : (
              <div>
                <div>Your bug has been filed successfully!</div>
              </div>
            )}
          </div>
          <div className='Redux-Bug-Reporter__show-hide-container'>
            <button className={`Redux-Bug-Reporter__show-hide-button Redux-Bug-Reporter__show-hide-button--${error ? 'expanded' : 'collapsed'}`} onClick={this.dismiss} />
          </div>
        </div>
      )
    }

    return (
      <div className='Redux-Bug-Reporter'>
        {expanded && (
          <div className='Redux-Bug-Reporter__form'>
            <div
              style={{
                maxHeight: '300px',
                overflow: 'auto'
              }}
            >
            {this.state.bugReports &&
              Object.keys(this.state.bugReports).map((key, index) => {
                const bugReport = this.state.bugReports[key]
                return (
                  <BugReportItem
                    key={index}
                    onLoad={this.fetchBugReport}
                    {...bugReport}
                  />
                )
              })}
            </div>
            <form onSubmit={this.submit}>
              <input
                className='Redux-Bug-Reporter__form-input Redux-Bug-Reporter__form-input--reporter'
                onChange={this.handleChange('name')}
                value={name}
                placeholder='Name'
              />
              <input
                className='Redux-Bug-Reporter__form-input Redux-Bug-Reporter__form-input--screenshotURL'
                onChange={this.handleChange('createdBy')}
                value={createdBy}
                placeholder='Created by'
              />
              <textarea
                className='Redux-Bug-Reporter__form-input Redux-Bug-Reporter__form-input--notes'
                onChange={this.handleChange('description')}
                value={description}
                placeholder='Description'
                />
              <button className='Redux-Bug-Reporter__submit-button' type='submit'>File Bug</button>
            </form>
          </div>
        )}
        <div className='Redux-Bug-Reporter__show-hide-container'>
          <button className={`Redux-Bug-Reporter__show-hide-button Redux-Bug-Reporter__show-hide-button--${this.state.expanded ? 'expanded' : 'collapsed'}`} onClick={this.toggleExpanded} />
        </div>
      </div>
    )
  }
}
BugReporter.propTypes = {
  name: PropTypes.string,
  storeState: PropTypes.any.isRequired
}


export { BugReporter }
export default connect(
  state => ({ storeState: state })
)(BugReporter)
