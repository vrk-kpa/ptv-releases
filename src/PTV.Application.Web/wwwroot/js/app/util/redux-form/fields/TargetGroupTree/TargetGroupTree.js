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
import React from 'react'
import PropTypes from 'prop-types'
import { Field, change } from 'redux-form/immutable'
import { connect } from 'react-redux'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { localizeItem } from 'appComponents/Localize'
import { compose } from 'redux'
import { RenderTreeView } from 'util/redux-form/renders'
import { Nodes, Node, NodeLabelCheckBox, withCustomLabel } from 'util/redux-form/renders/RenderTreeView/TreeView'
import * as nodeActions from 'Containers/Common/Actions/Nodes'
import mapDispatchToProps from 'Configuration/MapDispatchToProps'
import { getTargetGroupsIds,
  getTargetGroup,
  getGeneralDescriptionTargetGroupsIds,
  getFormOverrideTargetGroupsOS,
  getTopTargetGroupsEntities,
  getFormTargetGroupsOrderedSet,
  getIsGeneralDescriptionAttached
} from './selectors'
import withLabel from 'util/redux-form/HOC/withLabel'
import asComparable from 'util/redux-form/HOC/asComparable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withErrorMessage from 'util/redux-form/HOC/withErrorMessage'
import CommonMessages from 'util/redux-form/messages'
import { Map, List } from 'immutable'

const TargetGroupNodes = compose(
  injectFormName,
  connect((state, ownProps) => {
    const nodes = (ownProps.isRootLevel || ownProps.checked) && getTargetGroupsIds(state, ownProps) || List()
    return { nodes, listType: 'simple' }
  })
)(Nodes)

const CustomNodeLabelCheckBox = compose(
  injectFormName,
  connect((state, ownProps) => ({
    targetGroupsOrderedSet: getFormTargetGroupsOrderedSet(state, ownProps),
    topTargetGroups: getTopTargetGroupsEntities(state, ownProps),
    generalDescriptionTargetGroups: getGeneralDescriptionTargetGroupsIds(state, ownProps),
    overrideTargetGroups: getFormOverrideTargetGroupsOS(state, ownProps)
  }), { change })
)(({
  onChange,
  formName,
  generalDescriptionTargetGroups,
  overrideTargetGroups,
  topTargetGroups,
  targetGroupsOrderedSet,
  change,
  ...rest
}) => {
  const handleOnChange = (id, value) => {
    if (generalDescriptionTargetGroups.get(id)) {
      let oTG
      if (!overrideTargetGroups.get(id)) {
        oTG = overrideTargetGroups.add(id)
      } else {
        oTG = overrideTargetGroups.delete(id)
      }

      change(formName, 'overrideTargetGroups', oTG)
      onChange(id, false)
    } else {
      onChange(id, value)
    }

    if (!value) {
      const children = (topTargetGroups.get(id) || Map()).get('children')
      if (children && children.size > 0) {
        const chIds = children.push(id).toOrderedSet()
        const newValues = targetGroupsOrderedSet.subtract(chIds)
        change(formName, 'targetGroups', newValues)
      }
    }
  }

  return (
    <NodeLabelCheckBox {...rest} onChange={handleOnChange} />
  )
})

const TargetGroupNode = compose(
  injectFormName,
  connect(
    (state, ownProps) => {
      const fromGD = getGeneralDescriptionTargetGroupsIds(state, ownProps)
      const fromOGD = getFormOverrideTargetGroupsOS(state, ownProps)
      const isFromOGD = fromOGD.get(ownProps.id)
      const isFromGD = fromGD.includes(ownProps.id)
      const checked = isFromOGD ? false : isFromGD || ownProps.value && ownProps.value.get(ownProps.id) || false
      const node = getTargetGroup(state, ownProps)
      return {
        node,
        checked,
        isLeaf: !checked || node.get('isLeaf')
      }
    },
    mapDispatchToProps([nodeActions])
  ),
  localizeItem({ input: 'node', output: 'node' }),
  withCustomLabel(CustomNodeLabelCheckBox)
)(Node)

const TargetGroupNodeReadOnly = compose(
  injectFormName,
  connect(
    (state, ownProps) => {
      const fromGD = getGeneralDescriptionTargetGroupsIds(state, ownProps)
      const fromOGD = getFormOverrideTargetGroupsOS(state, ownProps)
      const isFromOGD = fromOGD.get(ownProps.id)
      const isFromGD = fromGD.includes(ownProps.id)
      const checked = isFromOGD ? false : isFromGD || ownProps.value && ownProps.value.get(ownProps.id) || false
      const node = getTargetGroup(state, ownProps)
      return {
        node,
        checked,
        isLeaf: true,
        defaultCollapsed: false
      }
    },
    mapDispatchToProps([nodeActions])
  ),
  localizeItem({ input: 'node', output: 'node' }),
)(props => (props.checked && <Node {...props} /> || null))

const messages = defineMessages({
  subLabel: {
    id: 'Containers.Services.AddService.Step2.SubTargetGroup.Title',
    defaultMessage: 'Tarkenna tarvittaessa tätä kohderyhmää:'
  },
  subLink: {
    id: 'Containers.Services.AddService.Step2.SubTargetGroup.Link',
    defaultMessage: 'Tarkenna kohderyhmä'
  },
  tooltip: {
    id: 'Containers.Services.AddService.Step2.TargetGroup.Tooltip',
    defaultMessage: 'Palvelu luokitellaan kohderyhmän mukaan. Valitse kohderyhmä, jolle palvelu on suunnattu. Valitse vähintään yksi päätason kohderyhmä ja tarvittaessa tarkenna valintaa alakohderyhmällä. Jos palvelulla ei ole erityistä alakohderyhmää, älä valitse kaikkia alemman tason kohderyhmiä, vaan jätä alemman tason valinta kokonaan tekemättä. Mikäli olet käyttänyt palvelun pohjakuvausta, kenttään on kopioitu valmiiksi pohjakuvauksen kohderyhmä/t. Voit tarvittaessa lisätä kohderyhmiä.'
  },
  additionalLabel: {
    id: 'TargetGroupTree.GeneralDescription.Additional.Title',
    defaultMessage: 'The target group selection comes from the general description. You can edit the selection.'
  }
})

const Tree = compose(
  withErrorMessage,
  withFormFieldAnchor
)(RenderTreeView)

const TargetGroupTree = ({
  intl: { formatMessage },
  isReadOnly,
  isCompareMode,
  ...rest
}) => (
  <Field
    name='targetGroups'
    component={!isReadOnly && Tree || RenderTreeView}
    label={formatMessage(CommonMessages.targetGroups)}
    NodesComponent={TargetGroupNodes}
    NodeComponent={!isReadOnly && TargetGroupNode || TargetGroupNodeReadOnly}
    inline={!isReadOnly && !isCompareMode}
    simple
    isReadOnly={isReadOnly}
    {...rest}
  />
)
TargetGroupTree.propTypes = {
  intl: intlShape,
  isReadOnly: PropTypes.bool.isRequired,
  isCompareMode: PropTypes.bool.isRequired
}

// const TargetGroupTreeComponent = compose(
//   injectIntl,
//   withFormStates,
//   withLabel(CommonMessages.targetGroups, messages.tooltip, true)
// )(TargetGroupTree)

// const TargetGroupTreeComponentReadOnly = compose(
//   injectIntl,
//   withFormStates,
//   withLabel(CommonMessages.targetGroups)
// )(TargetGroupTree)

export default compose(
  injectIntl,
  withFormStates,
  connect(
    (state, ownProps) => {
      const isGDAttached = getIsGeneralDescriptionAttached(state, ownProps)
      return {
        additionalLabel: isGDAttached && messages.additionalLabel
      }
    }),
  asDisableable,
  asComparable(),
  withLabel(CommonMessages.targetGroups, messages.tooltip, true)
)(TargetGroupTree)

