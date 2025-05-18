import { useStorekeepers } from '@/hooks/useStorekeepers';
import React from 'react';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '../ui/tabs';
import { RegisterForm } from '../features/RegisterForm';
import type { LoginBindingModel, StorekeeperBindingModel } from '@/types/types';
import { LoginForm } from '../features/LoginForm';
import { Toaster } from '../ui/sonner';
import { toast } from 'sonner';

export const AuthStorekeeper = (): React.JSX.Element => {
  const { createStorekeeper, loginStorekeeper, isLoginError, loginError } =
    useStorekeepers();

  const handleRegister = (data: StorekeeperBindingModel) => {
    console.log(data);
    createStorekeeper(data);
  };
  const handleLogin = (data: LoginBindingModel) => {
    console.log(data);
    loginStorekeeper(data);
  };

  React.useEffect(() => {
    if (isLoginError) {
      toast(`Ошибка ${loginError?.message}`);
    }
  }, [isLoginError, loginError]);

  return (
    <>
      <main className="flex flex-col justify-center items-center">
        <div>
          <Tabs defaultValue="login" className="w-[400px]">
            <TabsList>
              <TabsTrigger value="login">Вход</TabsTrigger>
              <TabsTrigger value="register">Регистрация</TabsTrigger>
            </TabsList>
            <TabsContent value="login">
              <LoginForm onSubmit={handleLogin} />
            </TabsContent>
            <TabsContent value="register">
              <RegisterForm onSubmit={handleRegister} />
            </TabsContent>
          </Tabs>
        </div>
      </main>
      <Toaster />
    </>
  );
};
