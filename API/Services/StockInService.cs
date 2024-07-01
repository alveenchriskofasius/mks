using API.Context.SP;
using API.Repository.Interfaces;
using API.Services.Interfaces;
using MitraKaryaSystem.Models;

namespace API.Services
{
    public class StockInService : IStockInService
    {
        private readonly IStockInRepository _repository;

        public StockInService(IStockInRepository repository)
        {
            _repository = repository;
        }

        public async Task<object> DeleteById(int id)
        {
            return await _repository.DeleteById(id);
        }

        public async Task<object> DeleteProductById(int id)
        {
            return await _repository.DeleteProductById(id);
        }

        public async Task<StockInModel> FillForm(int id)
        {
            return await _repository.FillForm(id);
        }

        public async Task<StockInDetailModel> FillFormDetail(int id)
        {
            return await _repository.FillFormDetail(id);
        }

        public async Task<List<uspGetDetailListByIdResult>> GetDetailListById(int id)
        {
            return await _repository.GetDetailListById(id);
        }

        public async Task<object> GetTradeList()
        {
            return await _repository.GetTradeList();
        }

        public async Task<object> Save(StockInModel stockIn)
        {
            return await _repository.Save(stockIn);
        }

        public async Task<object> ScanBarcode(string barcode)
        {
            return await _repository.ScanBarcode(barcode);
        }
    }
}
