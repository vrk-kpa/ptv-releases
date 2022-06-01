import { useContext } from 'react';
import { useCookies } from 'react-cookie';
import { useQueryClient } from 'react-query';
import { useNavigate } from 'react-router';
import { DispatchContext } from 'context/DispatchContextProvider';
import { PtvCookieName, getCookieOptions } from 'utils/auth';

export function useLogout(): () => void {
  const dispatch = useContext(DispatchContext);
  const [, , removeCookie] = useCookies([PtvCookieName]);
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  return () => {
    removeCookie(PtvCookieName, getCookieOptions());
    queryClient.clear();
    navigate('/');
    dispatch({
      type: 'Logout',
    });
  };
}
