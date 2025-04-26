using AutoMapper;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BankDatabase.Implementations;

/// <summary>
/// реализация контракта хранилища для валюты
/// </summary>
internal class CurrencyStorageContract : ICurrencyStorageContract
{
    private readonly BankDbContext _dbContext;

    private readonly Mapper _mapper;

    public CurrencyStorageContract(BankDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(x =>
        {
            x.CreateMap<CurrencyDataModel, Currency>();
            x.CreateMap<Currency, CurrencyDataModel>();
        });
        _mapper = new Mapper(config);
    }

    public List<CurrencyDataModel> GetList(string? storekeeperId = null)
    {
        try
        {
            var query = _dbContext.Currencies.AsQueryable();
            if (storekeeperId is not null)
            {
                query = query.Where(x => x.StorekeeperId == storekeeperId);
            }
            return [.. query.Select(x => _mapper.Map<CurrencyDataModel>(x))];
        }
        catch (Exception ex) 
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public CurrencyDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<CurrencyDataModel>(GetCurrencyById(id));
        }
        catch (Exception ex) 
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }
    public CurrencyDataModel? GetElementByAbbreviation(string abbreviation)
    {
        try
        {
            return _mapper.Map<CurrencyDataModel>(_dbContext.Currencies.FirstOrDefault(x => x.Abbreviation == abbreviation));
        }
        catch (Exception ex) 
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void AddElement(CurrencyDataModel currencyDataModel)
    {
        try
        {
            _dbContext.Currencies.Add(_mapper.Map<Currency>(currencyDataModel));
            _dbContext.SaveChanges();
        }
        catch (InvalidOperationException ex) when (ex.TargetSite?.Name == "ThrowIdentityConflict")
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Id {currencyDataModel.Id}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Currencies_Abbreviation" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Abbreviation {currencyDataModel.Abbreviation}");
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void UpdElement(CurrencyDataModel currencyDataModel)
    {
        try
        {
            var element = GetCurrencyById(currencyDataModel.Id) ?? throw new ElementNotFoundException($"id: {currencyDataModel.Id}");
            _dbContext.Currencies.Update(_mapper.Map(currencyDataModel, element));
            _dbContext.SaveChanges();
        }
        catch (ElementNotFoundException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Currencies_Abbreviation" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Abbreviation {currencyDataModel.Name}");
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }
    private Currency? GetCurrencyById(string id) => _dbContext.Currencies.FirstOrDefault(x => x.Id == id);
}
