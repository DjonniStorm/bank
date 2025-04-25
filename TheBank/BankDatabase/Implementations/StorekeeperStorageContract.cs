using AutoMapper;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BankDatabase.Implementations;

/// <summary>
/// реализация контракта для кладовщика
/// </summary>
internal class StorekeeperStorageContract : IStorekeeperStorageContract
{
    private readonly BankDbContext _dbContext;
    private readonly Mapper _mapper;

    public StorekeeperStorageContract(BankDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Storekeeper, StorekeeperDataModel>();
            cfg.CreateMap<StorekeeperDataModel, Storekeeper>();
        });
        _mapper = new Mapper(config);
    }
    public List<StorekeeperDataModel> GetList()
    {
        try
        {
            return [.. _dbContext.Storekeepers.Select(x => _mapper.Map<StorekeeperDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public StorekeeperDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<StorekeeperDataModel>(_dbContext.Storekeepers.FirstOrDefault(x => x.Id == id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public StorekeeperDataModel? GetElementByLogin(string login)
    {
        try
        {
            return _mapper.Map<StorekeeperDataModel>(_dbContext.Storekeepers.FirstOrDefault(x => x.Login == login));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public StorekeeperDataModel? GetElementByPhoneNumber(string phoneNumber)
    {
        try
        {
            return _mapper.Map<StorekeeperDataModel>(_dbContext.Storekeepers.FirstOrDefault(x => x.PhoneNumber == phoneNumber));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void AddElement(StorekeeperDataModel storekeeperDataModel)
    {
        try
        {
            _dbContext.Storekeepers.Add(_mapper.Map<Storekeeper>(storekeeperDataModel));
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Storekeepers_Email" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Email {storekeeperDataModel.Email}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Storekeepers_PhoneNumber" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"PhoneNumber {storekeeperDataModel.PhoneNumber}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Storekeepers_Login" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Login {storekeeperDataModel.Login}");
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void UpdElement(StorekeeperDataModel storekeeperDataModel)
    {
        try
        {
            var element = GetStorekeeperById(storekeeperDataModel.Id) ?? throw new ElementNotFoundException(storekeeperDataModel.Id);
            _dbContext.Storekeepers.Update(_mapper.Map(storekeeperDataModel, element));
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Storekeepers_Email" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Email {storekeeperDataModel.Email}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Storekeepers_PhoneNumber" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"PhoneNumber {storekeeperDataModel.PhoneNumber}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Storekeepers_Login" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Login {storekeeperDataModel.Login}");
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }
    private Storekeeper? GetStorekeeperById(string id) => _dbContext.Storekeepers.Where(x => x.Id == id).FirstOrDefault();
}
