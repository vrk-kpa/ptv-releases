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
import { defineMessages } from 'react-intl';

export const buttonMessages = defineMessages({
    continueButton: {
        id: "Containers.ServiceAndChannels.Button.Continue",
        defaultMessage: "Jatka yhteenvetoon"
    },
    publishButton: {
        id: "Containers.ServiceAndChannels.Button.Publish",
        defaultMessage: "Julkaise luonnokset"
    },
});

export const confirmationMessages = defineMessages({
    serviceConfirmationTitle: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Confirmation.Title",
        defaultMessage: "Vahvistus"
    },
    serviceUnCheckTitle: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Confirmation.Uncheck.Title",
        defaultMessage: "Oletko varma, että haluat poistaa valinnan. Tallentamattomat tiedot katoavat."
    },
    clearAllServiceAndChannelsTitle: {
        id: "Containers.ServiceAndChannels.Confirmation.ClearAll.Title",
        defaultMessage: "Oletko varma, että haluat keskeyttää liitosten teon? Tekemäsi muutokset katoavat."
    },
    unsavedRelationsChangesWarningTitle: {
        id: "Containers.ServiceAndChannels.Confirmation.unsavedRelationsChanges.Title",
        defaultMessage: "Oletko varma, että haluat jatkaa yhteenvetoon? Varmista, että kaikki valittujen palvelujen ja asiointikanavien liitokset on tallennettu."
    },

    confirmAcceptButton: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Confirmation.Button.Ok",
        defaultMessage: "Kyllä"
    },
    confirmCancelButton: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Confirmation.Button.Cancel",
        defaultMessage: "Jatka"
    }
});
