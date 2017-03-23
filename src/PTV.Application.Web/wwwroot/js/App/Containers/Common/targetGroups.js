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
import React from 'react';
import { connect } from 'react-redux';
import { injectIntl, defineMessages } from 'react-intl';

import * as ServiceSelectors from '../Services/Service/Selectors';
import * as CommonServiceSelectors from '../Services/Common/Selectors';
import { PTVGroup } from '../../Components';
import { LocalizedTwoLevelCheckBoxList } from '../Common/localizedData';

const messages = defineMessages({
    targetGroupTitle: {
        id: "Containers.Services.AddService.Step2.TargetGroup.Title",
        defaultMessage: "Kohderyhmä"
    },
    subTargetGroupTitle: {
        id: "Containers.Services.AddService.Step2.SubTargetGroup.Title",
        defaultMessage: "Tarkenna tarvittaessa tätä kohderyhmää:"
    },
    subTargetLink: {
        id: "Containers.Services.AddService.Step2.SubTargetGroup.Link",
        defaultMessage: "Tarkenna kohderyhmä"
    },
    targetGroupTooltip: {
        id: "Containers.Services.AddService.Step2.TargetGroup.Tooltip",
        defaultMessage: "Palvelu luokitellaan kohderyhmän mukaan. Valitse kohderyhmä, jolle palvelu on suunnattu. Valitse vähintään yksi päätason kohderyhmä ja tarvittaessa tarkenna valintaa alakohderyhmällä. Jos palvelulla ei ole erityistä alakohderyhmää, älä valitse kaikkia alemman tason kohderyhmiä, vaan jätä alemman tason valinta kokonaan tekemättä. Mikäli olet käyttänyt palvelun pohjakuvausta, kenttään on kopioitu valmiiksi pohjakuvauksen kohderyhmä/t. Voit tarvittaessa lisätä kohderyhmiä. "
    },
});

const TargetGroups = ({intl, readOnly, topTargetGroups, targetGroups, validators, onClick, isTargetGroupSelected, generalTargetGroups,language }) => {
            const { formatMessage } = intl;
            const onTargetGroupClick = (id, checked) => {
                onClick(id, checked, generalTargetGroups.contains(id));
            }

            const renderTargetGroups = (targetGroups, readOnly) => {
                return (
                    topTargetGroups.map(tg => {
                        return <LocalizedTwoLevelCheckBoxList 
                                componentClass="checkbox-list"
                                key= { tg }
                                id= { tg }
                                data= { targetGroups }
                                onClick= { onTargetGroupClick }
                                isSelectedSelector= { ServiceSelectors.getIsSelectedTargetGroupWithGeneralDescription }
                                //isDisabledSelector= { ServiceSelectors.getIsSelectedTargetGroupFormGeneralDescription }
                                subGroupTitle = { formatMessage(messages.subTargetGroupTitle) }
                                readOnly = { readOnly }
                                language = { language }
                                link = { formatMessage(messages.subTargetLink) }
                                />
                        }
                    )
                );
            }

            return (<PTVGroup
                        validators={ validators }
                        order={ 50 }
                        contentClassName="col-xs-12"
                        labelClassName="col-xs-12"
                        labelTooltip={ formatMessage(messages.targetGroupTooltip) }
                        labelContent={ formatMessage(messages.targetGroupTitle) }
                        validatedField={ formatMessage(messages.targetGroupTitle) }
                        readOnly = { readOnly }
                        isAnySelected = { isTargetGroupSelected }>
                        {renderTargetGroups(targetGroups, readOnly)}
                </PTVGroup>)
}

function mapStateToProps(state, ownProps) {
    const isTargetGroupSelected = ServiceSelectors.getIsAnyTargetGroupsSelected(state, ownProps);
  return {
    isTargetGroupSelected: isTargetGroupSelected,
    targetGroups: CommonServiceSelectors.getTargetGroups(state, ownProps),
    topTargetGroups: CommonServiceSelectors.getTopTargetGroups(state, ownProps),
    generalTargetGroups: ServiceSelectors.getSelectedTargetGroupsFormGeneralDescription(state, ownProps)
  }
}

export default connect(mapStateToProps)(injectIntl(TargetGroups));