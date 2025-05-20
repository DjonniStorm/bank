import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { currenciesApi } from '@/api/api';

export const useCurrencies = () => {
  const queryClient = useQueryClient();

  const {
    data: currencies,
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: ['currencies'],
    queryFn: currenciesApi.getAll,
  });

  const { mutate: createCurrency } = useMutation({
    mutationFn: currenciesApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['currencies'] });
    },
  });

  const { mutate: updateCurrency } = useMutation({
    mutationFn: currenciesApi.update,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['currencies'] });
    },
  });

  return {
    currencies,
    isLoading,
    isError,
    error,
    createCurrency,
    updateCurrency,
  };
};
