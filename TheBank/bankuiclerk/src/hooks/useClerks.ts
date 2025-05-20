import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { clerksApi } from '@/api/api';

export const useClerks = () => {
  const queryClient = useQueryClient();

  const {
    data: clerks,
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: ['clerks'],
    queryFn: clerksApi.getAll,
  });

  const { mutate: createClerk } = useMutation({
    mutationFn: clerksApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clerks'] });
    },
  });

  const { mutate: updateClerk } = useMutation({
    mutationFn: clerksApi.update,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clerks'] });
    },
  });

  return {
    clerks,
    isLoading,
    isError,
    error,
    createClerk,
    updateClerk,
  };
};
