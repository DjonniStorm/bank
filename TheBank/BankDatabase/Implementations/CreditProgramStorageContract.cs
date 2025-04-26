using AutoMapper;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BankDatabase.Implementations;

/// <summary>
/// реализация контракта хранилища для кредитной программы
/// </summary>
internal class CreditProgramStorageContract : ICreditProgramStorageContract
{
    private readonly BankDbContext _dbContext;

    private readonly Mapper _mapper;

    public CreditProgramStorageContract(BankDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(x =>
        {
            x.CreateMap<CreditProgram, CreditProgramDataModel>();
            x.CreateMap<CreditProgramDataModel, CreditProgram>();
        });
        _mapper = new Mapper(config);
    }

    public List<CreditProgramDataModel> GetList(string? storekeeperId = null, string? periodId = null)
    {
        try
        {
            var query = _dbContext.CreditPrograms.AsQueryable();
            if (storekeeperId is not null)
            {
                query = query.Where(x => x.StorekeeperId == storekeeperId);
            }
            if (periodId is not null)
            {
                query = query.Where(x => x.PeriodId == periodId);
            }
            return [.. query.Select(x => _mapper.Map<CreditProgramDataModel>(x))];
        }
        catch (Exception ex) 
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public CreditProgramDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<CreditProgramDataModel>(GetCreditProgramById(id));
        }
        catch (Exception ex) 
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void AddElement(CreditProgramDataModel creditProgramDataModel)
    {
        try
        {
            _dbContext.CreditPrograms.Add(_mapper.Map<CreditProgram>(creditProgramDataModel));
            _dbContext.SaveChanges();
        }
        catch (InvalidOperationException ex) when (ex.TargetSite?.Name == "ThrowIdentityConflict")
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Id {creditProgramDataModel.Id}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_CreditPrograms_Name" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"PhoneNumber {creditProgramDataModel.Name}");
        }
        catch (Exception ex) 
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void UpdElement(CreditProgramDataModel creditProgramDataModel)
    {
        try
        {
            var element = GetCreditProgramById(creditProgramDataModel.Id) ?? throw new ElementNotFoundException($"id: {creditProgramDataModel.Id}");
            _dbContext.CreditPrograms.Update(_mapper.Map(creditProgramDataModel, element));
            _dbContext.SaveChanges();
        }
        catch (ElementNotFoundException)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_CreditPrograms_Name" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"PhoneNumber {creditProgramDataModel.Name}");
        }
        catch (Exception ex) 
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    private CreditProgram? GetCreditProgramById(string id) => _dbContext.CreditPrograms.FirstOrDefault(x => x.Id == id);
}
