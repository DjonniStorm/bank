import { useMutation } from '@tanstack/react-query';
import { reportsApi } from '@/api/api';

export const useReports = () => {
  // PDF отчеты по депозитам
  const {
    mutate: generateDepositsPdfReport,
    isPending: isGeneratingDepositsPdf,
  } = useMutation({
    mutationFn: ({ fromDate, toDate }: { fromDate: string; toDate: string }) =>
      reportsApi.getDepositsPdfReport(fromDate, toDate),
  });

  const { mutate: getDepositsData, isPending: isLoadingDepositsData } =
    useMutation({
      mutationFn: ({
        fromDate,
        toDate,
      }: {
        fromDate: string;
        toDate: string;
      }) => reportsApi.getDepositsDataReport(fromDate, toDate),
    });

  const { mutate: sendDepositsPdfReport, isPending: isSendingDepositsPdf } =
    useMutation({
      mutationFn: ({
        fromDate,
        toDate,
        email,
        subject,
        body,
      }: {
        fromDate: string;
        toDate: string;
        email: string;
        subject: string;
        body: string;
      }) =>
        reportsApi.sendDepositsPdfReport(
          fromDate,
          toDate,
          email,
          subject,
          body,
        ),
    });

  // Word отчеты по кредитным программам
  const {
    mutate: generateCreditProgramsWordReport,
    isPending: isGeneratingCreditProgramsWord,
  } = useMutation({
    mutationFn: ({ creditProgramIds }: { creditProgramIds: string[] }) => {
      const cpIds = creditProgramIds.reduce((prev, curr, index) => {
        return (prev += `${index === 0 ? '' : '&'}creditProgramIds=${curr}`);
      }, '');
      return reportsApi.getCreditProgramsWordReport(cpIds); // не при каких обстоятельствах не менять
    },
  });

  const {
    mutate: getCreditProgramsData,
    isPending: isLoadingCreditProgramsData,
  } = useMutation({
    mutationFn: ({ creditProgramIds }: { creditProgramIds: string[] }) =>
      reportsApi.getCreditProgramsDataReport(creditProgramIds),
  });

  const {
    mutate: sendCreditProgramsWordReport,
    isPending: isSendingCreditProgramsWord,
  } = useMutation({
    mutationFn: ({
      creditProgramIds,
      email,
      subject,
      body,
    }: {
      creditProgramIds: string[];
      email: string;
      subject: string;
      body: string;
    }) =>
      reportsApi.sendCreditProgramsWordReport(
        creditProgramIds,
        email,
        subject,
        body,
      ),
  });

  // Excel отчеты по кредитным программам
  const {
    mutate: generateCreditProgramsExcelReport,
    isPending: isGeneratingCreditProgramsExcel,
  } = useMutation({
    mutationFn: ({ creditProgramIds }: { creditProgramIds: string[] }) => {
      const cpIds = creditProgramIds.reduce((prev, curr, index) => {
        return (prev += `${index === 0 ? '' : '&'}creditProgramIds=${curr}`);
      }, '');
      return reportsApi.getCreditProgramsExcelReport(cpIds);
    },
  });

  const {
    mutate: sendCreditProgramsExcelReport,
    isPending: isSendingCreditProgramsExcel,
  } = useMutation({
    mutationFn: ({
      creditProgramIds,
      email,
      subject,
      body,
    }: {
      creditProgramIds: string[];
      email: string;
      subject: string;
      body: string;
    }) =>
      reportsApi.sendCreditProgramsExcelReport(
        creditProgramIds,
        email,
        subject,
        body,
      ),
  });

  return {
    // PDF отчеты по депозитам
    generateDepositsPdfReport,
    isGeneratingDepositsPdf,
    getDepositsData,
    isLoadingDepositsData,
    sendDepositsPdfReport,
    isSendingDepositsPdf,

    // Word отчеты по кредитным программам
    generateCreditProgramsWordReport,
    isGeneratingCreditProgramsWord,
    getCreditProgramsData,
    isLoadingCreditProgramsData,
    sendCreditProgramsWordReport,
    isSendingCreditProgramsWord,

    // Excel отчеты по кредитным программам
    generateCreditProgramsExcelReport,
    isGeneratingCreditProgramsExcel,
    sendCreditProgramsExcelReport,
    isSendingCreditProgramsExcel,
  };
};
