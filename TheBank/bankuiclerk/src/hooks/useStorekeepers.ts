import { storekeepersApi } from '@/api/api';
import { useAuthStore } from '@/store/workerStore';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useNavigate, useLocation } from 'react-router-dom';

export const useStorekeepers = () => {
  const queryClient = useQueryClient();
  const setAuth = useAuthStore((store) => store.setAuth);
  const navigate = useNavigate();
  const location = useLocation();

  const {
    data: storekeepers,
    isLoading,
    isError: isGetAllError,
    error,
  } = useQuery({
    queryKey: ['storekeepers'],
    queryFn: storekeepersApi.getAll,
  });

  const { mutate: createStorekeeper, isError: isCreateError } = useMutation({
    mutationFn: storekeepersApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['storekeepers'] });
    },
  });

  const {
    mutate: updateStorekeeper,
    isError: isUpdateError,
    error: updateError,
  } = useMutation({
    mutationFn: storekeepersApi.update,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['storekeepers'] });
    },
  });

  const {
    mutate: loginStorekeeper,
    isError: isLoginError,
    isSuccess: isLoginSuccess,
    error: loginError,
  } = useMutation({
    mutationFn: storekeepersApi.login,
    onSuccess: (userData) => {
      setAuth(userData);
      const params = new URLSearchParams(location.search);
      const redirect = params.get('redirect') || '/storekeepers';
      navigate(redirect);
      queryClient.invalidateQueries({ queryKey: ['storekeeper'] });
    },
  });

  return {
    storekeepers,
    isLoading,
    isGetAllError,
    isCreateError,
    isLoginError,
    isUpdateError,
    isLoginSuccess,
    loginError,
    updateError,
    error,
    createStorekeeper,
    loginStorekeeper,
    updateStorekeeper,
  };
};
