using AutoMapper;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BankDatabase.Implementations;

internal class ReplenishmentStorageContract : IReplenishmentStorageContract
{
    private readonly BankDbContext _dbContext;
    private readonly Mapper _mapper;

    public ReplenishmentStorageContract(BankDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Replenishment, ReplenishmentDataModel>();
            cfg.CreateMap<ReplenishmentDataModel, Replenishment>();
        });
        _mapper = new Mapper(config);
    }
    
    public List<ReplenishmentDataModel> GetList(DateTime? fromDate = null, DateTime? toDate = null, string? clerkId = null, string? depositId = null)
    {
        try
        {
            var query = _dbContext.Replenishments.Where(x => x.Date >= fromDate && x.Date <= toDate);
            if (clerkId is not null)
            {
                query = query.Where(x => x.ClerkId == clerkId);
            }
            if (depositId is not null)
            {
                query = query.Where(x => x.DepositId == depositId);
            }
            return [.. query.Select(x => _mapper.Map<ReplenishmentDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public ReplenishmentDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<ReplenishmentDataModel>(_dbContext.Replenishments.FirstOrDefault(x => x.Id == id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    
    public void AddElement(ReplenishmentDataModel replenishmentDataModel)
    {
        try
        {
            _dbContext.Replenishments.Add(_mapper.Map<Replenishment>(replenishmentDataModel));
            _dbContext.SaveChanges();
        }
        catch (InvalidOperationException ex) when (ex.TargetSite?.Name == "ThrowIdentityConflict")
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Id {replenishmentDataModel.Id}");
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void UpdElement(ReplenishmentDataModel replenishmentDataModel)
    {
        try
        {
            var element = GetReplenishmentById(replenishmentDataModel.Id) ?? throw new ElementNotFoundException(replenishmentDataModel.Id);
            _dbContext.Replenishments.Update(_mapper.Map(replenishmentDataModel, element));
            _dbContext.SaveChanges();
        }
        catch (ElementNotFoundException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }
    private Replenishment? GetReplenishmentById(string id) => _dbContext.Replenishments.FirstOrDefault(x => x.Id == id);
}
