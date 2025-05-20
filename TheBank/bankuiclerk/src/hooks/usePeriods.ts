import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { periodsApi } from '@/api/api';

export const usePeriods = () => {
  const queryClient = useQueryClient();

  const {
    data: periods,
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: ['periods'],
    queryFn: periodsApi.getAll,
  });

  const { mutate: createPeriod } = useMutation({
    mutationFn: periodsApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['periods'] });
    },
  });

  const { mutate: updatePeriod } = useMutation({
    mutationFn: periodsApi.update,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['periods'] });
    },
  });

  return {
    periods,
    isLoading,
    isError,
    error,
    createPeriod,
    updatePeriod,
  };
};
