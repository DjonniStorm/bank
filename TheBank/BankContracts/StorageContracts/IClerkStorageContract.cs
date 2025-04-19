using BankContracts.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.StorageContracts;

public interface IClerkStorageContract
{
    List<ClerkDataModel> GetList();

    ClerkDataModel? GetElementById(string id);

    ClerkDataModel? GetElementByPhoneNumber(string phoneNumber);

    ClerkDataModel? GetElementByName(string name);

    ClerkDataModel? GetElementBySurname(string surname);

    void AddElement(ClerkDataModel clerkDataModel);

    void UpdElement(ClerkDataModel clerkDataModel);
}
