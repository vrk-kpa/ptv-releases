import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { RhfTextInput, RhfTextarea } from 'fields';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cC, cWebPageLv } from 'types/forms/connectionFormTypes';
import { VisualHeading } from 'components/VisualHeading';
import { FormBlock } from 'features/connection/components/FormLayout';
import { toFieldId } from 'features/connection/utils/fieldid';

const useStyles = makeStyles(() => ({
  root: {
    display: 'flex',
    flexGrow: 1,
    '& p.noTopMargin': {
      marginTop: 0,
    },
  },
}));

type WebPageProps = {
  webPageIndex: number;
  control: Control<ConnectionFormModel>;
  language: Language;
};

export function WebPage(props: WebPageProps): React.ReactElement {
  const { t } = useTranslation();
  const lang = props.language;
  const classes = useStyles();

  const name = `${cC.webPages}.${lang}.${props.webPageIndex}.${cWebPageLv.name}`;
  const url = `${cC.webPages}.${lang}.${props.webPageIndex}.${cWebPageLv.url}`;
  const info = `${cC.webPages}.${lang}.${props.webPageIndex}.${cWebPageLv.additionalInformation}`;

  return (
    <div className={classes.root}>
      <Grid container direction='column'>
        <Grid item>
          <VisualHeading variant='h4' className='noTopMargin'>
            {t('Ptv.ConnectionDetails.WebPage.MainTitle')}
          </VisualHeading>
        </Grid>
        <Grid item>
          <FormBlock marginTop='20px'>
            <RhfTextInput
              control={props.control}
              name={name}
              id={toFieldId(name)}
              mode='edit'
              labelText={t('Ptv.ConnectionDetails.WebPage.Name.Label')}
              hintText={t('Ptv.ConnectionDetails.WebPage.Name.Hint')}
              visualPlaceholder={t('Ptv.ConnectionDetails.WebPage.Name.Placeholder')}
            />
          </FormBlock>
        </Grid>
        <Grid item>
          <FormBlock marginTop='30px'>
            <RhfTextarea
              control={props.control}
              name={url}
              id={toFieldId(url)}
              mode='edit'
              labelText={t('Ptv.ConnectionDetails.WebPage.Url.Label')}
              hintText={t('Ptv.ConnectionDetails.WebPage.Url.Hint')}
              visualPlaceholder={t('Ptv.ConnectionDetails.WebPage.Url.Placeholder')}
            />
          </FormBlock>
        </Grid>
        <Grid item>
          <FormBlock marginTop='30px'>
            <RhfTextarea
              control={props.control}
              name={info}
              id={toFieldId(info)}
              mode='edit'
              labelText={t('Ptv.ConnectionDetails.WebPage.Description.Label')}
              optionalText={t('Ptv.Common.Optional')}
              hintText={t('Ptv.ConnectionDetails.WebPage.Description.Hint')}
              visualPlaceholder={t('Ptv.ConnectionDetails.WebPage.Description.Placeholder')}
            />
          </FormBlock>
        </Grid>
      </Grid>
    </div>
  );
}
