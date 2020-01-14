using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace rateSetterAPI
{
    public enum Status
    {
        funded = 0,
        pending = 1,
        disapproved = 2,
    }

    public class LoanDbContext : DbContext
    {
        public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options)
        {

        }
        public DbSet<Loan> LoanItem { get; set; }
        public DbSet<Login> LoginItem { get; set; }
    }

    public class Loan
    {
        public long LoanId { get; set; }
        public float LoanAmount { get; set; }
        public string LoanName { get; set; }
        public string LoanStatus { get; set; }
        public string RequestedBy { get; set; }
        public string FundedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime FundedAt { get; set; }
    }
  
    public class RequestLoan
    {
        [Required]
        public string LoanName { get; set; }
        [Required]
        public string RequestedBy { get; set; }
        [Required]
        public float LoanAmount { get; set; }
    }

    public class FundLoan
    {
        [Required]
        public long LoanId { get; set; }
        [Required]
        public string FundedBy { get; set; }
    }

    public class Login
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
