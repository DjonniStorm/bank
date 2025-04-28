using AutoMapper;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BankDatabase.Implementations;

internal class ClientStorageContract : IClientStorageContract
{

    private readonly BankDbContext _dbContext;
    private readonly Mapper _mapper;

    public ClientStorageContract(BankDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Clerk, ClerkDataModel>();
            cfg.CreateMap<Client, ClientDataModel>();
            cfg.CreateMap<ClientDataModel, Client>();
            cfg.CreateMap<Replenishment, ReplenishmentDataModel>();
        });
        _mapper = new Mapper(config);
    }

    public List<ClientDataModel> GetList(string? clerkId = null)
    {
        try
        {
            var query = _dbContext.Clients.Include(x => x.Clerk).AsQueryable();
            if (clerkId is not null)
            {
                query = query.Where(x => x.ClerkId == clerkId);
            }
            return [.. query.Select(x => _mapper.Map<ClientDataModel>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public ClientDataModel? GetElementById(string id)
    {
        try
        {
            return _mapper.Map<ClientDataModel>(_dbContext.Clients.FirstOrDefault(x => x.Id == id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public ClientDataModel? GetElementByName(string name)
    {
        try
        {
            return _mapper.Map<ClientDataModel>(_dbContext.Clients.Include(x => x.Clerk).FirstOrDefault(x => x.Name == name));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public ClientDataModel? GetElementBySurname(string surname)
    {
        try
        {
           return _mapper.Map<ClientDataModel>(_dbContext.Clients.Include(x => x.Clerk).FirstOrDefault(x => x.Surname == surname));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void AddElement(ClientDataModel clientDataModel)
    {
        try
        {
            _dbContext.Clients.Add(_mapper.Map<Client>(clientDataModel));
            _dbContext.SaveChanges();
        }
        catch (InvalidOperationException ex) when (ex.TargetSite?.Name == "ThrowIdentityConflict")
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Id {clientDataModel.Id}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Clients_Name" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Name {clientDataModel.Name}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Clients_Surname" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Surname {clientDataModel.Surname}");
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }

    public void UpdElement(ClientDataModel clientDataModel)
    {
        try
        {
            var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var element = GetClientById(clientDataModel.Id) ?? throw new ElementNotFoundException(clientDataModel.Id);
                //проверь пожалуйста(не уверен)
                if (clientDataModel.Deposits != null && clientDataModel.CreditPrograms != null)
                {
                    if (element.DepositClients != null || element.DepositClients?.Count >= 0)
                    {
                        _dbContext.DepositClients.RemoveRange(element.DepositClients);
                    }

                    if (element.CreditProgramClients != null || element.CreditProgramClients?.Count >= 0)
                    {
                        _dbContext.DepositClients.RemoveRange(element.DepositClients);
                    }

                    element.DepositClients = _mapper.Map<List<DepositClient>>(clientDataModel.Deposits);
                    element.DepositClients = _mapper.Map<List<DepositClient>>(clientDataModel.CreditPrograms);
                }
                _mapper.Map(element, clientDataModel);
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
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Clients_Name" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Name {clientDataModel.Name}");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { ConstraintName: "IX_Clients_Surname" })
        {
            _dbContext.ChangeTracker.Clear();
            throw new ElementExistsException($"Surname {clientDataModel.Surname}");
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex.Message);
        }
    }
    private Client? GetClientById(string id) => _dbContext.Clients.FirstOrDefault(x => x.Id == id);
}
