using BankContracts.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.StorageContracts;

public interface IPeriodStorageContract
{
    List<PeriodDataModel> GetList(DateTime startDate, DateTime endDate);

    PeriodDataModel? GetElementById(string id);

    void AddElement(PeriodDataModel periodDataModel);

    void UpdElement(PeriodDataModel periodDataModel);
}
