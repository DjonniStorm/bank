import { clerksApi } from '@/api/api';
import { useAuthStore } from '@/store/workerStore';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useNavigate, useLocation } from 'react-router-dom';

export const useClerks = () => {
  const queryClient = useQueryClient();
  const setAuth = useAuthStore((store) => store.setAuth);
  const navigate = useNavigate();
  const location = useLocation();

  const {
    data: clerks,
    isLoading,
    isError: isGetAllError,
    error,
  } = useQuery({
    queryKey: ['clerks'],
    queryFn: clerksApi.getAll,
  });

  const { mutate: createClerk, isError: isCreateError } = useMutation({
    mutationFn: clerksApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clerks'] });
    },
  });

  const {
    mutate: updateClerk,
    isError: isUpdateError,
    error: updateError,
  } = useMutation({
    mutationFn: clerksApi.update,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clerks'] });
    },
  });

  const {
    mutate: loginClerk,
    isError: isLoginError,
    isSuccess: isLoginSuccess,
    error: loginError,
  } = useMutation({
    mutationFn: clerksApi.login,
    onSuccess: (userData) => {
      setAuth(userData);
      const params = new URLSearchParams(location.search);
      const redirect = params.get('redirect') || '/clerks';
      navigate(redirect);
      queryClient.invalidateQueries({ queryKey: ['clerk'] });
    },
  });

  return {
    clerks,
    isLoading,
    isGetAllError,
    isCreateError,
    isLoginError,
    isUpdateError,
    isLoginSuccess,
    loginError,
    updateError,
    error,
    createClerk,
    loginClerk,
    updateClerk,
  };
};
