import React from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Grid from '@mui/material/Grid';
import { makeStyles } from '@mui/styles';
import { RhfTextInput } from 'fields';
import { Button, Heading, Paragraph } from 'suomifi-ui-components';
import { LoginModel } from 'types/loginTypes';
import LoadingIndicator from 'components/LoadingIndicator';
import { useGetAccessGroupId } from 'hooks/auth/useGetAccessGroupId';
import { useLogin } from 'hooks/auth/useLogin';
import { getSettings } from 'utils/settings';
import { validateLoginForm } from 'features/login/validation/cloudLoginValidation';

const useStyles = makeStyles(() => ({
  root: {
    display: 'flex',
    flex: 1,
  },
  item: {
    marginTop: '20px',
  },
  field: {
    marginTop: '8px',
  },
  loadingIndicator: {
    display: 'flex',
    flex: 1,
    justifyContent: 'center',
    alignItems: 'flex-start',
  },
}));

export default function CloudLogin(): React.ReactElement {
  const classes = useStyles();
  const settings = getSettings();
  const { isLoading, accessGroupId } = useGetAccessGroupId(settings.environmentType);

  const { t } = useTranslation();

  const onError = (error: unknown) => {
    console.log('Login failed', error);
  };

  const defaultValues: LoginModel = {
    name: '',
    password: '',
    organization: null,
    userAccessRightsGroup: accessGroupId === undefined ? null : accessGroupId,
  };

  const { control, getValues, formState } = useForm<LoginModel>({
    defaultValues: defaultValues,
    mode: 'all',
    resolver: validateLoginForm,
  });

  const login = useLogin(settings.isFakeAuthenticationEnabled, settings.environmentType, onError);

  if (isLoading) {
    return (
      <div className={classes.loadingIndicator}>
        <LoadingIndicator />
      </div>
    );
  }

  const isValid = formState.isValid && !formState.isValidating;

  function onLogin() {
    const values = getValues();
    const model: LoginModel = {
      name: values.name,
      organization: null,
      password: values.password,
      userAccessRightsGroup: defaultValues.userAccessRightsGroup,
    };

    login.mutate(model);
  }

  return (
    <Grid container direction='column'>
      <Grid item className={classes.item}>
        <Paragraph>{t('Ptv.Login.Form.WelcomeMessage1')}</Paragraph>
      </Grid>
      <Grid item className={classes.item}>
        <Paragraph>{t('Ptv.Login.Form.WelcomeMessage2')}</Paragraph>
      </Grid>
      <Grid item className={classes.item}>
        <Paragraph>{t('Ptv.Login.Form.WelcomeMessage3')}</Paragraph>
      </Grid>

      <Grid item className={classes.item}>
        <form className={classes.root}>
          <Grid container justifyContent='space-between' direction='column'>
            <Grid item>
              <Heading variant='h2'>{t('Ptv.Login.Form.Title.Text')}</Heading>
            </Grid>
            <Grid item className={classes.field}>
              <RhfTextInput
                control={control}
                id='name'
                name='name'
                mode='edit'
                fullWidth={false}
                labelText={t('Ptv.Login.Form.Field.Email.Label')}
              />
            </Grid>
            <Grid item className={classes.field}>
              <RhfTextInput
                control={control}
                id='password'
                name='password'
                mode='edit'
                type='password'
                fullWidth={false}
                labelText={t('Ptv.Login.Form.Field.Password.Label')}
              />
            </Grid>

            <Grid item className={classes.field}>
              <Button onClick={onLogin} disabled={!isValid}>
                {t('Ptv.Login.Form.Button.Login.Label')}
              </Button>
            </Grid>
          </Grid>
        </form>
      </Grid>
    </Grid>
  );
}
