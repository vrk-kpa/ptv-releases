import React from 'react';
import { makeStyles } from '@mui/styles';
import { Text } from 'suomifi-ui-components';
import { OrganizationModel } from 'types/organizationTypes';
import useTranslateLocalizedText from 'hooks/useTranslateLocalizedText';

const useStyles = makeStyles(() => ({
  flex: {
    display: 'flex',
  },
}));

type UserInfoProps = {
  firstName?: string | null;
  lastName?: string | null;
  organization?: OrganizationModel | undefined;
};

export default function UserInfo(props: UserInfoProps): React.ReactElement {
  const classes = useStyles();
  const translate = useTranslateLocalizedText();

  const getOrganizationName = () => {
    if (!props.organization) {
      return null;
    }

    return (
      <div className={classes.flex}>
        <Text smallScreen={true}>{translate(props.organization.texts)}</Text>
      </div>
    );
  };

  return (
    <div>
      <div className={classes.flex}>
        <Text smallScreen={true} variant='bold'>
          {props.firstName} {props.lastName}
        </Text>
      </div>
      {getOrganizationName()}
    </div>
  );
}
