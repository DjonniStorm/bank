using BankContracts.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.BusinessLogicContracts;

public interface IStorekeeperBusinessLogicContract
{
    List<StorekeeperDataModel> GetAllStorekeepers();

    StorekeeperDataModel GetStorekeeperByData(string data);

    void InsertStorekeeper(StorekeeperDataModel storekeeperDataModel);

    void UpdateStorekeeper(StorekeeperDataModel storekeeperDataModel);
}
