import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { clientsApi } from '@/api/api';

export const useClients = () => {
  const queryClient = useQueryClient();

  const {
    data: clients,
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: ['clients'],
    queryFn: clientsApi.getAll,
  });

  const { mutate: createClient } = useMutation({
    mutationFn: clientsApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clients'] });
    },
  });

  const { mutate: updateClient } = useMutation({
    mutationFn: clientsApi.update,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clients'] });
    },
  });

  return {
    clients,
    isLoading,
    isError,
    error,
    createClient,
    updateClient,
  };
};
