using AutoMapper;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BankDatabase.Implementations;

/// <summary>
/// реализация контракта для клерка
/// </summary>
internal class ClerkStorageContract : IClerkStorageContract
{
    private readonly BankDbContext _dbContext;
    private readonly Mapper _mapper;

    public ClerkStorageContract(BankDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Clerk, ClerkDataModel>();
            cfg.CreateMap<ClerkDataModel, Clerk>();
        });
        _mapper = new Mapper(config);
    }

    public List<ClerkDataModel> GetList()
    {
        try
        {
            return [.. _dbContext.Clerks.Select(x => _mapper.Map<ClerkDataModel>(x))];
        }
        catch (Exception ex) 
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }
    public ClerkDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<ClerkDataModel>(_dbContext.Clerks.FirstOrDefault(x => x.Id == id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public ClerkDataModel? GetElementByLogin(string login)
    {
        try
        {
            return _mapper.Map<ClerkDataModel>(_dbContext.Clerks.FirstOrDefault(x => x.Login == login));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public ClerkDataModel? GetElementByPhoneNumber(string phoneNumber)
    {
        try
        {
            return _mapper.Map<ClerkDataModel>(_dbContext.Clerks.FirstOrDefault(x => x.PhoneNumber == phoneNumber));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void AddElement(ClerkDataModel clerkDataModel)
    {
        try
        {
            _dbContext.Clerks.Add(_mapper.Map<Clerk>(clerkDataModel));
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Clerks_Email" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Email {clerkDataModel.Email}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Clerks_PhoneNumber" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"PhoneNumber {clerkDataModel.PhoneNumber}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Clerks_Login" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Login {clerkDataModel.Login}");
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void UpdElement(ClerkDataModel clerkDataModel)
    {
        try
        {
            var element = GetClerkById(clerkDataModel.Id) ?? throw new ElementNotFoundException(clerkDataModel.Id);
            _dbContext.Clerks.Update(_mapper.Map(clerkDataModel, element));
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Clerks_Email" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Email {clerkDataModel.Email}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Clerks_PhoneNumber" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"PhoneNumber {clerkDataModel.PhoneNumber}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Clerks_Login" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Login {clerkDataModel.Login}");
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    private Clerk? GetClerkById(string id) => _dbContext.Clerks.Where(x => x.Id == id).FirstOrDefault();
}
