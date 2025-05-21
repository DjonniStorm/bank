import { clerksApi } from '@/api/api';
import type { ClerkBindingModel } from '@/types/types';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

type AuthState = {
  user?: ClerkBindingModel;
  setAuth: (user: ClerkBindingModel) => void;
  updateUser: (user: ClerkBindingModel) => void;
  getUser: () => ClerkBindingModel | undefined;
  logout: () => Promise<void>;
};

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      user: undefined,
      setAuth: (user: ClerkBindingModel) => {
        set({ user: user });
      },
      updateUser: (user: ClerkBindingModel) => {
        set({ user: user });
      },
      getUser: () => {
        return get().user;
      },
      logout: async () => {
        await clerksApi.logout();
        set({ user: undefined });
      },
    }),
    {
      name: 'auth-storage',
    },
  ),
);
