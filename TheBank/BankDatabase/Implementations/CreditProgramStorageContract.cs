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
            x.CreateMap<CreditProgram, CreditProgramDataModel>()
                .ForMember(dest => dest.Currencies, opt => opt.MapFrom(src => src.CurrencyCreditPrograms));
            x.CreateMap<CreditProgramDataModel, CreditProgram>()
                .ForMember(dest => dest.CurrencyCreditPrograms, opt => opt.MapFrom(src => src.Currencies));
            x.CreateMap<CreditProgramCurrency, CreditProgramCurrencyDataModel>()
                .ForMember(dest => dest.CreditProgramId, opt => opt.MapFrom(src => src.CreditProgramId))
                .ForMember(dest => dest.CurrencyId, opt => opt.MapFrom(src => src.CurrencyId));
            x.CreateMap<CreditProgramCurrencyDataModel, CreditProgramCurrency>()
                .ForMember(dest => dest.CreditProgramId, opt => opt.MapFrom(src => src.CreditProgramId))
                .ForMember(dest => dest.CurrencyId, opt => opt.MapFrom(src => src.CurrencyId))
                .ForMember(dest => dest.CreditProgram, opt => opt.Ignore())
                .ForMember(dest => dest.Currency, opt => opt.Ignore());
        });
        _mapper = new Mapper(config);
    }

    public List<CreditProgramDataModel> GetList(string? storekeeperId = null, string? periodId = null)
    {
        try
        {
            var query = _dbContext.CreditPrograms
                .Include(x => x.CurrencyCreditPrograms)
                .AsQueryable();
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

    public async Task<List<CreditProgramDataModel>> GetListAsync(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        try
        {
            var query = _dbContext.CreditPrograms.AsQueryable();
            //query = query.Where(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            var creditPrograms = await query.ToListAsync(ct);
            return creditPrograms.Select(x => _mapper.Map<CreditProgramDataModel>(x)).ToList();
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
