import { storekeepersApi } from '@/api/api';
import type { StorekeeperBindingModel } from '@/types/types';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

type AuthState = {
  user?: StorekeeperBindingModel;
  setAuth: (user: StorekeeperBindingModel) => void;
  updateUser: (user: StorekeeperBindingModel) => void;
  getUser: () => StorekeeperBindingModel | undefined;
  logout: () => Promise<void>;
};

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      user: undefined,
      setAuth: (user: StorekeeperBindingModel) => {
        set({ user: user });
      },
      updateUser: (user: StorekeeperBindingModel) => {
        set({ user: user });
      },
      getUser: () => {
        return get().user;
      },
      logout: async () => {
        await storekeepersApi.logout();
        set({ user: undefined });
      },
    }),
    {
      name: 'auth-storage',
    },
  ),
);
