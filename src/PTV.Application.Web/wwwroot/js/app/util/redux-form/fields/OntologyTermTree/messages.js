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
import { defineMessages } from 'react-intl'

export const messages = defineMessages({
  tooltip: {
    id: 'Containers.Services.AddService.Step2.OntologyTerms.Tooltip',
    defaultMessage: 'Palvelun asiasisältö kuvataan ontologiakäsitteillä, joiden käyttö helpottaa palveluiden löytämistä. Kirjoita palvelun asiasisältöä mahdollisimman tarkasti kuvaava sana ja valitse ennakoivan tekstinsyötön tarjoamista vaihtoehdoista ontologiakäsite. Valitse palvelun kuvaamiseksi vähintään yksi ja enintään kymmenen käsitettä. Mikäli olet käyttänyt palvelun pohjakuvausta, kenttään on kopioitu valmiiksi pohjakuvauksen ontologiakäsitteet. Voit tarvittaessa lisätä uusia käsitteitä. '
  },
  annotationListHeader: {
    id: 'Containers.Services.AddService.Step2.OntologyTerms.AnnotationList.Header',
    defaultMessage: 'Lisätyt asiasanat (voit poistaa klikkaamalla rastia)'
  },
  annotationTitle:{
    id: 'Containers.Services.AddService.Step2.OntologyTerms.AnnotationTitle',
    defaultMessage: 'Palvelu ehdotta asiasanoja täyttämäsi sisällön mukaan. Hae suositellut asiasanat automaattisesti painikkeella Hae asiasanat tai poimi haluamasi sanat asiasanalistasta.'
  },
  annotationButton:{
    id: 'Containers.Services.AddService.Step2.OntologyTerms.AnnotationButton',
    defaultMessage: 'Hae asiasanat'
  },
  annotationToolAlert:{
    id: 'Containers.Services.AddService.Step2.OntologyTerms.AnnotationToolAlert',
    defaultMessage: 'Annotation tool is disabled due to technical reasons'
  },
  treeLink:{
    id: 'Containers.Services.AddService.Step2.OntologyTerms.TreeLink',
    defaultMessage: 'Hae ja lisää asiasanoja listasta'
  },
  annotationTagPostfix:{
    id: 'Containers.GeneralDescription.OntologyTerms.Annotation.TagPostfix',
    defaultMessage: 'Ehdotettu asiasana'
  },
  annotationTagTooltip:{
    id: 'Containers.GeneralDescription.OntologyTerms.Annotation.TagTooltip',
    defaultMessage: 'Asiasanan sijainti käsitehierarkiassa:'
  },
  targetListHeader: {
    id: 'Containers.Services.AddService.Step2.OntologyTerms.TargetList.Header',
    defaultMessage: 'Pohjakuvauksen asiasanat (ei voi poistaa)'
  }
})
