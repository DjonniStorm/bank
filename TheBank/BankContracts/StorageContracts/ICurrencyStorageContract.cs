using BankContracts.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.StorageContracts;

public interface ICurrencyStorageContract
{
    List<CurrencyDataModel> GetList();

    CurrencyDataModel? GetElementById(string id);

    CurrencyDataModel? GetElementByAbbreviation(string abbreviation);

    void AddElement(CurrencyDataModel currencyDataModel);

    void UpdElement(CurrencyDataModel currencyDataModel);
}
