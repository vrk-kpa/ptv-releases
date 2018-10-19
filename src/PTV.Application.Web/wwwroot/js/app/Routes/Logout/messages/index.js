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
import { defineMessages } from 'util/react-intl'
// sts_EnvironmentDEV	KEHITYS
// sts_EnvironmentPROD	TUOTANTO
// sts_EnvironmentQA	HYVÄKSYMISTESTAUSYMPÄRISTÖ
// sts_EnvironmentTEST	TESTI
// sts_EnvironmentTRN	KOULUTUS
// sts_LoginButton	Kirjaudu sisään
// sts_LoginDescription	Welcome to the Suomi.fi PTV...
// sts_LoginEmail	Sähköposti
// sts_LoginEmailPlaceholder	Sähköposti
// sts_LoginHeader	Kirjaudu sisään
// sts_LoginPassword	Salasana
// sts_LoginPasswordPlaceholder	Salasana
// sts_LoginRememberMe	Muista minut
// sts_ReadMoreLink	https://esuomi.fi/palveluntarjoajille/palvelutietovaranto/
// sts_ReadMoreTitle	Lue lisää
// sts_WelcomeMessage1	Tervetuloa Suomi.fi-palvelutietovarantoon! Palvelutietovaranto on kansallinen, yhteinen tietomalli ja tietokanta. Palvelutietovarantoon kuvataan kansalaisille, yrityksille ja viranomaisille kohdistetut palvelut ja niiden asiointikanavat yhdenmukaisesti ja asiakaslähtöisesti.
// sts_WelcomeMessage2	Palvelutietovarannon sisältö on avointa dataa ja se on avoimen rajapinnan kautta käytettävissä missä tahansa muussa palvelussa.
// sts_WelcomeMessage3_1	Eikö organisaatiosi vielä kuvaa palveluita palvelutietovarantoon?
// sts_WelcomeMessage3_2	Suomi.fi-palvelutietovarannosta ja ota meihin yhteyttä!

// sts_EnvironmentDEV	UTVECKLING
// sts_EnvironmentPROD	PRODUKTION
// sts_EnvironmentQA	KVALITETTESTNING
// sts_EnvironmentTEST	TEST
// sts_EnvironmentTRN	TRÄNING
// sts_LoginButton	Logga in
// sts_LoginDescription	Welcome to the Suomi.fi PTV...
// sts_LoginEmail	E-postadress
// sts_LoginEmailPlaceholder	E-postadress
// sts_LoginHeader	Logga in
// sts_LoginPassword	Lösenord
// sts_LoginPasswordPlaceholder	Lösenord
// sts_LoginRememberMe	Kom ihåg mig
// sts_ReadMoreLink	https://esuomi.fi/suomi-fi-tjanster/suomi-fi-servicedatalager/?lang=sv
// sts_ReadMoreTitle	Läs mer
// sts_WelcomeMessage1	Välkommen till Suomi.fi-servicedatalagret! Servicedatalagret är en nationell gemensam informationsstruktur och databas. I Servicedatalagret definieras tjänster och deras kanaler för medborgare, företag och myndigheter på ett enhetligt och kundorienterat sätt.
// sts_WelcomeMessage2	Innehållet i Servicedatalagret är öppen data, och det är tillgänglig via ett öppet gränssnitt (API) för att användas i alla e-tjänster.
// sts_WelcomeMessage3_1	Är din organisations tjänster redan i Servicedatalagret?
// sts_WelcomeMessage3_2	om Suomi.fi-servicedatalagret och kontakta oss!

// sts_LogoutMessage	Olet kirjautunut ulos!
// sts_LogoutTitle	Uloskirjautuminen

// sts_LogoutMessage	Du har loggats ut!
// sts_LogoutTitle	Utloggning

// sts_EnvironmentDEV	KEHITYS
// sts_EnvironmentPROD	TUOTANTO
// sts_EnvironmentQA	HYVÄKSYMISTESTAUSYMPÄRISTÖ
// sts_EnvironmentTEST	TESTI
// sts_EnvironmentTRN	KOULUTUS
// sts_FooterAboutService	Tietoa Palvelutietovarannosta
// sts_FooterAboutServiceLink	https://esuomi.fi/palveluntarjoajille/palvelutietovaranto/
// sts_FooterFeedback	Palaute
// sts_FooterFeedbackLink	https://response.questback.com/isa/qbv.dll/ShowQuest?QuestID=4857167&sid=8gLknyGeq2
// sts_FooterPrivacyPolicy	Tietosuojaseloste
// sts_FooterPrivacyPolicyLink	https://esuomi.fi/palveluntarjoajille/palvelutietovaranto/kayttoonotto/asiakas-ja-kayttajarekisterin-tietosuojaseloste/
// sts_FooterPublicWeb	Suomi.fi-verkkopalvelu
// sts_FooterPublicWebLink	http://suomi.fi/
// sts_FooterServiceProvider	Väestörekisterikeskus
// sts_FooterServiceProviderDescription	Tämän sisällönhallintapalvelun teille tarjoaa
// sts_FooterServiceProviderLink	http://www.vrk.fi
// sts_FooterSourceCode	Lisenssitiedot ja lähdekoodi
// sts_FooterSourceCodeDescription	Suomi.fi-palvelutietovarannossa käytetään avoimen lähdekoodin ohjelmistoja.
// sts_FooterSourceCodeLink	https://github.com/vrk-kpa/ptv-releases
// sts_ServiceName	Palvelutietovaranto

// sts_EnvironmentDEV	UTVECKLING
// sts_EnvironmentPROD	PRODUKTION
// sts_EnvironmentQA	KVALITETTESTNING
// sts_EnvironmentTEST	TEST
// sts_EnvironmentTRN	TRÄNING
// sts_FooterAboutService	Information om Servicedatalagret
// sts_FooterAboutServiceLink	https://esuomi.fi/suomi-fi-tjanster/suomi-fi-servicedatalager/?lang=sv
// sts_FooterFeedback	Respons
// sts_FooterFeedbackLink	https://response.questback.com/isa/qbv.dll/ShowQuest?QuestID=4857167&sid=8gLknyGeq2
// sts_FooterPrivacyPolicy	Dataskyddsbeskrivningen
// sts_FooterPrivacyPolicyLink	https://esuomi.fi/palveluntarjoajille/palvelutietovaranto/kayttoonotto/asiakas-ja-kayttajarekisterin-tietosuojaseloste/
// sts_FooterPublicWeb	Suomi.fi-nättjänsten
// sts_FooterPublicWebLink	https://www.suomi.fi/hemsidan
// sts_FooterServiceProvider	Befolkningscentralen
// sts_FooterServiceProviderDescription	Denna innehållshanteringstjänst tillhandahålls av
// sts_FooterServiceProviderLink	http://www.vrk.fi/sv/framsida
// sts_FooterSourceCode	Licensinformation och källkoden
// sts_FooterSourceCodeDescription	I Suomi.fi-servicedatalagret användas programvara med öppen källkod.
// sts_FooterSourceCodeLink	https://github.com/vrk-kpa/ptv-releases
// sts_ServiceName	Servicedatalager

// sts_roleEeva	järjestelmäpääkäyttäjä
// sts_rolePete	pääkäyttäjä
// sts_roleShirley	ylläpitäjä

// sts_roleEeva	systemadministratör
// sts_rolePete	huvudanvändare
// sts_roleShirley	användare

// sts_roleEeva	Population Register Centre’s system administrator
// sts_rolePete	system administrator
// sts_roleShirley	administrator

export default defineMessages({
  logoffTitle: {
    id: 'Logoff.Page.Title',
    defaultMessage: 'Uloskirjautuminen'
  },
  logoffMessage: {
    id: 'Logoff.Page.Message',
    defaultMessage: 'Olet kirjautunut ulos!'
  }
})
