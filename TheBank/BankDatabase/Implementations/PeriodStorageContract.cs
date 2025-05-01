using AutoMapper;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Models;

namespace BankDatabase.Implementations;

/// <summary>
/// реализация контракта хранилища для срока
/// </summary>
internal class PeriodStorageContract : IPeriodStorageContract
{
    private readonly BankDbContext _dbContext;
    private readonly Mapper _mapper;

    public PeriodStorageContract(BankDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg => 
        {
            cfg.CreateMap<Period, PeriodDataModel>()
                .ConstructUsing(src => new PeriodDataModel(src.Id, src.StartTime, src.EndTime, src.StorekeeperId));
            cfg.CreateMap<PeriodDataModel, Period>();
        });
        _mapper = new Mapper(config);
    }

    public List<PeriodDataModel> GetList(DateTime? startDate = null, DateTime? endDate = null, string? storekeeperId = null)
    {
        try
        {
            var query = _dbContext.Periods.AsQueryable();
            if (startDate is not null)
            {
                query = query.Where(x => x.StartTime <= startDate);
            }
            if (endDate is not null)
            {
                query = query.Where(x => x.EndTime <= endDate);
            }
            if (storekeeperId is not null)
            {
                query = query.Where(x => x.StorekeeperId == storekeeperId);
            }
            var test0 = query.FirstOrDefault();
            var test = _mapper.Map<PeriodDataModel>(test0);
            return [..query.Select(x => _mapper.Map<PeriodDataModel>(x))];
        }
        catch (Exception ex) 
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public PeriodDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<PeriodDataModel>(GetPeriodById(id));
        }
        catch (Exception ex) 
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void AddElement(PeriodDataModel periodDataModel)
    {
        try
        {
            _dbContext.Periods.Add(_mapper.Map<Period>(periodDataModel));
            _dbContext.SaveChanges();
        }
        catch (InvalidOperationException ex) when (ex.TargetSite?.Name == "ThrowIdentityConflict")
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Id {periodDataModel.Id}");
        }
        catch (Exception ex) 
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void UpdElement(PeriodDataModel periodDataModel)
    {
        try
        {
            var element = GetPeriodById(periodDataModel.Id) ?? throw new ElementNotFoundException(periodDataModel.Id);
            _dbContext.Periods.Update(_mapper.Map(periodDataModel, element));
            _dbContext.SaveChanges();
        } catch (ElementNotFoundException)
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

    private Period? GetPeriodById(string id) => _dbContext.Periods.FirstOrDefault(x => x.Id == id);
}
