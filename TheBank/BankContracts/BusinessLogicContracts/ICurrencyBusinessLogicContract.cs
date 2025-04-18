using BankContracts.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.BusinessLogicContracts;

public interface ICurrencyBusinessLogicContract
{
    List<CurrencyDataModel> GetAllCurrencys();

    CurrencyDataModel GetCurrencyByData(string data);

    void InsertCurrency(CurrencyDataModel currencyDataModel);

    void UpdateCurrency(CurrencyDataModel currencyDataModel);
}
