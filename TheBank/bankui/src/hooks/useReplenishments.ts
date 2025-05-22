import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { replenishmentsApi } from '@/api/api';

export const useReplenishments = () => {
  const queryClient = useQueryClient();

  const {
    data: replenishments,
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: ['replenishments'],
    queryFn: replenishmentsApi.getAll,
  });

  const { mutate: createReplenishment } = useMutation({
    mutationFn: replenishmentsApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['replenishments'] });
    },
  });

  const { mutate: updateReplenishment } = useMutation({
    mutationFn: replenishmentsApi.update,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['replenishments'] });
    },
  });

  return {
    replenishments,
    isLoading,
    isError,
    error,
    createReplenishment,
    updateReplenishment,
  };
};
