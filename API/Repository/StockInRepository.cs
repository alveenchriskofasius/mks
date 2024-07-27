using API.Context.SP;
using API.Context.Table;
using API.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using MitraKaryaSystem.Models;

namespace API.Repository
{
    public class StockInRepository : IStockInRepository
    {
        private readonly MKSTableContext _context;
        private readonly MKSSPContextProcedures _procedure;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public StockInRepository(MKSTableContext context, MKSSPContextProcedures procedure, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _procedure = procedure;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<StockInModel> FillForm(int id)
        {
            Trade trade = await _context.Trades.FindAsync(id);
            StockInModel stockIn = null;
            if (trade == null)
            {
                stockIn = new StockInModel();
            }
            else
            {
                stockIn = new StockInModel
                {
                    ID = trade.ID,
                    Date = trade.Date,
                    StatusID = trade.StatusID,
                    No = trade.No
                };
            }
            return stockIn;
        }
        public async Task<object> DeleteProductById(int id)
        {
            try
            {
                var product = await _context.StockInItems.FindAsync(id);
                if (product == null)
                {
                    return new { success = false, result = "Stock In item not found." };
                }
                _context.StockInItems.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await Task.FromResult<object>(new { success = false, result = e.Message });
            }
            return new { success = true };
        }
        public async Task<object> DeleteById(int id)
        {
            try
            {
                var trade = await _context.Trades.FindAsync(id);
                var stockInItems = await _context.StockInItems.Where(x => x.TradeID == id).ToListAsync();
                if (trade == null)
                {
                    return new { success = false, result = "Stock In not found." };
                }
                _context.StockInItems.RemoveRange(stockInItems);
                _context.Trades.Remove(trade);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await Task.FromResult<object>(new { success = false, result = e.Message });
            }
            return new { success = true };
        }
        public async Task<StockInDetailModel> FillFormDetail(int id)
        {
            var product = await _context.Products.FindAsync(id);
            var purchaseOrderDetail = new StockInDetailModel();
            try
            {
                if (product == null)
                {
                    return new StockInDetailModel();
                }
                else
                {
                    purchaseOrderDetail = new StockInDetailModel
                    {
                        ID = product.ID,
                        Quantity = 1
                    };
                };
            }
            catch (Exception e)
            {
                await Task.FromResult<object>(new { success = false, result = e.Message });
            }
            return purchaseOrderDetail;
        }
        public async Task<object> GetStockInList()=> await _procedure.GetStockInListAsync();

        public Task<List<uspGetDetailListByIdResult>> GetDetailListById(int id)=> _procedure.uspGetDetailListByIdAsync(id);
        public async Task<object> Save(StockInModel stockInModel)
        {
            try
            {
                int tradeID = 0;
                if (stockInModel.ID == 0)
                {
                    var newTrade = new Trade
                    {
                        Date = stockInModel.Date,
                        StatusID = 1,
                        No =  _procedure.uspGenerateNoAsync("SI", stockInModel.Date).Result.FirstOrDefault().NewPONumber,
                        CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name,
                        TradeTypeID = 3
                    };
                    _context.Trades.Add(newTrade);
                    await _context.SaveChangesAsync();
                    tradeID = newTrade.ID;
                }
                else
                {
                    var existingTrade = await _context.Trades.FindAsync(stockInModel.ID);
                    if (existingTrade != null)
                    {
                        existingTrade.Date = stockInModel.Date;
                        existingTrade.UpdatedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                        existingTrade.UpdatedAt = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return new { success = false, result = "Purchase order not found." };
                    }
                }
                foreach (var product in stockInModel.StockInDetails)
                {
                    await SaveProduct(product, stockInModel.ID == 0 ? tradeID : stockInModel.ID);
                }
                return new { success = true };
            }
            catch (Exception e)
            {
                return new { success = false, result = e.Message };
            }
        }

        public async Task<object> SaveProduct(StockInDetailModel stockInDetailModel, int tradeID)
        {
            try
            {
                var product = await _context.Products.FindAsync(stockInDetailModel.ProductID);
                var trade = await _context.Trades.FindAsync(tradeID);

                if (product == null)
                {
                    return new { success = false, result = "Product not found." };
                }

                if (stockInDetailModel.ID == 0)
                {
                    // Add new StockInItem
                    var newProduct = new StockInItem
                    {
                        TradeID = tradeID,
                        ProductID = stockInDetailModel.ProductID,
                        Quantity = stockInDetailModel.Quantity
                    };

                    _context.StockInItems.Add(newProduct);
                }
                else
                {
                    // Update existing StockInItem
                    var existingProduct = await _context.StockInItems.FindAsync(stockInDetailModel.ID);
                    if (existingProduct == null)
                    {
                        return new { success = false, result = "Stock In item not found." };
                    }
                    existingProduct.Quantity = stockInDetailModel.Quantity;
                    _context.StockInItems.Update(existingProduct);
                }

                await _context.SaveChangesAsync();

                return new { success = true };
            }
            catch (Exception e)
            {
                return new { success = false, result = e.Message };
            }
        }


        public async Task<object> ScanBarcode(string barcode) => (await _procedure.uspBarcodeScanAsync(barcode)) == null ? await Task.FromResult<object>(new { success = false, result = "Barcode not found" }) : (await _procedure.uspBarcodeScanAsync(barcode)).FirstOrDefault();
    }
}
