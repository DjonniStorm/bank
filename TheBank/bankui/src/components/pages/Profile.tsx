import React from 'react';
import { useAuthStore } from '@/store/workerStore';
import { ProfileForm } from '../features/ProfileForm';
import type { StorekeeperBindingModel } from '@/types/types';
import { useStorekeepers } from '@/hooks/useStorekeepers';
import { toast } from 'sonner';

export const Profile = (): React.JSX.Element => {
  const { user, updateUser } = useAuthStore();
  const { updateStorekeeper, isUpdateError, updateError } = useStorekeepers();

  React.useEffect(() => {
    if (isUpdateError) {
      toast(updateError?.message);
    }
  }, [isUpdateError, updateError]);

  if (!user) {
    return (
      <main className="container mx-auto py-10">
        Загрузка данных пользователя...
      </main>
    );
  }

  const handleUpdate = (data: Partial<StorekeeperBindingModel>) => {
    console.log(data);
    updateUser(data);
    updateStorekeeper(data);
  };

  return (
    <main className="container mx-auto py-10">
      <h1 className="text-2xl font-bold mb-6">Профиль пользователя</h1>
      <ProfileForm defaultValues={user} onSubmit={handleUpdate} />
    </main>
  );
};
