import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Menu as MenuIcon } from '@mui/icons-material';
import { Menu as MaterialMenu } from '@mui/material';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import MenuItem from '@mui/material/MenuItem';
import { useLogout } from 'hooks/auth/useLogout';

export default function Menu(): React.ReactElement {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const logout = useLogout();
  const open = Boolean(anchorEl);
  const { t } = useTranslation();

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const onLogout = () => {
    logout();
    setAnchorEl(null);
  };

  return (
    <div>
      <IconButton aria-label='more' aria-controls='long-menu' aria-haspopup='true' onClick={handleClick}>
        <MenuIcon />
      </IconButton>
      <MaterialMenu id='long-menu' anchorEl={anchorEl} keepMounted open={open} onClose={handleClose}>
        <MenuItem key='logout' onClick={onLogout}>
          {t('Ptv.Header.Menu.Logout.Text')}
        </MenuItem>
        <Divider />
        <MenuItem key='profile' onClick={handleClose}>
          {t('Ptv.Header.Menu.OwnProfile.Text')}
        </MenuItem>
        <MenuItem key='organization' onClick={handleClose}>
          {t('Ptv.Header.Menu.OwnOrganization.Text')}
        </MenuItem>
        <MenuItem key='management' onClick={handleClose}>
          {t('Ptv.Header.Menu.UserManagement.Text')}
        </MenuItem>
        <Divider />
        <MenuItem key='statistics' onClick={handleClose}>
          {t('Ptv.Header.Menu.Statistics.Text')}
        </MenuItem>
        <MenuItem key='support' onClick={handleClose}>
          {t('Ptv.Header.Menu.Support.Text')}
        </MenuItem>
      </MaterialMenu>
    </div>
  );
}
