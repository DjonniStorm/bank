using BankContracts.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.BusinessLogicContracts;

public interface ICreditProgramBusinessLogicContract
{
    List<CreditProgramDataModel> GetAllCreditPrograms();

    List<CreditProgramDataModel> GetCreditProgramByStorekeeper(string storekeeperId);

    List<CreditProgramDataModel> GetCreditProgramByPeriod(string periodId);

    CreditProgramDataModel GetCreditProgramByData(string data);  

    void InsertCreditProgram(CreditProgramDataModel creditProgramDataModel);

    void UpdateCreditProgram(CreditProgramDataModel creditProgramDataModel);
}
