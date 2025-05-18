import { storekeepersApi } from '@/api/api';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

export const useStorekeepers = () => {
  const queryClient = useQueryClient();

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

  const { mutate: updateStorekeeper, isError: isUpdateError } = useMutation({
    mutationFn: storekeepersApi.update,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['storekeepers'] });
    },
  });

  const {
    mutate: loginStorekeeper,
    isError: isLoginError,
    error: loginError,
  } = useMutation({
    mutationFn: storekeepersApi.login,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['storekeepers'] });
    },
  });

  return {
    storekeepers,
    isLoading,
    isGetAllError,
    isCreateError,
    isLoginError,
    loginError,
    error,
    createStorekeeper,
    loginStorekeeper,
    updateStorekeeper,
  };
};
