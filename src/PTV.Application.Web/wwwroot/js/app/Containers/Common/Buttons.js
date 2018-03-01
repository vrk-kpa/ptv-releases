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
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import PTVButton from "../../Components/PTVButton";
import './Styles/Buttons.scss';
import classNames from 'classnames';

const messages = defineMessages({
    draftSaveButton: {
        id: "Components.Buttons.DraftSaveButton",
        defaultMessage: "Tallenna"
    },
    saveButton: {
        id: "Components.Buttons.SaveButton",
        defaultMessage: "Tallenna"
    },
    cancelButton: {
        id: "Components.Buttons.CancelButton",
        defaultMessage: "Keskeytä"
    },
    continueButton: {
        id: "Components.Buttons.Continue",
        defaultMessage: "Jatka esikatseluun"
    },
    searchButton: {
        id: "Components.Buttons.SearchButton",
        defaultMessage: "Hae"
    },
    backButton: {
        id: "Components.Buttons.BackButton",
        defaultMessage: "< Takaisin"
    },
    doneButton: {
        id: "Components.Buttons.DoneButton",
        defaultMessage: "Valmis"
    },
    addButton: {
        id: "Components.Buttons.AddButton",
        defaultMessage: "Lisää"
    },
    deleteButton: {
        id: "Components.Buttons.DeleteButton",
        defaultMessage: "Poista"
    },
    withdrawButton: {
        id: "Components.Buttons.WithdrawButton",
        defaultMessage: "Palauta luonnokseksi"
    },
    updateButton: {
        id: "Components.Buttons.UpdateButton",
        defaultMessage: "Muokkaa"
    },
    cancelUpdateButton: {
        id: "Components.Buttons.CancelUpdateButton",
        defaultMessage: "Keskeytä"
    },
    publishButton: {
        id: "Components.Buttons.PublishButton",
        defaultMessage: "Julkaise"
    },
    restoreButton: {
        id: "Components.Buttons.RestoreButton",
        defaultMessage: 'Palauta arkistosta'
    },
    showMoreButton: {
        id: "Components.Buttons.ShowMoreButton",
        defaultMessage: "Näytä lisää"
    },
    archiveButton: {
        id: "Components.Buttons.ArchiveButton",
        defaultMessage: "Arkistoi"
    },
    buttonClearList: {
        id: "Components.Buttons.ClearList",
        defaultMessage: "Tyhjennä lista"
    }
});

let getOptionProperties = (props, title, className) => {
    return {
        item: props.item,
        onClick: props.onClick,
        title,
        disabled: props.disabled,
        className: classNames(className, props.className)
    }
}

const propTypesDef = {
        onClick: PropTypes.func.isRequired,
        item: PropTypes.any,
        buttonClass: PropTypes.string,
        disabled: PropTypes.bool,
        withIcon: PropTypes.bool,
        iconName: PropTypes.string,
        secondary: PropTypes.bool,
        small: PropTypes.bool
    };

// let CreateButton = ({item, onClick, disabled, title, className, buttonClass}) => {
//     const props = { item, onClick, disabled, className: classNames(className, buttonClass)}
//     return (
//         <PTVButton {...props}><FormattedMessage {...title}/></PTVButton>
//         )
// }

let DefineButton = (title, buttonClass) => {
    return ({item, onClick, disabled, className, withIcon, iconName, secondary, small, children}) =>{
        const props = { item, onClick, disabled, className: classNames(className, buttonClass), withIcon, iconName, secondary, small};
        return (
            <PTVButton {...props}>
                {children ?
                    children :
                    <FormattedMessage {...title}/>
                }
            </PTVButton>
            )
        }
}


export const ButtonSaveDraft = DefineButton(messages.draftSaveButton, "button-draft-save");
export const ButtonSave = DefineButton(messages.saveButton, "button-save");
export const ButtonCancel = DefineButton(messages.cancelButton, "button-cancel");
export const ButtonArchive = DefineButton(messages.archiveButton, "button-archive");
export const ButtonContinue = DefineButton(messages.continueButton, "button-continue");
export const ButtonSearch = DefineButton(messages.searchButton, "button-search");
export const ButtonGoBack = DefineButton(messages.backButton, "button-back");
export const ButtonDone = DefineButton(messages.doneButton, "button-done");
export const ButtonAdd = DefineButton(messages.addButton, "button-add");
export const ButtonDelete = DefineButton(messages.deleteButton, "button-delete");
export const ButtonWithdraw = DefineButton(messages.withdrawButton, "button-withdraw");
export const ButtonUpdate = DefineButton(messages.updateButton, "button-update");
export const ButtonCancelUpdate = DefineButton(messages.cancelUpdateButton, "button-cancel-update");
export const ButtonPublish = DefineButton(messages.publishButton, "button-publish");
export const ButtonRestore = DefineButton(messages.restoreButton, "button-restore");
export const ButtonShowMore = DefineButton(messages.showMoreButton, "button-show-more");
export const ButtonClearList = DefineButton(messages.buttonClearList, "button-clear-list");

ButtonSaveDraft.propTypes = propTypesDef;
ButtonCancel.propTypes = propTypesDef;
ButtonContinue.propTypes = propTypesDef;
ButtonSearch.propTypes = propTypesDef;
ButtonGoBack.propTypes = propTypesDef;
ButtonDone.propTypes = propTypesDef;
ButtonAdd.propTypes = propTypesDef;
ButtonSave.propTypes = propTypesDef;
ButtonDelete.propTypes = propTypesDef;
ButtonWithdraw.propTypes = propTypesDef;
ButtonUpdate.propTypes = propTypesDef;
ButtonCancelUpdate.propTypes = propTypesDef;
ButtonPublish.propTypes = propTypesDef;
ButtonRestore.propTypes = propTypesDef;
ButtonShowMore.propTypes = propTypesDef;
ButtonArchive.propTypes = propTypesDef;
ButtonClearList.propTypes = propTypesDef;

export default {
    ButtonSaveDraft,
    ButtonCancel,
    ButtonContinue,
    ButtonSearch,
    ButtonGoBack,
    ButtonDone,
    ButtonAdd,
    ButtonSave,
    ButtonDelete,
    ButtonWithdraw,
    ButtonUpdate,
    ButtonCancelUpdate,
    ButtonRestore,
    ButtonPublish,
    ButtonShowMore,
    ButtonArchive,
    ButtonClearList
}
