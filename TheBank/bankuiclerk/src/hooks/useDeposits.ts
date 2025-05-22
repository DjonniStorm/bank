import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { depositsApi } from '@/api/api';

export const useDeposits = () => {
  const queryClient = useQueryClient();

  const {
    data: deposits,
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: ['deposits'],
    queryFn: depositsApi.getAll,
  });

  const { mutate: createDeposit } = useMutation({
    mutationFn: depositsApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['deposits'] });
    },
  });

  const { mutate: updateDeposit } = useMutation({
    mutationFn: depositsApi.update,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['deposits'] });
    },
  });

  return {
    deposits,
    isLoading,
    isError,
    error,
    createDeposit,
    updateDeposit,
  };
};
