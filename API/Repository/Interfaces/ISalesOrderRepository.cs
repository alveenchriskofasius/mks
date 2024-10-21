﻿using API.Models;

namespace API.Repository.Interfaces
{
    public interface ISalesOrderRepository
    {
        Task<object> Save(SalesOrderModel salesOrder);
        Task<SalesOrderModel> FillForm(int id);
        Task<object> GetSalesOrderDetailById(int id);
        Task<object> DeleteProductById(int id);
        Task<object> GetSearchList();
        Task<object> Delete(int id);
    }
}
