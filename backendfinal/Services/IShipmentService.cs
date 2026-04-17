using backendfinal.Dtos;

namespace backendfinal.Services;

public interface IShipmentService
{
    Task<ShipmentResponse> CreateShipmentAsync(CreateShipmentRequest request);
    Task<ReceiveShipmentResponse> ReceiveShipmentAsync(ReceiveShipmentRequest request, CancellationToken cancellationToken = default);
}
