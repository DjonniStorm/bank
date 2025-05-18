import { useStorekeepers } from '@/hooks/useStorekeepers';
import React from 'react';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '../ui/tabs';
import { RegisterForm } from '../features/RegisterForm';
import type { LoginBindingModel, StorekeeperBindingModel } from '@/types/types';
import { LoginForm } from '../features/LoginForm';
import { Toaster } from '../ui/sonner';
import { toast } from 'sonner';
import { useNavigate } from 'react-router-dom';

export const AuthStorekeeper = (): React.JSX.Element => {
  const navigate = useNavigate();
  const {
    createStorekeeper,
    loginStorekeeper,
    isLoginError,
    loginError,
    isLoginSuccess,
  } = useStorekeepers();

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

  React.useEffect(() => {
    if (isLoginSuccess) {
      navigate('/storekeepers');
    }
  }, [isLoginSuccess]);

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
