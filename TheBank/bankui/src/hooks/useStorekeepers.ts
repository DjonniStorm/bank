import { storekeepersApi } from '@/api/api';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

export const useStorekeepers = () => {
  const queryClient = useQueryClient();

  const {
    data: storekeepers,
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: ['storekeepers'],
    queryFn: storekeepersApi.getAll,
  });

  const { mutate: createStorekeeper } = useMutation({
    mutationFn: storekeepersApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['storekeepers'] });
    },
  });

  const { mutate: updateStorekeeper } = useMutation({
    mutationFn: storekeepersApi.update,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['storekeepers'] });
    },
  });

  return {
    storekeepers,
    isLoading,
    isError,
    error,
    createStorekeeper,
    updateStorekeeper,
  };
};
