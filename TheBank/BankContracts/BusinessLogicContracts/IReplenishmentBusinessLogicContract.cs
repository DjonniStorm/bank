using BankContracts.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.BusinessLogicContracts;

public interface IReplenishmentBusinessLogicContract
{
    List<ReplenishmentDataModel> GetAllReplenishments();

    ReplenishmentDataModel GetReplenishmentByData(string data);

    List<ReplenishmentDataModel> GetAllReplenishmentsByDate(DateTime fromDate, DateTime toDate);

    void InsertReplenishment(ReplenishmentDataModel replenishmentataModel);

    void UpdateReplenishment(ReplenishmentDataModel replenishmentataModel);
}
