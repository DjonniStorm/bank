using AutoMapper;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BankDatabase.Implementations;

internal class DepositStorageContract : IDepositStorageContract
{
    private readonly BankDbContext _dbContext;
    private readonly Mapper _mapper;

    public DepositStorageContract(BankDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Clerk, ClerkDataModel>();
            cfg.CreateMap<Deposit, DepositDataModel>();
            cfg.CreateMap<DepositDataModel, Deposit>();
            cfg.CreateMap<Replenishment, ReplenishmentDataModel>();
        });
        _mapper = new Mapper(config);
    }
    public List<DepositDataModel> GetList(string? clerkId = null)
    {
        try
        {
            var query = _dbContext.Deposits.Include(x => x.Clerk).AsQueryable();
            if (clerkId is not null)
            {
                query = query.Where(x => x.ClerkId == clerkId);
            }
            return [.. query.Select(x => _mapper.Map<DepositDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public DepositDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<DepositDataModel>(_dbContext.Deposits.FirstOrDefault(x => x.Id == id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public DepositDataModel? GetElementByInterestRate(float interestRate)
    {
        try
        {
            return _mapper.Map<DepositDataModel>(_dbContext.Deposits.Include(x => x.Clerk).FirstOrDefault(x => x.InterestRate == interestRate));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void AddElement(DepositDataModel depositDataModel)
    {
        try
        {
            _dbContext.Deposits.Add(_mapper.Map<Deposit>(depositDataModel));
            _dbContext.SaveChanges();
        }
        catch (InvalidOperationException ex) when (ex.TargetSite?.Name == "ThrowIdentityConflict")
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Id {depositDataModel.Id }");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Deposits_InterestRate" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"InterestRate {depositDataModel.InterestRate}");
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void UpdElement(DepositDataModel depositDataModel)
    {
        try
        {
            var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var element = GetDepositById(depositDataModel.Id) ?? throw new ElementNotFoundException(depositDataModel.Id);

                if (depositDataModel.Currencies != null)
                {
                    if (element.DepositCurrencies != null || element.DepositCurrencies?.Count >= 0)
                    {
                        _dbContext.DepositCurrencies.RemoveRange(element.DepositCurrencies);
                    }

                    element.DepositCurrencies = _mapper.Map<List<DepositCurrency>>(depositDataModel.Currencies);
                }
                _mapper.Map(depositDataModel, element);
                _dbContext.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (ElementNotFoundException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Deposits_InterestRate" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"InterestRate {depositDataModel.InterestRate}");
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    private Deposit? GetDepositById(string id) => _dbContext.Deposits.FirstOrDefault(x => x.Id == id);
}
