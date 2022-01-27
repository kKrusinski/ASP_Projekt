using Autofac.Extras.Moq;
using EquipmentRentalService.Models;
using EquipmentRentalService.Services;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

            using (var mock = AutoMock.GetLoose())
            {
                List<RentalEquipment> expected = GetAll();

                mock.Mock<IRentalService>().Setup(x => x.GetAll()).Returns(GetAll());

                var cls = mock.Create<IRentalService>();

                var actual = cls.GetAll();


                Assert.Equal(expected.Count, actual.Count);
            }
        }
        private List<RentalEquipment> GetAll()
        {
            List<RentalEquipment> list = new List<RentalEquipment>
            {
                new RentalEquipment
                {
                     Name = "Koparka CAT",
                    BaseRentPrice = 250.00,
                    DailyRentPrice = 75.00,
                    OverdueRate = 15,
                    IsAvailable = false
                },
                  new RentalEquipment
                {
                    Name = "Koparka CAT2",

                    BaseRentPrice = 250.00,
                    DailyRentPrice = 75.00,
                    OverdueRate = 15
                },

           new RentalEquipment
            {
                Name = "Koparka CAT3",

                BaseRentPrice = 250.00,
                DailyRentPrice = 75.00,
                OverdueRate = 15
            }

        };
            return list;
        }

    }
}