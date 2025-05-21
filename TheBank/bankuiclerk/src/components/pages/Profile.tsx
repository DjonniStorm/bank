import React from 'react';
import { useAuthStore } from '@/store/workerStore';
import { ProfileForm } from '../features/ProfileForm';
import type { ClerkBindingModel } from '@/types/types';
import { useClerks } from '@/hooks/useClerks';
import { toast } from 'sonner';

export const Profile = (): React.JSX.Element => {
  const { user, updateUser } = useAuthStore();
  const { updateClerk, isUpdateError, updateError } = useClerks();

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

  const handleUpdate = (data: Partial<ClerkBindingModel>) => {
    console.log(data);
    updateUser(data);
    updateClerk(data);
  };

  return (
    <main className="container mx-auto py-10">
      <h1 className="text-2xl font-bold mb-6">Профиль пользователя</h1>
      <ProfileForm defaultValues={user} onSubmit={handleUpdate} />
    </main>
  );
};
