export interface ClientBindingModel {
  id?: string;
  name?: string;
  surname?: string;
  balance: number;
  clerkId?: string;
  depositClients?: DepositClientBindingModel[];
  creditProgramClients?: ClientCreditProgramBindingModel[];
}

export interface DepositClientBindingModel {
  id?: string;
  clientId?: string;
  depositId?: string;
}

export interface ClientCreditProgramBindingModel {
  id?: string;
  clientId?: string;
  creditProgramId?: string;
}

export interface DepositBindingModel {
  id?: string;
  interestRate: number;
  cost: number;
  period: number;
  clerkId?: string;
  depositCurrencies?: DepositCurrencyBindingModel[];
}

export interface DepositCurrencyBindingModel {
  id?: string;
  depositId?: string;
  currencyId?: string;
}

export interface CurrencyBindingModel {
  id?: string;
  name?: string;
  abbreviation?: string;
  cost: number;
  storekeeperId?: string;
}

export interface CreditProgramBindingModel {
  id: string;
  name: string;
  cost: number;
  maxCost: number;
  storekeeperId: string;
  periodId: string;
  currencyCreditPrograms?: CreditProgramCurrencyBindingModel[];
}

export interface CreditProgramCurrencyBindingModel {
  id?: string;
  creditProgramId?: string;
  currencyId?: string;
}

export interface ClerkBindingModel {
  id?: string;
  name?: string;
  surname?: string;
  middleName?: string;
  login?: string;
  password?: string;
  email?: string;
  phoneNumber?: string;
}

export interface PeriodBindingModel {
  id?: string;
  startTime: Date;
  endTime: Date;
  storekeeperId?: string;
}

export interface ReplenishmentBindingModel {
  id?: string;
  amount: number;
  date: Date;
  depositId?: string;
  clerkId?: string;
}

export interface StorekeeperBindingModel {
  id?: string;
  name?: string;
  surname?: string;
  middleName?: string;
  login?: string;
  password?: string;
  email?: string;
  phoneNumber?: string;
}

export interface LoginBindingModel {
  login: string;
  password: string;
}

export interface MailSendInfoBindingModel {
  email: string;
  subject?: string;
  body?: string;
}
