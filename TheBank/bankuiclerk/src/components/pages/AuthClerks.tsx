import { useClerks } from '@/hooks/useClerks';
import React from 'react';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '../ui/tabs';
import { RegisterForm } from '../features/RegisterForm';
import { LoginForm } from '../features/LoginForm';
import { toast } from 'sonner';
import type { LoginBindingModel, ClerkBindingModel } from '@/types/types';

type Forms = 'login' | 'register';

export const AuthClerks = (): React.JSX.Element => {
  const { createClerk, loginClerk, isLoginError, loginError, isCreateError } =
    useClerks();

  const [currentForm, setCurrentForm] = React.useState<Forms>('login');

  const handleRegister = (data: ClerkBindingModel) => {
    createClerk(data, {
      onSuccess: () => {
        toast('Регистрация успешна! Войдите в систему.');
      },
      onError: (error) => {
        toast(`Ошибка регистрации: ${error.message}`);
      },
    });
  };

  const handleLogin = (data: LoginBindingModel) => {
    loginClerk(data);
  };

  React.useEffect(() => {
    if (isLoginError) {
      toast(`Ошибка входа: ${loginError?.message}`);
    }
    if (isCreateError) {
      toast('Ошибка при регистрации');
    }
  }, [isLoginError, loginError, isCreateError]);

  return (
    <>
      <main className="flex flex-col justify-center items-center">
        <div>
          <Tabs defaultValue="login" className="w-[400px]">
            <TabsList>
              <TabsTrigger
                onClick={() => setCurrentForm('login')}
                value="login"
              >
                Вход
              </TabsTrigger>
              <TabsTrigger
                onClick={() => setCurrentForm('register')}
                value="register"
              >
                Регистрация
              </TabsTrigger>
            </TabsList>
            <TabsContent value={currentForm}>
              <LoginForm onSubmit={handleLogin} />
            </TabsContent>
            <TabsContent value="register">
              <RegisterForm onSubmit={handleRegister} />
            </TabsContent>
          </Tabs>
        </div>
      </main>
    </>
  );
};
