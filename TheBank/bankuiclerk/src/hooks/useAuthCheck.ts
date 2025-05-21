import { useQuery } from '@tanstack/react-query';
import { useAuthStore } from '@/store/workerStore';
import { clerksApi } from '@/api/api';
import { type ClerkBindingModel } from '@/types/types';

export const useAuthCheck = () => {
  const setAuth = useAuthStore((store) => store.setAuth);
  const logout = useAuthStore((store) => store.logout);

  const {
    isPending: isLoading,
    error,
    isError,
  } = useQuery<ClerkBindingModel, Error>({
    queryKey: ['authCheck'],
    queryFn: async () => {
      const userData = await clerksApi.getCurrentUser();
      setAuth(userData);
      return userData;
    },
    retry: false,
  });

  if (isError) {
    console.error('Auth check failed:', error?.message);
    logout();
  }

  return { isLoading, error };
};
