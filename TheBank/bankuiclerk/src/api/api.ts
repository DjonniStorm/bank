import {
  getData,
  getSingleData,
  postData,
  postLoginData,
  putData,
  getFileData,
  postEmailData,
} from './client';
import type {
  ClientBindingModel,
  ClerkBindingModel,
  CreditProgramBindingModel,
  CurrencyBindingModel,
  DepositBindingModel,
  PeriodBindingModel,
  ReplenishmentBindingModel,
  StorekeeperBindingModel,
  LoginBindingModel,
} from '../types/types';

// Clients API
export const clientsApi = {
  getAll: () => getData<ClientBindingModel>('api/Clients/GetAllRecords'),
  getById: (id: string) =>
    getData<ClientBindingModel>(`api/Clients/GetRecord/${id}`),
  getByClerk: (clerkId: string) =>
    getData<ClientBindingModel>(`api/Clients/GetRecordByClerk/${clerkId}`),
  create: (data: ClientBindingModel) => postData('api/Clients/Register', data),
  update: (data: ClientBindingModel) => putData('api/Clients/ChangeInfo', data),
};

// Clerks API
export const clerksApi = {
  getAll: () => getData<ClerkBindingModel>('api/clerks'),
  getById: (id: string) =>
    getData<ClerkBindingModel>(`api/Clerks/GetRecord/${id}`),
  create: (data: ClerkBindingModel) => postData('api/Clerks/Register', data),
  update: (data: ClerkBindingModel) => putData('api/Clerks', data),
  // auth
  login: (data: LoginBindingModel) => postLoginData('api/Clerks/login', data),
  logout: () => postData('api/clerks/logout', {}),
  getCurrentUser: () => getSingleData<ClerkBindingModel>('api/clerks/me'),
};

// Credit Programs API
export const creditProgramsApi = {
  getAll: () =>
    getData<CreditProgramBindingModel>('api/CreditPrograms/GetAllRecords'),
  getById: (id: string) =>
    getData<CreditProgramBindingModel>(`api/CreditPrograms/GetRecord/${id}`),
  getByStorekeeper: (storekeeperId: string) =>
    getData<CreditProgramBindingModel>(
      `api/CreditPrograms/GetRecordByStorekeeper/${storekeeperId}`,
    ),
  create: (data: CreditProgramBindingModel) =>
    postData('api/CreditPrograms/Register', data),
  update: (data: CreditProgramBindingModel) =>
    putData('api/CreditPrograms/ChangeInfo', data),
};

// Currencies API
export const currenciesApi = {
  getAll: () => getData<CurrencyBindingModel>('api/Currencies/GetAllRecords'),
  getById: (id: string) =>
    getData<CurrencyBindingModel>(`api/Currencies/GetRecord/${id}`),
  getByStorekeeper: (storekeeperId: string) =>
    getData<CurrencyBindingModel>(
      `api/Currencies/GetRecordByStorekeeper/${storekeeperId}`,
    ),
  create: (data: CurrencyBindingModel) =>
    postData('api/Currencies/Register', data),
  update: (data: CurrencyBindingModel) =>
    putData('api/Currencies/ChangeInfo', data),
};

// Deposits API
export const depositsApi = {
  getAll: () => getData<DepositBindingModel>('api/Deposits/GetAllRecords'),
  getById: (id: string) =>
    getData<DepositBindingModel>(`api/Deposits/GetRecord/${id}`),
  getByClerk: (clerkId: string) =>
    getData<DepositBindingModel>(`api/Deposits/GetRecordByClerk/${clerkId}`),
  create: (data: DepositBindingModel) =>
    postData('api/Deposits/Register', data),
  update: (data: DepositBindingModel) =>
    putData('api/Deposits/ChangeInfo', data),
};

// Periods API
export const periodsApi = {
  getAll: () => getData<PeriodBindingModel>('api/Periods/GetAllRecords'),
  getById: (id: string) =>
    getData<PeriodBindingModel>(`api/Periods/GetRecord/${id}`),
  getByStorekeeper: (storekeeperId: string) =>
    getData<PeriodBindingModel>(
      `api/Period/GetRecordByStorekeeper/${storekeeperId}`,
    ),
  create: (data: PeriodBindingModel) => postData('api/Periods/Register', data),
  update: (data: PeriodBindingModel) => putData('api/Periods/ChangeInfo', data),
};

// Replenishments API
export const replenishmentsApi = {
  getAll: () =>
    getData<ReplenishmentBindingModel>('api/Replenishments/GetAllRecords'),
  getById: (id: string) =>
    getData<ReplenishmentBindingModel>(`api/Replenishments/GetRecord/${id}`),
  getByDeposit: (depositId: string) =>
    getData<ReplenishmentBindingModel>(
      `api/Replenishments/GetRecordByDeposit/${depositId}`,
    ),
  getByClerk: (clerkId: string) =>
    getData<ReplenishmentBindingModel>(
      `api/Replenishments/GetRecordByClerk/${clerkId}`,
    ),
  create: (data: ReplenishmentBindingModel) =>
    postData('api/Replenishments/Register', data),
  update: (data: ReplenishmentBindingModel) =>
    putData('api/Replenishments/ChangeInfo', data),
};

// Storekeepers API
export const storekeepersApi = {
  getAll: () => getData<StorekeeperBindingModel>('api/storekeepers'),
  getById: (id: string) =>
    getData<StorekeeperBindingModel>(`api/Storekeepers/GetRecord/${id}`),
  create: (data: StorekeeperBindingModel) =>
    postData('api/Storekeepers/Register', data),
  update: (data: StorekeeperBindingModel) => putData('api/Storekeepers', data),
  // auth
  login: (data: LoginBindingModel) =>
    postLoginData('api/Storekeepers/login', data),
  logout: () => postData('api/storekeepers/logout', {}),
  getCurrentUser: () =>
    getSingleData<StorekeeperBindingModel>('api/storekeepers/me'),
};

// Reports API
export const reportsApi = {
  // PDF отчеты по депозитам
  getDepositsPdfReport: (fromDate: string, toDate: string) =>
    getFileData(
      `api/Report/LoadClientsByDeposit?fromDate=${fromDate}&toDate=${toDate}`,
    ),

  getDepositsDataReport: (fromDate: string, toDate: string) =>
    getData(
      `api/Report/GetClientByDeposit?fromDate=${fromDate}&toDate=${toDate}`,
    ),

  sendDepositsPdfReport: (
    fromDate: string,
    toDate: string,
    email: string,
    subject: string,
    body: string,
  ) =>
    postEmailData(
      `api/Report/SendReportByDeposit?fromDate=${fromDate}&toDate=${toDate}`,
      { email, subject, body },
    ),

  // Word отчеты по кредитным программам
  getCreditProgramsWordReport: (creditProgramIds: string) =>
    getFileData(`api/Report/LoadClientsByCreditProgram?${creditProgramIds}`),

  getCreditProgramsDataReport: (creditProgramIds: string[]) =>
    getData(
      `api/Report/GetClientByCreditProgram?creditProgramIds=${creditProgramIds.join(
        ',',
      )}`,
    ),

  sendCreditProgramsWordReport: (
    creditProgramIds: string[],
    email: string,
    subject: string,
    body: string,
  ) =>
    postEmailData('api/Report/SendReportByCreditProgram', {
      email,
      subject,
      body,
      creditProgramIds,
    }),

  // Excel отчеты по кредитным программам
  getCreditProgramsExcelReport: (creditProgramIds: string) =>
    getFileData(
      `api/Report/LoadExcelClientByCreditProgram?${creditProgramIds}`,
    ),

  sendCreditProgramsExcelReport: (
    creditProgramIds: string[],
    email: string,
    subject: string,
    body: string,
  ) =>
    postEmailData('api/Report/SendExcelReportByCreditProgram', {
      email,
      subject,
      body,
      creditProgramIds,
    }),
};
