import { useQuery } from '@tanstack/react-query';
import { useAuthStore } from '@/store/workerStore';
import { storekeepersApi } from '@/api/api';
import { useNavigate, useLocation } from 'react-router-dom';
import { type StorekeeperBindingModel } from '@/types/types';
import { useEffect } from 'react';

export const useAuthCheck = () => {
  const setAuth = useAuthStore((store) => store.setAuth);
  const logout = useAuthStore((store) => store.logout);
  const navigate = useNavigate();
  const location = useLocation();

  const {
    isPending: isLoading,
    error,
    isError,
  } = useQuery<StorekeeperBindingModel, Error>({
    queryKey: ['authCheck'],
    queryFn: async () => {
      const userData = await storekeepersApi.getCurrentUser();
      setAuth(userData);
      return userData;
    },
    retry: false,
  });

  useEffect(() => {
    if (isError) {
      console.error('Auth check failed:', error?.message);
      logout();
      const redirect = encodeURIComponent(location.pathname + location.search);
      navigate(`/auth?redirect=${redirect}`);
    }
  }, [isError, error, logout, navigate, location]);

  return { isLoading, error };
};
