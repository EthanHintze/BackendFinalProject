using backendfinal.Dtos;

namespace backendfinal.Services;

public interface IShipmentService
{
    Task<ReceiveShipmentResponse> ReceiveShipmentAsync(ReceiveShipmentRequest request, CancellationToken cancellationToken = default);
}
