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
    defaultMessage: 'Valitse hakutuloslistasta sisällöt, jotka haluat käsitellä massana. Voit valita sisältöjä useiden hakujen tuloksista. Valitut sisällöt kerääntyvät alla olevan linkin taakse, jota painamalla voit tarvittaessa muokata niitä. Siirry sinisellä painikkeella erilliseen näkymään, jossa hyväksyt sisällöt yksitellen, jonka jälkeen voit julkaista kaikki sisällöt kerralla. ', // eslint-disable-line
  },
  // publishTitle: {
  //   id: 'MassTool.Options.Publish',
  //   defaultMessage: 'Julkaise',
  //   description: 'Components.Buttons.PublishButton'
  // },
  // publishTooltip: {
  //   id: 'MassTool.Options.Publish.Tooltip',
  //   defaultMessage: 'Mass Publish Tooltip'
  // },
  publishConfirmButton: {
    id: 'MassTool.Options.Publish.ReviewButton',
    defaultMessage: 'Siirry hyväksymään ja julkaisemaan',
    description: { en: 'Start review and publish' }
  },
  copySelectionInstruction: {
    id: 'MassTool.Copy.Instruction',
    defaultMessage: 'Voit kopioida ne toiselle organisaatiolle valitsemalla organisaation pudotusvalikosta ja klikkaamalla Kopioi-painiketta.', // eslint-disable-line
    description: { en: 'To copy them to another organization, select the organization drop-down menu and click the \'Copy\' button.' } // eslint-disable-line
  },
  // copyTitle: {
  //   id: 'MassTool.Copy.Title',
  //   defaultMessage: 'Kopioi',
  //   description: 'Components.Buttons.CopyButton'
  // },
  // copyTooltip: {
  //   id: 'MassTool.Copy.Tooltip',
  //   defaultMessage: 'Mass Copy Tooltip'
  // },
  copyConfirmButton: {
    id: 'MassTool.Copy.ConfirmButton',
    defaultMessage: 'Kopioi',
    description: 'Components.Buttons.CopyButton'
  },
  archiveSelectionInstruction: {
    id: 'MassTool.Archive.Instruction',
    defaultMessage: 'Voit arkistoida ne klikkaamalla Arkistoi-painiketta.',
    description: { en: 'You can archive them by clicking the \'Archive\' button.' }
  },
  // archiveTitle: {
  //   id: 'MassTool.Archive.Title',
  //   defaultMessage: 'Arkistoi',
  //   description: 'Components.Buttons.ArchiveButton'
  // },
  // archiveTooltip: {
  //   id: 'MassTool.Archive.Tooltip',
  //   defaultMessage: 'Mass Archive Tooltip'
  // },
  archiveConfirmButton: {
    id: 'MassTool.Archive.ConfirmButton',
    defaultMessage: 'Arkistoi',
    description: 'Components.Buttons.ArchiveButton'
  },
  createTaskListSelectionInstruction: {
    id: 'MassTool.CreateTaskList.Instruction',
    defaultMessage: 'Voit luoda niistä tehtävän antamalla kenttään tehtävän nimen ja painamalla Tallenna-painiketta.',
    description: { en: 'To create a task, enter a name in the field and press the Save button.' }
  },
  // createTaskListTitle: {
  //   id: 'MassTool.CreateTaskList.Title',
  //   defaultMessage: 'Luo tehtävä',
  //   description: { en: 'Create a task' }
  // },
  // createTaskListTooltip: {
  //   id: 'MassTool.CreateTaskList.Tooltip',
  //   defaultMessage: 'Create Task List Tooltip'
  // },
  createTaskListConfirmButton: {
    id: 'MassTool.CreateTaskList.ConfirmButton',
    defaultMessage: 'Talenna',
    description: 'Components.Buttons.SaveButton'
  },
  // emptySelection: {
  //   id: 'MassTool.ConfirmButton.EmptySelection.Text',
  //   defaultMessage: 'There are no services / channels selected.'
  // },
  // limitReached: {
  //   id: 'MassTool.ConfirmButton.LimitReached.Text',
  //   defaultMessage: 'You have selected more than {limit} language versions. Please, unselect exceeding records.'
  // },
  clearButton: {
    id: 'Components.Buttons.ClearButton',
    defaultMessage: 'Tyhjennä',
    description: 'ModalDialog.ClearSelectedGeneralDescription.Button.Clear'
  },
  massToolStep1Title: {
    id: 'MassTool.Step1.Title',
    defaultMessage: '1. Valitse massatoiminto'
  },
  massToolStep1Tooltip: {
    id: 'MassTool.Step1.Tooltip',
    defaultMessage: 'Mass tool step 1 tooltip'
  },
  massToolStep1Description: {
    id: 'MassTool.Step1.Description',
    defaultMessage: 'Valitse toiminto, jonka haluat suorittaa joukolle sisältöjä.'
  },
  massToolStep1PublishTitle: {
    id: 'MassTool.Step1.Publish.Title',
    defaultMessage: 'Massajulkaisu'
  },
  massToolStep1PublishDescription: {
    id: 'MassTool.Step1.Publish.Text',
    defaultMessage: 'Voit massajulkaista vain sellaisia sisältöjä, joihin sinulla on oikeudet. Tästä johtuen, hakutuloslistassa voi olla sisältöjä, joita et voi valita massajulkaisuun.' // eslint-disable-line
  },
  massToolStep1ArchiveTitle: {
    id: 'MassTool.Step1.Archive.Title',
    defaultMessage: 'Massa-arkistointi'
  },
  massToolStep1ArchiveDescription: {
    id: 'MassTool.Step1.Archive.Text',
    defaultMessage: 'Voit massa-arkistoida vain sellaisia sisältöjä, joihin sinulla on oikeudet. Tästä johtuen, hakutuloslistassa voi olla sisältöjä, joita et voi valita massa-arkistointiin.' // eslint-disable-line
  },
  massToolStep1RestoreTitle: {
    id: 'MassTool.Step1.Restore.Title',
    defaultMessage: 'Arkistosta palauttaminen'
  },
  massToolStep1RestoreDescription: {
    id: 'MassTool.Step1.Restore.Text',
    defaultMessage: 'Voit palauttaa arkistosta vain sellaisia sisältöjä, joihin sinulla on oikeudet. Tästä johtuen, hakutuloslistassa voi olla sisältöjä, joita et voi valita tähän toimintoon.' // eslint-disable-line
  },
  massToolStep1CopyTitle: {
    id: 'MassTool.Step1.Copy.Title',
    defaultMessage: 'Massakopiointi'
  },
  massToolStep1CopyDescription: {
    id: 'MassTool.Step1.Copy.Text',
    defaultMessage: 'Voit massakopioida sisältöjä kaikilta organisaatioilta omalle organisaatiolle.'
  },
  restoreConfirmButton: {
    id: 'MassTool.Restore.ConfirmButton',
    defaultMessage: 'Palauta arkistosta',
    description: 'Components.Buttons.RestoreButton'
  },
  restoreSelectionInstruction: {
    id: 'MassTool.Restore.Instruction',
    defaultMessage: 'Mass restore selection instruction'
  },
  massToolStep2Title: {
    id: 'MassTool.Step2.Title',
    defaultMessage: '2. Valitse sisällöt'
  },
  massToolStep2Tooltip: {
    id: 'MassTool.Step2.Tooltip',
    defaultMessage: 'Mass tool step 2 tooltip'
  },
  cartRemoveButton: {
    id: 'MassTool.Cart.RemoveButton.Title',
    defaultMessage: 'Poista',
    description: 'Buttons.Delete.Title'
  },
  cartClearButton: {
    id: 'MassTool.Cart.ClearButton.Title',
    defaultMessage: 'Poista kaikki'
  },
  itemAccepted: {
    id: 'MassTool.CartDialog.Review.Accepted.Title',
    defaultMessage: 'Hyväksytty'
  },
  itemNotAccepted: {
    id: 'MassTool.CartDialog.Review.NotAccepted.Title',
    defaultMessage: 'Hyväksymätön'
  },
  contentCannotBeRestore: {
    id: 'MassTool.SummaryMassToolDialog.Restore.notBeRestore.Text',
    defaultMessage: 'Sisältöä ei voi palauttaa arkistosta.'
  },
  contentExistInModifiedVersion: {
    id: 'MassTool.SummaryMassToolDialog.Restore.modiifedExist.Text',
    defaultMessage: 'Sisällöstä on jo olemassa muokkattu versio'
  }
})
