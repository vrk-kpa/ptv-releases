import React from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Grid from '@mui/material/Grid';
import { makeStyles } from '@mui/styles';
import { RhfTextInput } from 'fields';
import { Button, Heading, Paragraph } from 'suomifi-ui-components';
import { FakeLoginModel, LoginModel } from 'types/loginTypes';
import { ServerErrorBox } from 'components/ErrorBox/ServerErrorBox';
import LoadingIndicator from 'components/LoadingIndicator';
import { useLogin } from 'hooks/auth/useLogin';
import { usePublicEnumTypesQuery } from 'hooks/queries/usePublicEnumTypesQuery';
import { getSettings } from 'utils/settings';
import AccessGroups from 'features/login/components/AccessGroups';
import Organizations from 'features/login/components/Organizations';

const useStyles = makeStyles(() => ({
  root: {
    display: 'flex',
    flex: 1,
    maxWidth: '500px',
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

export default function FakeLogin(): React.ReactElement {
  const settings = getSettings();
  const classes = useStyles();
  const enumQuery = usePublicEnumTypesQuery();
  const { t } = useTranslation();

  const groups = enumQuery.data?.data.UserAccessRightsGroups || [];

  const onError = (error: unknown) => {
    console.log('Login failed', error);
  };

  const login = useLogin(settings.isFakeAuthenticationEnabled, settings.environmentType, onError);

  const defaultValues: FakeLoginModel = {
    name: '',
    password: '',
    userAccessRightsGroup: groups.length !== 0 ? groups[0].id : null,
    organization: null,
  };

  const { control, getValues } = useForm<FakeLoginModel>({
    defaultValues: defaultValues,
    mode: 'all',
  });

  const isLoading = enumQuery.isLoading;
  const error = enumQuery.error;

  if (isLoading) {
    return (
      <div className={classes.loadingIndicator}>
        <LoadingIndicator />
      </div>
    );
  }

  if (error) {
    return <ServerErrorBox httpError={error} />;
  }

  function onLogin() {
    const values = getValues();

    const model: LoginModel = {
      name: values.name,
      organization: values.organization?.id ? values.organization.id : null,
      password: values.password,
      userAccessRightsGroup: values.userAccessRightsGroup,
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
          <Grid container direction='column'>
            <Grid item>
              <Heading variant='h2'>{t('Ptv.Login.Form.Title.Text')}</Heading>
            </Grid>
            <Grid item className={classes.field}>
              <RhfTextInput
                control={control}
                id='name'
                name='name'
                mode='edit'
                fullWidth={true}
                labelText={t('Ptv.Login.Form.Field.Email.Label')}
              />
            </Grid>
            <Grid item className={classes.field}>
              <Organizations control={control} />
            </Grid>
            <Grid item className={classes.field}>
              <AccessGroups control={control} groups={groups} />
            </Grid>
            <Grid item className={classes.field}>
              <Button id='login-button' onClick={onLogin}>
                {t('Ptv.Login.Form.Button.Login.Label')}
              </Button>
            </Grid>
          </Grid>
        </form>
      </Grid>
    </Grid>
  );
}
