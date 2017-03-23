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
import { defineMessages, injectIntl, intlShape, FormattedMessage, FormattedHTMLMessage } from 'react-intl';
import { getUserName } from 'Configuration/selectors';

const messages = defineMessages({
    pageTitle: {
        id: "Containers.FrontPage.LandingPage.Title",
        defaultMessage: "Tervetuloa Suomi.fi-palvelutietovarantoon {userName}"
    },
    pageDescription: {
        id: 'Containers.FrontPage.LandingPage.Description',
        defaultMessage: '<p>Palvelutietovarantoon voit kuvata organisaatiosi palvelut ja niiden asiointikanavat - verkkoasiointikanavat, tulostettavat lomakkeet, verkkosivut, puhelinasiointikanavat ja palvelupisteet.</p>' + '<p>Palvelut kuvataan asiakaslähtöisesti. Palvelujen kuvaamista on ohjeistettu käyttöliittymissä olevilla ohjeilla. Kuvaa palvelut ohjeiden mukaisesti. Kirjoita yleistajuisesti, asiakkaan näkökulmasta.</p>' + '<p>Haluatko antaa palautetta tai kehitysehdotuksia palvelutietovarannosta tai tarvitsetko apua palvelutietovarannon käytössä? Ota yhteyttä sähköpostitse ptv-tuki@vrk.fi.</p>'
    }
});

class FrontPageContainer extends React.Component {
    render() {
        const {formatMessage} = this.props.intl;

        return (
              <div>
                 <h2>{formatMessage(messages.pageTitle, { userName: getUserName() })}</h2>
                 <div>
                    <FormattedHTMLMessage {...messages.pageDescription}></FormattedHTMLMessage>
                 </div>
              </div>
                )
            }
        }
export default injectIntl(FrontPageContainer);
