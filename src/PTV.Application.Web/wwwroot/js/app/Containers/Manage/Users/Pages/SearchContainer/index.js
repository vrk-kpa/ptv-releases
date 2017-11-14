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
import React, {PropTypes, Component} from 'react';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'
import { PTVHeaderSection, PTVPreloader, PTVTable, PTVLabel } from '../../../../../Components';


const messages = defineMessages({
    headerTitle: {
        id: "Containers.Manage.Users.Search.Header.Title",
        defaultMessage: "Käyttäjät"
    },
    headerDescription: {
        id: "Containers.Manage.Users.Search.Header.Description",
        defaultMessage: "Käyttäjänhallinta löytyy ulkopuolisen linkin kautta englanninkieliseltä sivustolta. Alla oleva linkki avaa uuden selainikkunan ja voit palata takaisin Palvelutietovarantoon pitämällä tämän selainikkunan auki."
    },
});

class SearchUserContainer extends Component {

    constructor(props) {
        super(props);
    }

    render() {

        const {formatMessage} = this.props.intl;
        let stsURL = getStsUrl() || null;

        return (
            <div>
                <PTVHeaderSection
                    titleClass = "card-title"
                    title= {formatMessage(messages.headerTitle)}
                    descriptionClass = "lead"
                    description = {formatMessage(messages.headerDescription)}
                    showButton = { false }
                />

                <div>
                    <a lang="fi" href={stsURL + '/ui/login'}>{stsURL}</a>
                </div>

                <div>
                    <p>Sts alkuiselta sivustolta löytyvät seuraavat toiminnot: </p>
                    <p>Vaihda salasana = Change password </p>
                    <p>Lisää käyttäjä = Register new user </p>
                    <p>Valitse käyttäjän rooli = Set role to user </p>
                    <p>Valitse käyttäjän organisaatio = Map user to organization </p>
                </div>
            </div>

        );
    }
}

export default injectIntl(SearchUserContainer);
