using BankContracts.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.StorageContracts;

public interface IDepositStorageContract
{
    List<DepositDataModel> GetList();

    DepositDataModel? GetElementById(string id);

    DepositDataModel? GetElementByName(string name);

    void AddElement(DepositDataModel depositDataModel);
}
