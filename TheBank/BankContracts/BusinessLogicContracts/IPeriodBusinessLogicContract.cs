﻿using BankContracts.DataModels;

namespace BankContracts.BusinessLogicContracts;

public interface IPeriodBusinessLogicContract
{
    List<PeriodDataModel> GetAllPeriods();

    PeriodDataModel GetPeriodByData(string data);

    List<PeriodDataModel> GetAllPeriodsByStorekeeper(string storekeeperId);

    List<PeriodDataModel> GetAllPeriodsByStartTime(DateTime fromDate);

    List<PeriodDataModel> GetAllPeriodsByEndTime(DateTime toDate);

    void InsertPeriod(PeriodDataModel periodataModel);

    void UpdatePeriod(PeriodDataModel periodataModel);
}
