using BankContracts.Infrastructure;
using BankContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.AdapterContracts.OperationResponses;

public class ClientOperationResponse : OperationResponse
{
    public static ClientOperationResponse OK(List<ClientViewModel> data) =>
        OK<ClientOperationResponse, List<ClientViewModel>>(data);

    public static ClientOperationResponse OK(ClientViewModel data) =>
        OK<ClientOperationResponse, ClientViewModel>(data);

    public static ClientOperationResponse NoContent() => NoContent<ClientOperationResponse>();

    public static ClientOperationResponse NotFound(string message) =>
        NotFound<ClientOperationResponse>(message);

    public static ClientOperationResponse BadRequest(string message) =>
        BadRequest<ClientOperationResponse>(message);

    public static ClientOperationResponse InternalServerError(string message) =>
        InternalServerError<ClientOperationResponse>(message);
}
