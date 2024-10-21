using API.Context.Table;
using API.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using MitraKaryaSystem.Models;

namespace API.Repository
{
    public class UnitRepository : IUnitRepository
    {
        private readonly MKSTableContext _context;

        public UnitRepository(MKSTableContext context)
        {
            _context = context;
        }

        public async Task<object> SaveUnit(UnitModel unitModel)
        {
            try
            {
                if (unitModel.ID == 0)
                {
                    _context.Units.Add(new Unit
                    {
                        Name = unitModel.UnitName
                    });
                }
                else
                {
                    Unit unit = await _context.Units.FindAsync(unitModel.ID);
                    unit.Name = unitModel.UnitName;
                    _context.Units.Update(unit);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Task.FromResult<object>(new { success = false, error = e.Message });

            }
            return Task.FromResult<object>(new { success = true });
        }
        public async Task<object> GetUnitList() => Task.FromResult<object>(await _context.Units.Select(x => new { x.ID, UnitName = x.Name }).ToListAsync());

        public async Task<UnitModel> FillFormUnit(int id)
        {
            Unit? unit = await _context.Units.FindAsync(id);
            UnitModel? unitModel = null;
            if (unit == null)
            {
                unitModel = new UnitModel();
            }
            else
            {
                unitModel = new UnitModel
                {
                    ID = unit.ID,
                    UnitName = unit.Name
                };
            }
            return unitModel;
        }
        public async Task<object> DeleteUnit(int id)
        {
            try
            {
                _context.Units.Remove(await _context.Units.FindAsync(id));
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Task.FromResult<object>(new { success = false, error = e.Message });

            }
            return Task.FromResult<object>(new { success = true });
        }
    }
}
