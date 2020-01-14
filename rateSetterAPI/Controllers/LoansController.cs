using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace rateSetterAPI.Controllers
{
   // [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private LoanDbContext _dbContext;
        public LoansController(LoanDbContext context)
        {
            _dbContext = context;
        }
     
        // GET: /Loans/{username}
        [HttpGet, Route("loans/")]
        public IActionResult Loans()
        {
            try
            {
                var userloans = _dbContext.LoanItem.ToList();
                return Ok(userloans);
            }
            catch (Exception ex)
            {
                return NoContent();
            }            
        }
        
        // GET: /Loans/{username}
        [HttpGet, Route("loans/{username}")]
        public IActionResult Loans(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                    return BadRequest("invalid input");
                var user = _dbContext.LoginItem.SingleOrDefault(a => a.Username == username);
                if (user == null)
                    return BadRequest("invalid input");

                List<Loan> userLoans;
                if (user.Role == "borrower")              
                    userLoans = _dbContext.LoanItem.Where(a => a.RequestedBy == username).ToList();                
                else
                    userLoans = _dbContext.LoanItem.Where(a => a.FundedBy == username || a.FundedBy == null).ToList();

                return Ok(userLoans);
            }
            catch (Exception ex)
            {
                return BadRequest("invalid input");
            }            
        }

        // POST: /Loans/fundLoan
        [HttpPost, Route("fundloan")]
        public IActionResult FundLoan([FromBody]FundLoan model)
        {
            try
            {
                if (!ModelState.IsValid)           
                    return BadRequest("invalid object");                

                var loan = _dbContext.LoanItem.SingleOrDefault(l => l.LoanId == model.LoanId);
                if(loan == null)
                    return BadRequest("invalid object");

                loan.FundedBy = model.FundedBy;
                loan.FundedAt = DateTime.Now;
                loan.LoanStatus = Status.funded.ToString();
                _dbContext.LoanItem.Update(loan);
                _dbContext.SaveChanges();
               
                var newloan = _dbContext.LoanItem.Single(l => l.LoanId == model.LoanId);
                return Ok(newloan);

            }
            catch (Exception ex)
            {
                return BadRequest("invalid object");
            }
        }

        // POST: /Loans/requestLoan
        [HttpPost, Route("requestloan")]
        public IActionResult RequestLoan([FromBody]RequestLoan model)
        {
            try
            {
                if (!ModelState.IsValid)                
                    return BadRequest("invalid object");                
                
                if(!_dbContext.LoanItem.Any(l=>l.LoanName == model.LoanName))
                {
                    _dbContext.LoanItem.Add(new Loan
                    {
                        LoanName = model.LoanName,
                        LoanAmount = model.LoanAmount,
                        RequestedBy = model.RequestedBy,
                        RequestedAt = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        LoanStatus = Status.pending.ToString()
                    });
                    _dbContext.SaveChanges();
                }               

                var newloan = _dbContext.LoanItem.Single(l => l.LoanName == model.LoanName);
                return Ok(newloan);

            }
            catch (Exception ex)
            {
                return BadRequest("invalid object");
            }
        }

        // POST: /statelessLogin
        [HttpPost, Route("statelessLogin")]
        public IActionResult StateLessLogin([FromBody]Login model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("invalid object");

                var user = _dbContext.LoginItem.SingleOrDefault(l => l.Username == model.Username && l.Password == model.Password);
               
                if (user == null)
                    return NotFound("user not found");
                
                return Ok(user);

            }
            catch (Exception ex)
            {
                return BadRequest("invalid object");
            }
        }

    }
}
