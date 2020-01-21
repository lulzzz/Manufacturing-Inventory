using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Application.Boundaries;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Domain.Buisness.Concrete;
using ManufacturingInventory.Application.Boundaries.Checkout;
using System;

namespace ManufacturingInventory.Tests {
    [TestFixture]
    public class Tests {
        [SetUp]
        public void Setup() {
            
        }

        [Test]
        public void TestTransaction() {
            IUserService userService = new UserService();
            using var context = new ManufacturingContext();
            var user = context.Users
                .Include(e => e.Sessions)
                    .ThenInclude(e => e.Transactions)
                .Include(e => e.Permission)
                .FirstOrDefault(e => e.FirstName == "Andrew");

            if (user != null) {
                Session session = new Session(user);
                context.Sessions.Add(session);
                context.SaveChanges();
                userService.CurrentUser = user;
                userService.CurrentSession = session;
            } else {
                Assert.Fail("User Not Found");
            }


            IRepository<Transaction> transactionRepository = new TransactionRepository(context);
            IRepository<Location> locationRepository = new LocationRepository(context);
            IRepository<Category> categoryRepository = new CategoryRepository(context);
            IRepository<PartInstance> partInstanceRepository = new PartInstanceRepository(context);
            IUnitOfWork unitOfWork = new UnitOfWork(context);

            CheckOutBubbler checkOut = new CheckOutBubbler(userService, transactionRepository, locationRepository, categoryRepository, partInstanceRepository, unitOfWork);

            var location = context.Locations.FirstOrDefault(e => e.Id == 3);
            var instance = context.PartInstances.Include(e => e.BubblerParameter).Include(e => e.CurrentLocation).FirstOrDefault(e => e.Id == 3);
            CheckOutBubblerInput input = new CheckOutBubblerInput();
            input.Items.Add(new CheckOutBubblerInputData(DateTime.Now, instance.Id, location.Id, 1, instance.UnitCost, 52.65, 2300, 5400));
            checkOut.Execute(input).Wait();

        }


    }
}