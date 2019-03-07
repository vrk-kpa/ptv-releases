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
import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { Field } from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { branch } from 'recompose'
import asContainer from 'util/redux-form/HOC/asContainer'
import asGroup from 'util/redux-form/HOC/asGroup'
import asComparable from 'util/redux-form/HOC/asComparable'
import Info from './Info'
import Links from './Links'
import TimeStamp from './TimeStamp'
import Instructions from './Instructions'
import ContactInfo from './ContactInfo'
import { defineMessages, FormattedMessage } from 'util/react-intl'
import {
  getIsAccessibilityRegisterSet,
  getIsAccessibilityRegisterValid
} from './selectors'
import withFormStates from 'util/redux-form/HOC/withFormStates'

const messages = defineMessages({
  title: {
    id: 'Accessibility.Title',
    defaultMessage: 'Käyntiosoitteen esteettömystiedot'
  },
  subTitle: {
    id: 'Accessibility.SubTitle',
    defaultMessage: 'Palvelupaikan esteettömystiedot'
  },
  arEmpty: {
    id: 'Accessibility.ArEmpty',
    defaultMessage: 'Tietoa ei saatu esteettömyyssovelluksesta',
    description: {
      en: 'Information was not received from the accessibility application'
    }
  }
})

const AccessibilityInformation = asGroup({
  title: (
    <Fragment>
      <FormattedMessage {...messages.subTitle} />
      {' '}
      <TimeStamp />
    </Fragment>
  )
})(({
  index,
  items,
  addressUseCase,
  compare
}) => (
  <Info
    index={index}
    compare={compare}
  />
))

const AccessibilityInformationBody = compose(
  withFormStates,
  connect(state => ({
    arFetched: getIsAccessibilityRegisterSet(state),
    arValid: getIsAccessibilityRegisterValid(state)
  })),
  branch(({ isReadOnly, arFetched }) =>
    isReadOnly && arFetched,
  asContainer({ doNotCompareContainerHead: true })),
  branch(({ isReadOnly }) =>
    !isReadOnly,
  asContainer({ title: messages.title, doNotCompareContainerHead: true }))
)(({
  isReadOnly,
  arFetched,
  arValid,
  ...rest
}) => (
  <Fragment>
    {!isReadOnly && <Instructions />}
    {arFetched && <AccessibilityInformation doNotCompareGroupHead {...rest} />}
    <ContactInfo />
    {(!arValid && isReadOnly && arFetched) && (
      <FormattedMessage {...messages.arEmpty} />
    )}
  </Fragment>
))

AccessibilityInformationBody.propTypes = {
  arFetched: PropTypes.bool,
  arValid: PropTypes.bool,
  isReadOnly: PropTypes.bool
}

const RenderAccessibilityInformation = ({
  index,
  items,
  addressUseCase,
  compare,
  showLinks
}) => (
  <Fragment>
    {showLinks && (
      <Links
        index={index}
        items={items}
        addressUseCase={addressUseCase}
      />
    )}
    <AccessibilityInformationBody
      index={index}
      items={items}
      addressUseCase={addressUseCase}
      compare={compare}
      showLinks={showLinks}
      withinGroup
    />
  </Fragment>
)
RenderAccessibilityInformation.propTypes = {
  index: PropTypes.number,
  items: PropTypes.object,
  addressUseCase: PropTypes.string,
  compare: PropTypes.bool,
  showLinks: PropTypes.bool
}

const Accessibility = props => (
  <Field
    name='accessibilityInformation'
    component={RenderAccessibilityInformation}
    {...props}
  />
)

export default compose(
  asComparable({ DisplayRender: RenderAccessibilityInformation })
)(Accessibility)
