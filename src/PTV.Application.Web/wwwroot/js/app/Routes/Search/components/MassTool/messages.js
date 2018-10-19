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

export const messages = defineMessages({
  publishSelectionInstruction: {
    id: 'MassTool.Options.Publish.Instruction',
    defaultMessage: 'Olet valinnut {selected} sisältöä (yhteensä {totalCount} kieliversiota). Siirry ”Tarkista ja julkaise” -painikkeella erilliseen näkymään, jossa voit tarkistaa ja hyväksyä sisällöt yksitellen ja julkaista ne sitten yhdellä kertaa. Voit rajata kieliä tippuvalikosta. Siirry painikkeella erilliseen näkymään, jossa hyväksyt sisällöt yksitellen ja voit julkaista ne yhdellä kertaa.', // eslint-disable-line
    description: {
      sv: 'Du har valt {selected} innehåll sammanlagt {totalCount} språkversioner). Klicka på "Granska och publicera" för att gå till en separat vy där du kan granska och godkänna innehåll separat och sedan publicera allt innehåll samtidigt.', // eslint-disable-line
      en: 'You have selected {selected} services / channels (total {totalCount} language versions). Click on the “Check and publish” icon to move to a separate view, where you can check and approve pieces of content one by one or publish them all at one time. You can narrow down the languages to review. To start the review of the content, click \'Start review and publish\' link.' // eslint-disable-line
    }
  },
  publishTitle: {
    id: 'MassTool.Options.Publish',
    defaultMessage: 'Julkaise',
    description: 'Components.Buttons.PublishButton'
  },
  publishTooltip: {
    id: 'MassTool.Options.Publish.Tooltip',
    defaultMessage: 'Mass Publish Tooltip'
  },
  publishConfrimButton: {
    id: 'MassTool.Options.Publish.ReviewButton',
    defaultMessage: 'Siirry hyväksymään ja julkaisemaan',
    description: { en: 'Start review and publish' }
  },
  copySelectionInstruction: {
    id: 'MassTool.Copy.Instruction',
    defaultMessage: 'Voit kopioida ne toiselle organisaatiolle valitsemalla organisaation pudotusvalikosta ja klikkaamalla Kopioi-painiketta.', // eslint-disable-line
    description: { en: 'To copy them to another organization, select the organization drop-down menu and click the \'Copy\' button.' } // eslint-disable-line
  },
  copyTitle: {
    id: 'MassTool.Copy.Title',
    defaultMessage: 'Kopioi',
    description: 'Components.Buttons.CopyButton'
  },
  copyTooltip: {
    id: 'MassTool.Copy.Tooltip',
    defaultMessage: 'Mass Copy Tooltip'
  },
  copyConfrimButton: {
    id: 'MassTool.Copy.ConfirmButton',
    defaultMessage: 'Kopioi',
    description: 'Components.Buttons.CopyButton'
  },
  archiveSelectionInstruction: {
    id: 'MassTool.Archive.Instruction',
    defaultMessage: 'Voit arkistoida ne klikkaamalla Arkistoi-painiketta.',
    description: { en: 'You can archive them by clicking the \'Archive\' button.' }
  },
  archiveTitle: {
    id: 'MassTool.Archive.Title',
    defaultMessage: 'Arkistoi',
    description: 'Components.Buttons.ArchiveButton'
  },
  archiveTooltip: {
    id: 'MassTool.Archive.Tooltip',
    defaultMessage: 'Mass Archive Tooltip'
  },
  archiveConfrimButton: {
    id: 'MassTool.Archive.ConfirmButton',
    defaultMessage: 'Arkistoi',
    description: 'Components.Buttons.ArchiveButton'
  },
  createTaskListSelectionInstruction: {
    id: 'MassTool.CreateTaskList.Instruction',
    defaultMessage: 'Voit luoda niistä tehtävän antamalla kenttään tehtävän nimen ja painamalla Tallenna-painiketta.',
    description: { en: 'To create a task, enter a name in the field and press the Save button.' }
  },
  createTaskListTitle: {
    id: 'MassTool.CreateTaskList.Title',
    defaultMessage: 'Luo tehtävä',
    description: { en: 'Create a task' }
  },
  createTaskListTooltip: {
    id: 'MassTool.CreateTaskList.Tooltip',
    defaultMessage: 'Create Task List Tooltip'
  },
  createTaskListConfrimButton: {
    id: 'MassTool.CreateTaskList.ConfirmButton',
    defaultMessage: 'Talenna',
    description: 'Components.Buttons.SaveButton'
  },
  // emptyFilteresSelection: {
  //   id: 'MassTool.ConfirmButton.EmptyFilteredSelection.Text',
  //   defaultMessage: 'There are no services / channels matching language filter. Please, select entities which include selected languages or change language filter.' // eslint-disable-line
  // },
  emptySelection: {
    id: 'MassTool.ConfirmButton.EmptySelection.Text',
    defaultMessage: 'There are no services / channels selected.'
  },
  limitReached: {
    id: 'MassTool.ConfirmButton.LimitReached.Text',
    defaultMessage: 'You have selected more than {limit} language versions. Please, unselect exceeding records.'
  },
  clearButton: {
    id: 'Components.Buttons.ClearButton',
    defaultMessage: 'Tyhjennä',
    description: 'ModalDialog.ClearSelectedGeneralDescription.Button.Clear'
  }
})
