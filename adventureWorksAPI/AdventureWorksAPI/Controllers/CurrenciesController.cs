using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdventureWorksAPI.Models;

namespace AdventureWorksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly AdventureWorks2019Context _context;

        public CurrenciesController(AdventureWorks2019Context context)
        {
            _context = context;
        }

        // GET: api/Currencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Currency>>> GetCurrencies()
        {
            return await _context.Currencies.ToListAsync();
        }

        // GET: api/Currencies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Currency>> GetCurrency(string id)
        {
            //obtiene el registro seleccionado por el parametro id de la moneda
            //var currency = await _context.Currencies.FindAsync(id);

            //obtiene el registro seleccionado por el parametro id de la moneda
            //var currency =  _context.Currencies.Where(cur => cur.CurrencyCode == id).FirstOrDefault();

            //obtiene el registro seleccionado por el parametro id, ademas obtiene datos de registros relacionados ejemplo: el pais de la moneda
            //var currency = _context.Currencies.Include(cur => cur.CountryRegionCurrencies)
            //                                  .Where(cur => cur.CurrencyCode == id)
            //                                  .FirstOrDefault();

            //var currency = _context.Currencies.Include(cur => cur.CountryRegionCurrencies)
            //                          .Include(cur => cur.CurrencyRateFromCurrencyCodeNavigations)
            //                          //.Include(cur => cur.CurrencyRateToCurrencyCodeNavigations)
            //                          .Where(cur => cur.CurrencyCode == id)
            //                          .FirstOrDefault();

            var currency = await _context.Currencies.SingleAsync(cur => cur.CurrencyCode == id);

            _context.Entry(currency)
               .Collection(cur => cur.CountryRegionCurrencies)
               .Query()
               .Load();


            if (currency == null)
            {
                return NotFound();
            }

            return currency;
        }

        // PUT: api/Currencies/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCurrency(string id, Currency currency)
        {
            if (id != currency.CurrencyCode)
            {
                return BadRequest();
            }

            _context.Entry(currency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrencyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Currencies
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Currency>> PostCurrency(Currency currency)
        {
            _context.Currencies.Add(currency);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CurrencyExists(currency.CurrencyCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCurrency", new { id = currency.CurrencyCode }, currency);
        }

        // DELETE: api/Currencies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Currency>> DeleteCurrency(string id)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return NotFound();
            }

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();

            return currency;
        }

        private bool CurrencyExists(string id)
        {
            return _context.Currencies.Any(e => e.CurrencyCode == id);
        }
    }
}
