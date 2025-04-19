using BankContracts.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.StorageContracts;

public interface IStorekeeperStorageContract
{
    List<StorekeeperDataModel> GetList();

    StorekeeperDataModel? GetElementById(string id);

    StorekeeperDataModel? GetElementByPhoneNumber(string phoneNumber);

    StorekeeperDataModel? GetElementByName(string name);

    StorekeeperDataModel? GetElementBySurname(string surname);

    void AddElement(StorekeeperDataModel storekeeperDataModel);

    void UpdElement(StorekeeperDataModel storekeeperDataModel);
}
