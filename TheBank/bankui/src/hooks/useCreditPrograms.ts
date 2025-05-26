import { useQuery } from '@tanstack/react-query';
import { creditProgramsApi } from '@/api/api';
import type { CreditProgramBindingModel } from '@/types/types';

export const useCreditPrograms = () => {
  const {
    data: creditPrograms,
    isLoading,
    error,
  } = useQuery<CreditProgramBindingModel[]>({
    queryKey: ['creditPrograms'],
    queryFn: creditProgramsApi.getAll,
  });

  return {
    creditPrograms,
    isLoading,
    error,
  };
};
