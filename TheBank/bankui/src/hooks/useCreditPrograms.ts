import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { creditProgramsApi } from '@/api/api';

export const useCreditPrograms = () => {
  const queryClient = useQueryClient();

  const {
    data: creditPrograms,
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: ['creditPrograms'],
    queryFn: creditProgramsApi.getAll,
  });

  const { mutate: createCreditProgram } = useMutation({
    mutationFn: creditProgramsApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['creditPrograms'] });
    },
  });

  const { mutate: updateCreditProgram } = useMutation({
    mutationFn: creditProgramsApi.update,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['creditPrograms'] });
    },
  });

  return {
    creditPrograms,
    isLoading,
    isError,
    error,
    createCreditProgram,
    updateCreditProgram,
  };
};
