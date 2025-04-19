using BankContracts.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.StorageContracts;

public interface ICreditProgramStorageContract
{
    List<CreditProgramDataModel> GetList();

    CreditProgramDataModel? GetElementById(string id);

    void AddElement(CreditProgramDataModel creditProgramDataModel);
}
