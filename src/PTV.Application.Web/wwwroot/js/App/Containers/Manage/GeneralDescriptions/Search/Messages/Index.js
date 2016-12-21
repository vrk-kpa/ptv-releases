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

export const messages = defineMessages({
    generalDescriptionHeaderTitle:{
        id:"Containers.Services.GeneralDescriptions.Header.Title", 
        defaultMessage: "Hae ja liitä yleiskuvaus"
    },
    generalDescriptionHeaderSearchTitle:{
        id:"Containers.Services.GeneralDescriptions.Header.Search.Title", 
        defaultMessage: "Pohjakuvaukset"
    },
    generalDescriptionGoBack: {
        id: "Containers.Manage.GeneralDescriptions.Search.Button.GoBack",
        defaultMessage: "Ohita pohjakuvauksen lisääminen"
    },
    generalDescriptionButtonAdd:{
        id: "Containers.Manage.GeneralDescriptions.Search.Button.Add",
        defaultMessage: "Add general description"
    },
    generalDescriptionHeaderText: {
        id: "Containers.Manage.GeneralDescriptions.Connect.Description",
        defaultMessage: "<p>Palvelutietovarantoon on tuotettu valmiiksi palvelujen pohjakuvauksia. Pohjakuvauksessa on kuvattu yleisellä tasolla palvelu, jota useat organisaatiot tuottavat. Alkuvaiheessa pohjakuvauksia on tuotettu kuntien lakisääteisiin tehtäviin perustuvista palveluista. Jos et                    löydä sopivaa pohjakuvausta, ohita sen lisääminen.</p>"+
                        "<p>Pohjakuvaus on kokonaisuus, joka sisältää palvelun nimen, kuvaustekstin ja metatietoja. Valitun pohjakuvauksen tiedot kopioituvat pohjaksi uudelle palvelulle. Voit tarvittaessa muuttaa palvelun nimeä, lisätä palveluluokkia ja kohderyhmiä. Pohjakuvauksen kuvaustekstin lisäksi palvelulle on kirjoitettava tarkentava palvelukuvaus, jossa kerrotaan palvelun paikalliset lisätiedot.</p>" +
                        "<p>Hae pohjakuvauksia yhdellä tai useammalla hakuehdolla, valitse haluamasi rivi ja klikkaa Valitse pohjakuvaus-painiketta.</p>"
    },
    generalDescriptionHeaderSearchText: {
        id: "Containers.Manage.GeneralDescriptions.Search.Description",
        defaultMessage: "<p>Palvelutietovarantoon voidaan tuottaa valmiiksi palvelujen pohjakuvauksia. Pohjakuvauksessa on kuvattu yleisellä tasolla palvelu, jota useat organisaatiot tarjoavat. Pohjakuvaus on kokonaisuus, joka sisältää palvelun nimen, kuvaustekstin ja metatietoja. Valitun pohjakuvauksen tiedot kopioituvat pohjaksi uudelle palvelulle. Palvelujen ylläpitäjä voi tarvittaessa muuttaa palvelun nimeä, lisätä palveluluokkia ja kohderyhmiä.</p>"+
                        "<p>Tässä osiossa voit lisätä uusia sekä hakea ja muokata olemassa olevia oman organisaatiosi pohjakuvauksia. Hae pohjakuvauksia yhdellä tai useammalla hakuehdolla. Luo uusi Lisää pohjakuvaus -painikkeella tai hae olemassa olevia ja valitse haluttu rivi muokattavaksi.</p>"
    },
    nameTitle: {
        id: "Containers.Manage.GeneralDescriptions.Search.Name.Title",
        defaultMessage: "Nimi"
    },
    namePlaceholder: {
        id: "Containers.Manage.GeneralDescriptions.Search.Name.Placeholder",
        defaultMessage: "Hae pohjakuvauksen nimellä. "
    },
    serviceClassComboTitle: {
        id: "Containers.Manage.GeneralDescriptions.Search.ServiceClass.Title",
        defaultMessage: "Palveluluokka"
    },
    serviceClassComboTooltip: {
        id: "Containers.Manage.GeneralDescriptions.Search.ServiceClass.Tooltip",
        defaultMessage: "Voit hakea pohjakuvauksia myös palveluluokan mukaan. Palveluluokka on aihetunniste, jonka avulla pohjakuvaukset voidaan ryhmitellä ja löytää. Pohjakuvaus voi kuulua useaan eri luokkaan. Valitse pudotusvalikosta haluamasi palveluluokka."
    },
    targetGroupComboTitle: {
        id: "Containers.Manage.GeneralDescriptions.Search.TargetGroup.Title",
        defaultMessage: "Pääkohderyhmä"
    },
    targetGroupComboTooltip: {
        id: "Containers.Manage.GeneralDescriptions.Search.TargetGroup.Tooltip",
        defaultMessage: "Voit hakea tietylle kohderyhmälle tarkoitettujen palveluiden pohjakuvauksia. Valitse pudotusvalikosta pääkohderyhmä. Tarvittaessa tarkenna kohderyhmävalintaa valitsemalla alakohderyhmä."
    },
    subTargetGroupComboTitle: {
        id: "Containers.Manage.GeneralDescriptions.Search.SubTargetGroup.Title",
        defaultMessage: "Alakohderyhmä"
    },
    generalDescriptionsResultHeaderName: {
        id: "Containers.Manage.GeneralDescriptions.SearchResult.Header.Name",
        defaultMessage: "Nimi"
    },
    generalDescriptionsResultHeaderServiceClass: {
        id: "Containers.Manage.GeneralDescriptions.SearchResult.Header.ServiceClass",
        defaultMessage: "Palveluluokka"
    },
    generalDescriptionsResultHeaderShortDescription: {
        id: "Containers.Manage.GeneralDescriptions.SearchResult.Header.ShortDescription",
        defaultMessage: "Lyhyt kuvaus"
    },
    generalDescriptionDetailIcon: {
        id: "Containers.Manage.GeneralDescriptions.SearchResult.Header.Detail",
        defaultMessage: "Tarkemmat tiedot"
    },
    detaileHeaderTitle: {
        id: "Containers.Manage.GeneralDescriptions.Detail.Header.Title",
        defaultMessage: "Tiedot"
    },
    detaileNameTitle: {
        id: "Containers.Manage.GeneralDescriptions.Detail.Name.Title",
        defaultMessage: "Nimi"
    },
    detailAlternateNameTitle: {
        id: "Containers.Manage.GeneralDescriptions.Detail.AlternateName.Title",
        defaultMessage: "Vaihtoehtoinen nimi"
    },
    detailServiceClassTitle: {
        id: "Containers.Manage.GeneralDescriptions.Detail.ServiceClass.Title",
        defaultMessage: "Palveluluokka"
    },
    detailTargetGroupTitle: {
        id: "Containers.Manage.GeneralDescriptions.Detail.TargetGroup.Title",
        defaultMessage: "Pääkohderyhmä"
    },
    detailSubTargetGroupTitle: {
        id: "Containers.Manage.GeneralDescriptions.Detail.SubTargetGroup.Title",
        defaultMessage: "Alakohderyhmä"
    },
    detailDescriptionTitle: {
        id: "Containers.Manage.GeneralDescriptions.Detail.Description.Title",
        defaultMessage: "Kuvaus"
    },
    detailShortDescriptionTitle: {
        id: "Containers.Manage.GeneralDescriptions.Detail.ShortDescription.Title",
        defaultMessage: "Lyhyt kuvaus"
    },
    generalDescriptionResultCount: {
        id: "Containers.Manage.GeneralDescriptions.SearchResult.ResultCount.Title",
        defaultMessage: "Hakutuloksia: "
    },
    generalDescriptionResultCountHitText: {
        id: "Containers.Manage.GeneralDescriptions.SearchResult.ResultCount.HitText",
        defaultMessage: "kpl"
    },
    generalDescriptionConfirm: {
        id: "Containers.Manage.GeneralDescriptions.Search.Button.Confirm",
        defaultMessage: "Valitse pohjakuvaus"
    }
});